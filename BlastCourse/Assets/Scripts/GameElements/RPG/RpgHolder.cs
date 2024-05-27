using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMethods;

[System.Serializable]
public class RpgData
{
    [SerializeField] public RpgBase _rpgBehaviour;
    [SerializeField] public RpgStats _rpgStats;
}

public enum FiringMode
{
    Classic = 0,
    Pipe = 1,
    Remote = 2,
    Homing = 3
}

public class RpgHolder : MonoBehaviour
{

    #region Fields
    [Space(5), Header("Shared Values"), Space(3)]
    public AnimationCurve _energyRecoveryRate;
    public Transform _fireOrigin;
    public LayerMask _aimLayerMask;
    public float _maxAimDistance;
    public Camera g_camera;
    public HUD g_HUD;
    public float _fireSpeed;
    public PlayerRotation _rotation;
    public PlayerMovement _player;


    [Tooltip("If there is room, collisions will be disabled until the rocket has covered this distance")] public float IntangibleDistance;
    public LayerMask IntangibleMask;

    [Space(5), Header("RPG Collection"), Space(3)]
    public FiringMode _fireMode;
    public List<RpgData> _rpgList;
    public List<FiringMode> _rpgFiringMode;
    public Dictionary<FiringMode, RpgData> _rpgCollection;


    [Space(5), Header("Strings"), Space(3)]
    public string _primaryFireButtonName;
    public string _secondaryFireButtonName;
    public string _downaimButton;
    public string _classicButtonName;
    public string _remoteButtonName;
    public string _pipeButtonName;
    public string _homingButtonName;

    [Space(5), Header("Audio"), Space(3)]
    public AudioCue failSound;
    public AudioCue rechargeSound;


    #endregion

    #region Variables

    private bool _canShoot = true;
    public bool _canDetonate = true;
    private bool _isDownaiming = false;
    private bool _weaponWheel = false;
    private bool _canQuickSwap = true;

    private RpgData _currentRpg;

    #endregion

    #region UnityFunctions

    private void OnEnable()
    {
        EventManager.OnSelectNewRpg += SetFiringMode;
        EventManager.RocketCount = 0;
        EventManager.GameRpgHolder = this;
    }

    private void OnDisable()
    {
        EventManager.OnSelectNewRpg -= SetFiringMode;
        EventManager.GameRpgHolder = null;
    }

    private void OnValidate()
    {
        SetFiringMode(_fireMode);
    }

    private void Start()
    {
        _rpgCollection = new Dictionary<FiringMode, RpgData>();
        for (int i = 0; i < _rpgList.Count; i++)
        {
            if (!_rpgCollection.ContainsKey(_rpgFiringMode[i])) _rpgCollection.Add(_rpgFiringMode[i], _rpgList[i]);
        }
        _currentRpg = _rpgCollection[_fireMode];

        InitializeAllRpgBehaviours();

        //yield return null;

        if(SaveLoader.Instance != null)
        {
            if (SaveLoader.Instance.UnlockedRpgs == null || SaveLoader.Instance.UnlockedRpgs.Length != 4) SaveLoader.Instance.UnlockedRpgs = new bool[4];

            if (SaveLoader.Instance.startWithAllUnlocks)
            {
                for (int i = 0; i < SaveLoader.Instance.UnlockedRpgs.Length; i++)
                {
                    _rpgCollection[(FiringMode)i]._rpgStats.Unlocked = SaveLoader.Instance.UnlockedRpgs[i] = true;
                }
            }
            else
            {
                for (int i = 0; i < SaveLoader.Instance.UnlockedRpgs.Length; i++)
                {
                    _rpgCollection[(FiringMode)i]._rpgStats.Unlocked = SaveLoader.Instance.UnlockedRpgs[i];
                }

                for (int i = 0; i < 4; i++)
                {
                    //Debug.Log(_rpgCollection[(FiringMode)i]._rpgStats.Unlocked);
                    //Debug.Log(SaveLoader.Instance._rpgs[i]);
                    SaveLoader.Instance.UnlockedRpgs[i] = _rpgCollection[(FiringMode)i]._rpgStats.Unlocked;
                }
            }
        }

        if (ExtendedDataUtility.CheckForValues(SaveLoader.Instance.UnlockedRpgs, true) > 0)
        {
            HideWeapon(false);

            for (int i = 0; i < SaveLoader.Instance.UnlockedRpgs.Length; i++)
            {
                if (SaveLoader.Instance.UnlockedRpgs[i])
                {
                    SetFiringMode((FiringMode)i);
                    break;
                }
            }
        }
        else
        {
            RPGAnimator.Instance.SetHidden(true);
            SetFiringMode(FiringMode.Classic);
            HideWeapon(true);
        }
    }

    private void Update()
    {
        ReadRpgInput();

        _currentRpg._rpgBehaviour.TickSelected();
        foreach (RpgData rpg in _rpgList) rpg._rpgBehaviour.TickUnselected();

        //THIS CHECK IS ONLY RELEVANT ON VALUE CHANGE, SHOULD NOT BE DONE EVEYR FRAME
        //if (_canShoot && ExtendedDataUtility.CheckForValues(SaveLoader.Instance.UnlockedRpgs, true) > 0) HideWeapon(false);
        //else HideWeapon(true);
    }

    #endregion

    #region Methods

    private void ReadRpgInput()
    {
        if (OptionsLoader.TryGetKeyDown(InputActions.Primary_Fire,_primaryFireButtonName) && _currentRpg._rpgStats.Unlocked && _canShoot && Time.timeScale != 0)
        {
            _currentRpg._rpgBehaviour.ReceivePrimaryInput();
        }

        if (OptionsLoader.TryGetKeyDown(InputActions.Secondary_Fire, _secondaryFireButtonName) && _currentRpg._rpgStats.Unlocked && _canDetonate && Time.timeScale != 0)
        {
            _rpgCollection[FiringMode.Remote]._rpgBehaviour.ReceiveSecondaryInput();
        }

        if (_canShoot && Time.timeScale != 0) AimDown(OptionsLoader.TryGetKey(InputActions.Aim_Down, _downaimButton));

        if (OptionsLoader.TryGetKeyDown(InputActions.Classic, _classicButtonName) && _rpgCollection[FiringMode.Classic]._rpgStats.Unlocked && _canQuickSwap)
            QuickSwap(FiringMode.Classic);
        else if (OptionsLoader.TryGetKeyDown(InputActions.Remote, _remoteButtonName) && _rpgCollection[FiringMode.Remote]._rpgStats.Unlocked && _canQuickSwap)
            QuickSwap(FiringMode.Remote);
        else if (OptionsLoader.TryGetKeyDown(InputActions.Pipe, _pipeButtonName) && _rpgCollection[FiringMode.Pipe]._rpgStats.Unlocked && _canQuickSwap)
            QuickSwap(FiringMode.Pipe);
        else if (OptionsLoader.TryGetKeyDown(InputActions.Homing, _homingButtonName) && _rpgCollection[FiringMode.Homing]._rpgStats.Unlocked && _canQuickSwap)
            QuickSwap(FiringMode.Homing);
    }

    public void InitializeAllRpgBehaviours()
    {
        foreach (RpgData rpgData in _rpgList)
        {
            rpgData._rpgBehaviour.InitializeValues(rpgData._rpgStats, this, _aimLayerMask, _maxAimDistance, g_camera, _fireOrigin);
        }
    }

    public void QuickSwap(FiringMode fireMode)
    {
        SetFiringMode(fireMode);
        RPGAnimator.Instance.SetRocket(_fireMode);
    }
    public void SetFiringMode(FiringMode fireMode)
    {
        _fireMode = fireMode;

        if (_rpgCollection != null && _rpgCollection.ContainsKey(_fireMode))
        {
            //RPGAnimator.Instance.SetRocket(_fireMode);
            _currentRpg = _rpgCollection[_fireMode];
            RPGAnimator.Instance.RpgMesh.materials[1].SetColor("_EmissionColor", _currentRpg._rpgStats.AssociatedVisuals.EmissiveColor);
        }

        EventManager.OnChangeRpg?.Invoke(_currentRpg);
    }

    public void PrimaryFireAnimation()
    {
        RPGAnimator.Instance.Shoot();
        if (AudioManager.Instance!= null && _rpgCollection[_fireMode]._rpgStats.ShootingSound.SfxClip != null)AudioManager.TryPlayCueAtPoint(_rpgCollection[_fireMode]._rpgStats.ShootingSound, Vector3.zero);
    }

    public void FailShoot()
    {
        AudioManager.TryPlayCueAtPoint(failSound, Vector3.zero);
    }

    public void EnergyRecharge()
    {
        AudioManager.TryPlayCueAtPoint(rechargeSound, Vector3.zero);
    }

    public void AimDown(bool isAimingDown)
    {
        if(isAimingDown != _isDownaiming)
        {
            RPGAnimator.Instance.SetSSSAngle(isAimingDown, 0);
            _isDownaiming = isAimingDown;
        }

        if(_isDownaiming & _rotation != null) RPGAnimator.Instance.SetSSSAngle(_isDownaiming,_rotation.GetxRot());
    }

    public void AcquireRpg(FiringMode mode)
    {
        HideWeapon(false);
        if (ExtendedDataUtility.CheckForValues(SaveLoader.Instance.UnlockedRpgs, true) == 0) RPGAnimator.Instance.SetHidden(false);
        _rpgCollection[mode]._rpgStats.Unlocked = true;
        SetFiringMode(mode);
        RPGAnimator.Instance.SetRocket(_fireMode);
        if (SaveLoader.Instance != null) SaveLoader.Instance.UnlockedRpgs[(int)mode] = true;
    }

    public void EnableShooting(bool canShoot) 
    {
        _canShoot = canShoot;
        if(ExtendedDataUtility.CheckForValues(SaveLoader.Instance.UnlockedRpgs, true) > 0) HideWeapon(!canShoot);
    }

    public void WeaponWheel(bool isOpen)
    {
        _weaponWheel = isOpen;
        EnableShooting(!isOpen);
        EnableRemoteDetonation(!isOpen);
        _canQuickSwap = !isOpen;
    }

    public void EnableRemoteDetonation(bool canDetonate)
    {
        _canDetonate = canDetonate;
    }

    public void HideWeapon(bool hide)
    {
        if(!_weaponWheel) RPGAnimator.Instance.SetHidden(hide);
        if (g_HUD != null) g_HUD.HideEnergy(hide);
    }

    public bool IsAimingDown()
    {
        return _isDownaiming;
    }
    public Vector3 GetTargetPoint() 
    {
        return _player.GetCoyote() ? g_camera.transform.position + (_player.GetSSSPosition() - g_camera.transform.position) * _maxAimDistance : g_camera.transform.position + Vector3.down * _maxAimDistance;
    }

    #endregion
}
