using System;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using CustomMethods;

public class HUD : MonoBehaviour
{
    #region Fields
    [Space(5), Header("Energy"), Space(2)]
    [SerializeField] EnergyBarValues _energyBarValues;
    [Serializable] public class EnergyBarValues
    {
        public Image _energyBar;
        public Color _notEnoughEnergyTint;
    }

    [Space(5), Header("Health"), Space(2)]
    public Image _healthTint;

    [Space(5), Header("Interaction"), Space(2)]
    [SerializeField] InteractableIconValues _actionIconValues;
    private float _actionIconBaseAlpha;
    [Serializable] public class InteractableIconValues
    {
        public Image _actionIcon;
        public float _actionIconFadeTime;
    }

    [Space(5), Header("SpeedMeter"), Space(2)]
    public GameObject g_speedMeter;
    public TextMeshProUGUI _speedTextXYZ;
    public TextMeshProUGUI _speedTextXZ;
    public TextMeshProUGUI _speedTextY;

    [Space(5), Header("Timer"), Space(2)]
    public GameObject g_timer;
    public TextMeshProUGUI _timerText;
    public TextMeshProUGUI _segmentText;

    [Space(5), Header("SpeedParticles"), Space(2)]
    [SerializeField] ParticleSystem _speedParticles;
    [SerializeField] ParticleSystem _speedBackParticles;
    [SerializeField] float _particleDecayRate;
    [SerializeField] float _particleMinThreshold;
    [SerializeField] float _particleMaxThreshold;
    [SerializeField] int _particleMaxCount;
    [SerializeField] float _backParticleMinThreshold;
    [SerializeField] float _backParticleMaxThreshold;
    [SerializeField] int _backParticleMaxCount;

    [Space(5), Header("Weapon Wheel"), Space(2)]
    [SerializeField] WeaponWheelValues _weaponWheelValues;

    [Serializable] public class WeaponWheelValues
    {
        public string _weaponSelectButtonName;
        [Space(3)] public UnityEvent _onOpenWeaponWheel;
        [Space(3)] public UnityEvent _onCloseWeaponWheel;
    }

    [Space(5), Header("Save Icon"), Space(2)]
    [SerializeField] Animator _saveIcon;

    #endregion

    #region Vars

    private Action<float> UpdateSpeedXYZ;
    public enum SpeedMeterType
    {
        Simple,
        Complex,
        Hidden
    }

    private RpgData _currentRpg;

    #endregion

    #region UnityEvents

    private void OnEnable()
    {
        EventManager.OnUpdateEnergy += UpdateEnergy;
        EventManager.OnChangeRpg += UpdateCurrentRpg;
        EventManager.OnSelectNewInteractable += SetInteractable;
        EventManager.OnSaveGame += NotifySave;
        EventManager.OnUpdateHealth += UpdateTint;
        EventManager.OnPlayerDeath += PlayerFuckingDies;
        EventManager.OnUpdatePlayerLocalVelocity += UpdateSpeedParticles;
        EventManager.OnActivateExtraHUD += SetExtraHUD;
    }

    private void OnDisable()
    {
        EventManager.OnUpdateEnergy -= UpdateEnergy;
        EventManager.OnChangeRpg -= UpdateCurrentRpg;
        EventManager.OnSelectNewInteractable -= SetInteractable;
        EventManager.OnSaveGame -= NotifySave;
        EventManager.OnUpdateHealth -= UpdateTint;
        EventManager.OnPlayerDeath -= PlayerFuckingDies;
        EventManager.OnUpdatePlayerLocalVelocity -= UpdateSpeedParticles;
        EventManager.OnActivateExtraHUD -= SetExtraHUD;

        EventManager.OnUpdatePlayerVelocity -= UpdateSpeedMeter;
        EventManager.OnTimeTick -= UpdateTime;
    }

    private void Start()
    {
        OptionsLoader.Instance.UpdateConfig();
        UpdateLastTime(SpeedLoader.Instance.GetPrevTime());

        _actionIconBaseAlpha = _actionIconValues._actionIcon.color.a;
        SetInteractable(null);

        _weaponWheelValues._onCloseWeaponWheel?.Invoke();
        EventManager.OnCloseWeaponWheel?.Invoke();
    }

    private void Update()
    {
        if (EventManager.IsDead) return;

        if (OptionsLoader.TryGetKeyDown(InputActions.Weapon_Wheel,_weaponWheelValues._weaponSelectButtonName) && 
            ExtendedDataUtility.CheckForValues(SaveLoader.Instance.UnlockedRpgs.ToList(),true) >= 2)
        {
            _weaponWheelValues?._onOpenWeaponWheel?.Invoke();
            EventManager.OnOpenWeaponWheel?.Invoke();
        }
        if (OptionsLoader.TryGetKeyUp(InputActions.Weapon_Wheel, _weaponWheelValues._weaponSelectButtonName))
        {
            CloseWW();
        }
    }

    public void CloseWW()
    {
        _weaponWheelValues._onCloseWeaponWheel?.Invoke();
        EventManager.OnCloseWeaponWheel?.Invoke();
    }

    #endregion

    #region Methods

    #region Rpg
    private void UpdateCurrentRpg(RpgData rpgData)
    {
        if(rpgData != null)
        {
            _currentRpg = rpgData;
            _energyBarValues?._energyBar?.material.SetFloat("_segmentCount", Mathf.Round(1 / rpgData._rpgBehaviour.Cost));
        }
    }

    private void UpdateEnergy(float e)
    {
        if (ExtendedDataUtility.CheckForValues(SaveLoader.Instance.UnlockedRpgs, true) > 0)
        {
            if (!_energyBarValues._energyBar.enabled) _energyBarValues._energyBar.enabled = true;
            _energyBarValues._energyBar.material.SetFloat("_fill", e);
            _energyBarValues._energyBar.material.SetColor(
                "_fillColor",
                ExtendedDataUtility.Select<Color>(e >= _currentRpg._rpgBehaviour.Cost, _currentRpg._rpgStats.AssociatedVisuals.LightColor,
                _energyBarValues._notEnoughEnergyTint));
        }
        else if (_energyBarValues._energyBar.enabled) _energyBarValues._energyBar.enabled = false;
    }

    public void HideEnergy(bool hide)
    {
        _energyBarValues._energyBar.enabled = !hide;
    }
    #endregion

    #region Actions

    private void SetInteractable(IInteractable interactable)
    {
        _actionIconValues._actionIcon.DOFade( ExtendedDataUtility.Select(interactable == null, 0f, _actionIconBaseAlpha), _actionIconValues._actionIconFadeTime);
    }

    private void NotifySave()
    {
        _saveIcon.SetTrigger("Save");
    }

    #endregion

    #region SpeedMeter

    private void SetExtraHUD(bool set)
    {
        g_speedMeter.SetActive(set);
        g_timer.SetActive(set);

        if (set)
        {
            EventManager.OnUpdatePlayerVelocity += UpdateSpeedMeter;
            EventManager.OnTimeTick += UpdateTime;
        }
        else
        {
            EventManager.OnUpdatePlayerVelocity -= UpdateSpeedMeter;
            EventManager.OnTimeTick -= UpdateTime;
        }

    }

    private void UpdateLastTime(float t)
    {
        int mm = Mathf.FloorToInt(t / 60);
        int ss = Mathf.FloorToInt(t % 60);
        int ms = Mathf.FloorToInt((t * 100) % 100);
        _segmentText.text = string.Format("{0:00}:{1:00},{2:00}", mm, ss, ms);
    }

    private void UpdateTime(float t)
    {
        int mm = Mathf.FloorToInt(t / 60);
        int ss = Mathf.FloorToInt(t % 60);
        int ms = Mathf.FloorToInt((t * 100) % 100);
        _timerText.text = string.Format("{0:00}:{1:00},{2:00}", mm, ss, ms);
    }

    private void UpdateSpeedMeter(Vector3 speed)
    {
        if (g_speedMeter != null && g_speedMeter.activeInHierarchy)
        {
            _speedTextXYZ.text = speed.magnitude.ToString("00.00");
            _speedTextXZ.text = ExtendedMathUtility.VectorXZMagnitude(speed).ToString("00.00");
            _speedTextY.text = MathF.Abs(speed.y).ToString("00.00");
        }
    }

    float em1 = 0;
    float em2 = 0;
    private void UpdateSpeedParticles(Vector3 speed)
    {
        ParticleSystem.EmissionModule m1 = _speedParticles.emission;
        ParticleSystem.EmissionModule m2 = _speedBackParticles.emission;
        
        if(speed.y > _particleMinThreshold)
        {
            em1 = Mathf.Clamp01((Mathf.Abs(speed.y) - _particleMinThreshold) / (_particleMaxThreshold - _particleMinThreshold)) * _particleMaxCount;
            em2 = 0;
        }
        else if(speed.y < -_backParticleMinThreshold)
        {
            em1 = 0;
            em2 = Mathf.Clamp01((Mathf.Abs(speed.y) - _backParticleMinThreshold) / (_backParticleMaxThreshold - _backParticleMinThreshold)) * _backParticleMaxCount;
        }
        else
        {
            em1 = Mathf.Lerp(em1, 0,Time.deltaTime * _particleDecayRate);
            if (em1 <= 1) em1 = 0;
            em2 = Mathf.Lerp(em2, 0, Time.deltaTime * _particleDecayRate);
            if (em2 <= 1) em2 = 0;
        }

        m1.rateOverTime = em1;
        m2.rateOverTime = em2;
    }
    #endregion

    #region Health

    public void UpdateTint(float healthPercent)
    {
        if(healthPercent >= 0.99f) _healthTint.gameObject.SetActive(false);
        else
        {
            if (!_healthTint.gameObject.activeSelf) _healthTint.gameObject.SetActive(true);
            _healthTint.material.SetFloat("_Health", healthPercent);
        }
    }

    public void PlayerFuckingDies()
    {
        //disable all HUD effects except damage
        foreach (Transform child in transform)
        {
            if (child.gameObject.Equals(_healthTint.gameObject)) continue;
            child.gameObject.SetActive(false);
        }
    }

    #endregion

    #endregion
}

