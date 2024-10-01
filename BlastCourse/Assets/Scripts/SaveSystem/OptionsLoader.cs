using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum InputActions
{
    Forward = 0,
    Back = 1,
    Left = 2,
    Right = 3,
    Jump = 4,
    Aim_Down = 5,
    Primary_Fire = 6,
    Secondary_Fire = 7,
    Weapon_Wheel = 8,
    Interact = 9,
    Classic = 10,
    Remote = 11,
    Pipe = 12,
    Homing = 13
}

[Serializable]
public class OptionsLoader : MonoBehaviour
{
    #region Vars

    public static OptionsLoader Instance;


    [HideInInspector] public KeyCode[] Keys;

    [HideInInspector] public float Sensitivity;

    [HideInInspector]
    public float MasterVolume
    {
        get { return _masterVolume; }
        set
        {
            _masterVolume = Mathf.Clamp(value, 0.0001f, 1);
            if (_audioMixer != null) _audioMixer.SetFloat("VolumeMaster", Mathf.Log10(_masterVolume) * 20);
        }
    }
    private float _masterVolume;
    [HideInInspector] public float SfxVolume 
    { 
        get { return _sfxVolume; }
        set { _sfxVolume = Mathf.Clamp(value,0.0001f,1);
            if (_audioMixer != null) _audioMixer.SetFloat("VolumeEffects", Mathf.Log10(_sfxVolume) * 20);
        }
    }
    private float _sfxVolume;
    [HideInInspector] public float MusicVolume
    {
        get { return _musicVolume; }
        set
        {
            _musicVolume = Mathf.Clamp(value, 0.0001f, 1);
            if (_audioMixer != null) _audioMixer.SetFloat("VolumeMusic", Mathf.Log10(_musicVolume) * 20);
        }
    }
    private float _musicVolume;
    [HideInInspector] public float DialogueVolume
    {
        get { return _dialogueVolume; }
        set
        {
            _dialogueVolume = Mathf.Clamp(value, 0.0001f, 1);
            if (_audioMixer != null) _audioMixer.SetFloat("VolumeDialogue", Mathf.Log10(_dialogueVolume) * 20);
        }
    }
    private float _dialogueVolume;

    [HideInInspector]
    public float CameraShake
    {
        get { return _camShake; }
        set
        {
            _camShake = Mathf.Clamp(value, 0, 2);
        }
    }
    private float _camShake;

    [HideInInspector]
    public float FieldOfView
    {
        get { return _fieldOfView; }
        set
        {
            _fieldOfView = Mathf.Clamp(value, 50, 120);
        }
    }
    private float _fieldOfView;

    [HideInInspector] public bool Fullscreen;
    [HideInInspector] public bool HoldGrab;
    [HideInInspector] public bool ExtraHUD;

    [SerializeField] private OptionsSO _defaultOptions;
    [SerializeField] private AudioMixer _audioMixer;

    public float _timeTick;
    public float _timeSegment;
    #endregion

    #region UnityFunctions

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        Load();
    }

    private void Update()
    {
        _timeTick += Time.deltaTime;
        if (ExtraHUD) EventManager.OnTimeTick?.Invoke(_timeTick);
    }

    #endregion

    #region Methods

    public void UpdateConfig()
    {
        Screen.fullScreen = Fullscreen;

        //Set the Audio Mixers
        if (_audioMixer != null) _audioMixer.SetFloat("VolumeMaster", Mathf.Log10(_masterVolume) * 20);
        if (_audioMixer != null) _audioMixer.SetFloat("VolumeEffects", Mathf.Log10(_sfxVolume) * 20);
        if (_audioMixer != null) _audioMixer.SetFloat("VolumeMusic", Mathf.Log10(_musicVolume) * 20);
        if (_audioMixer != null) _audioMixer.SetFloat("VolumeDialogue", Mathf.Log10(_dialogueVolume) * 20);

        EventManager.OnActivateExtraHUD?.Invoke(ExtraHUD);
        EventManager.OnFovChanged?.Invoke(FieldOfView);

        //if (FovController.instance != null) FovController.instance.SetFov(FieldOfView);
    }

    [ContextMenu("Save")]
    public void Save()
    {
        SaveSystem.OptionsSave(Sensitivity,MasterVolume,SfxVolume,MusicVolume,DialogueVolume,Fullscreen,Keys,HoldGrab,CameraShake,FieldOfView, ExtraHUD);
        UpdateConfig();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        //Get Data
        OptionsData data = SaveSystem.OptionsLoad();

        if (data != null)
        {
            Sensitivity = data._sensitivity;
            MasterVolume = data._masterVolume;
            SfxVolume = data._sfxVolume;
            MusicVolume = data._musicVolume;
            DialogueVolume = data._dialogueVolume;
            Fullscreen = data._fullscreen;
            HoldGrab = data._holdGrab;
            ExtraHUD = data._extraHUD;
            CameraShake = data._camShake;
            FieldOfView = data._fieldOfView;


            Keys = new KeyCode[Enum.GetValues(typeof(InputActions)).Length];
            if (data._inputKeys != null)
                for (int i = 0; i < data._inputKeys.Length; i++)
                {
                    Keys[i] = data._inputKeys[i];
                }
            else if (_defaultOptions.Keys != null)
                for (int i = 0; i < _defaultOptions.Keys.Length; i++)
                {
                    Keys[i] = _defaultOptions.Keys[i];
                }
        }
        else
        {
            ResetOptions();
        }

        UpdateConfig();
    }

    [ContextMenu("Reset")]
    public void ResetOptions()
    {
        if (_defaultOptions != null)
        {
            Sensitivity = _defaultOptions.Sensitivity;
            MasterVolume = _defaultOptions.MasterVolume;
            SfxVolume = _defaultOptions.SfxVolume;
            MusicVolume = _defaultOptions.MusicVolume;
            DialogueVolume = _defaultOptions.DialogueVolume;
            Fullscreen = _defaultOptions.Fullscreen;
            HoldGrab = _defaultOptions.HoldToGrab;
            CameraShake = _defaultOptions.CameraShake;
            FieldOfView = _defaultOptions.Fov;
            ExtraHUD = _defaultOptions.ExtraHUD;


            Keys = new KeyCode[Enum.GetValues(typeof(InputActions)).Length];
            if (_defaultOptions.Keys != null)
                for (int i = 0; i < _defaultOptions.Keys.Length; i++)
                {
                    Keys[i] = _defaultOptions.Keys[i];
                }
            Save();
        }
        else Debug.LogError("Missing Default Options");
    }

    public bool ChangesFromSave()
    {
        OptionsData data = SaveSystem.OptionsLoad();

        if (data != null
            && Sensitivity == data._sensitivity
            && MasterVolume == data._masterVolume
            && SfxVolume == data._sfxVolume
            && MusicVolume == data._musicVolume
            && DialogueVolume == data._dialogueVolume
            && Fullscreen == data._fullscreen
            && HoldGrab == data._holdGrab
            && ExtraHUD == data._extraHUD
            && CameraShake == data._camShake
            && FieldOfView == data._fieldOfView
            && !AreControlsChanged(data))
        {
            return false;
        }
        else if (data == null
            && Sensitivity == _defaultOptions.Sensitivity
            && MasterVolume == _defaultOptions.MasterVolume
            && SfxVolume == _defaultOptions.SfxVolume
            && MusicVolume == _defaultOptions.MusicVolume
            && DialogueVolume == _defaultOptions.DialogueVolume
            && Fullscreen == _defaultOptions.Fullscreen
            && HoldGrab == _defaultOptions.HoldToGrab
            && ExtraHUD == _defaultOptions.ExtraHUD
            && CameraShake == _defaultOptions.CameraShake
            && FieldOfView == _defaultOptions.Fov
            && !AreControlsChanged(null))
        {
            return false;
        }
        else return true;
    }

    private bool AreControlsChanged(OptionsData data)
    {
        bool equal = true;
        if (data._inputKeys != null)
            for (int i = 0; i < data._inputKeys.Length; i++)
            {
                if (Keys[i] == data._inputKeys[i])
                {
                    equal = false;
                    return equal;
                }
            }
        else if (_defaultOptions.Keys != null)
            for (int i = 0; i < _defaultOptions.Keys.Length; i++)
            {
                if (Keys[i] == _defaultOptions.Keys[i])
                {
                    equal = false;
                    return equal;
                }
            }

        return equal;
    }


    /// <summary>
    /// Returns the value of the given name held in the current OptionsLoader Instance. This method does not guarantee the Instance exists, returns compilation default in that case.
    /// </summary>
    public static T TryGetValueFromInstance<T>(string valueName)
    {
        return Instance != null ? (T)Instance.GetType().GetField(valueName).GetValue(Instance) : default;
    }

    /// <summary>
    /// Returns the value of the given name held in the current OptionsLoader Instance. This method does not guarantee the Instance exists, returns given default in that case.
    /// </summary>
    public static T TryGetValueFromInstance<T>(string valueName, T defaultValue)
    {
        return Instance != null ? (T)Instance.GetType().GetField(valueName).GetValue(Instance) : defaultValue;
    }

    /// <summary>
    /// Returns a key press using the given InputAction, or a given value if there is no instance
    /// </summary>
    public static bool TryGetKeyDown(InputActions key, string defaultKeyName)
    {
        if (Instance != null && Instance.Keys != null) return Input.GetKeyDown(Instance.Keys[(int)key]);
        else return Input.GetButtonDown(defaultKeyName);
    }
    /// <summary>
    /// Returns a key hold using the given InputAction, or a given value if there is no instance
    /// </summary>
    public static bool TryGetKey(InputActions key, string defaultKeyName)
    {
        if (Instance != null && Instance.Keys != null) return Input.GetKey(Instance.Keys[(int)key]);
        else return Input.GetButton(defaultKeyName);
    }
    /// <summary>
    /// Returns a key press using the given InputAction, or a given value if there is no instance
    /// </summary>
    public static bool TryGetKeyUp(InputActions key, string defaultKeyName)
    {
        if (Instance != null && Instance.Keys != null) return Input.GetKeyUp(Instance.Keys[(int)key]);
        else return Input.GetButtonUp(defaultKeyName);
    }
    /// <summary>
    /// Returns a key inside an axis raw hold using the given InputAction, or a given value if there is no instance
    /// </summary>
    public static float TryGetAxisRaw(InputActions positiveKey, InputActions negativeKey, string defaultKeyName)
    {
        if (Instance != null && Instance.Keys != null) return (Input.GetKey(Instance.Keys[(int)positiveKey]) ? 1 : 0) + (Input.GetKey(Instance.Keys[(int)negativeKey]) ? -1 : 0);
        else return Input.GetAxisRaw(defaultKeyName);
    }

    #endregion
}


