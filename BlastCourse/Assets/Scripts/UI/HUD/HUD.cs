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
    [SerializeField] SpeedMeterValues _speedMeterValues;
    [Serializable] public class SpeedMeterValues
    {
        public SpeedMeterType _speedMeterType;

        [Space(3), Header("Simple SpeedMeter"), Space(2)]
        public SimpleSpeedMeterValues _simpleSpeedMeterValues;

        [Space(3), Header("Complex SpeedMeter"), Space(2)]
        public ComplexSpeedMeterValues _complexSpeedMeterValues;

        [Serializable] public class SimpleSpeedMeterValues
        {
            public GameObject g_simpleSpeedMeter;
            public TextMeshProUGUI _simpleSpeedText;
        }
        [Serializable] public class ComplexSpeedMeterValues
        {
            public GameObject g_complexSpeedMeter;
            public TextMeshProUGUI _complexSpeedTextXYZ;
            public TextMeshProUGUI _complexSpeedTextXZ;
            public TextMeshProUGUI _complexSpeedTextY;
        }
    }

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
    }

    private void OnDisable()
    {
        EventManager.OnUpdateEnergy -= UpdateEnergy;
        EventManager.OnChangeRpg -= UpdateCurrentRpg;
        EventManager.OnSelectNewInteractable -= SetInteractable;
        EventManager.OnSaveGame -= NotifySave;
        EventManager.OnUpdateHealth -= UpdateTint;
        EventManager.OnPlayerDeath -= PlayerFuckingDies;
        UnsusbsribeAllSpeedMeterUpdates();
    }

    private void OnValidate()
    {
        ValidateSpeedmeterType();
    }

    private void Start()
    {
        ValidateSpeedmeterType();

        _actionIconBaseAlpha = _actionIconValues._actionIcon.color.a;
        SetInteractable(null);

        _weaponWheelValues._onCloseWeaponWheel?.Invoke();
        EventManager.OnCloseWeaponWheel?.Invoke();
    }

    private void Update()
    {
        if (OptionsLoader.TryGetKeyDown(InputActions.Weapon_Wheel,_weaponWheelValues._weaponSelectButtonName) && 
            ExtendedDataUtility.CheckForValues(SaveLoader.Instance.UnlockedRpgs.ToList(),true) >= 2)
        {
            _weaponWheelValues?._onOpenWeaponWheel?.Invoke();
            EventManager.OnOpenWeaponWheel?.Invoke();
        }
        if (OptionsLoader.TryGetKeyUp(InputActions.Weapon_Wheel, _weaponWheelValues._weaponSelectButtonName))
        {
            _weaponWheelValues._onCloseWeaponWheel?.Invoke();
            EventManager.OnCloseWeaponWheel?.Invoke();
        }
    }

    #endregion

    #region Methods

    #region Rpg
    private void UpdateCurrentRpg(RpgData rpgData)
    {
        if(rpgData != null)
        {
            _currentRpg = rpgData;
            _energyBarValues._energyBar.material.SetFloat("_segmentCount", Mathf.Round(1 / rpgData._rpgBehaviour.Cost));
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
    private void UpdateSpeedXZ(float spdXZ)
    {
        _speedMeterValues._complexSpeedMeterValues._complexSpeedTextXZ.text = spdXZ.ToString("00.00");
    }

    private void UpdateSpeedY(float spdY)
    {
        _speedMeterValues._complexSpeedMeterValues._complexSpeedTextY.text = Mathf.Abs(spdY).ToString("00.00");
    }

    private void ValidateSpeedmeterType()
    {
        UnsusbsribeAllSpeedMeterUpdates();

        switch (_speedMeterValues._speedMeterType)
        {
            case SpeedMeterType.Simple:
            default:
                //SIMPLE SPEED METER, DEACTIVATE BROKEN UP VECTOR AND ACTIVATE SIMPLE DISPLAY
                _speedMeterValues._simpleSpeedMeterValues.g_simpleSpeedMeter.SetActive(true);
                _speedMeterValues._complexSpeedMeterValues.g_complexSpeedMeter.SetActive(false);

                //Asign functionaly to event
                UpdateSpeedXYZ = (float spdXYZ) =>
                {
                    _speedMeterValues._simpleSpeedMeterValues._simpleSpeedText.text = spdXYZ.ToString("00.00");
                };

                //subscribe update methods
                EventManager.OnUpdatePlayerSpeedXYZ += UpdateSpeedXYZ;

                break;

            case SpeedMeterType.Complex:
                //SIMPLE SPEED METER, DEACTIVATE BROKEN UP VECTOR AND ACTIVATE SIMPLE DISPLAY
                _speedMeterValues._simpleSpeedMeterValues.g_simpleSpeedMeter.SetActive(false);
                _speedMeterValues._complexSpeedMeterValues.g_complexSpeedMeter.SetActive(true);

                //Asign functionaly to event
                UpdateSpeedXYZ = (float spdXYZ) =>
                {
                    _speedMeterValues._complexSpeedMeterValues._complexSpeedTextXYZ.text = spdXYZ.ToString("00.00");
                };

                //subscribe update methods
                EventManager.OnUpdatePlayerSpeedXYZ += UpdateSpeedXYZ;
                EventManager.OnUpdatePlayerSpeedXZ += UpdateSpeedXZ;
                EventManager.OnUpdatePlayerSpeedY += UpdateSpeedY;

                break;

            case SpeedMeterType.Hidden:

                _speedMeterValues._simpleSpeedMeterValues.g_simpleSpeedMeter.SetActive(false);
                _speedMeterValues._complexSpeedMeterValues.g_complexSpeedMeter.SetActive(false);

                break;
        }
    }

    private void UnsusbsribeAllSpeedMeterUpdates()
    {
        EventManager.OnUpdatePlayerSpeedXYZ -= UpdateSpeedXYZ;
        EventManager.OnUpdatePlayerSpeedXZ -= UpdateSpeedXZ;
        EventManager.OnUpdatePlayerSpeedY -= UpdateSpeedY;
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
        //close weapon wheel and prevent it from opening again
        _weaponWheelValues._onCloseWeaponWheel?.Invoke();
        EventManager.OnCloseWeaponWheel?.Invoke();
        _weaponWheelValues._weaponSelectButtonName = "";

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

