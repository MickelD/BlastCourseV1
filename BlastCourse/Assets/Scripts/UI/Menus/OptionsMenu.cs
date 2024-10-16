using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OptionsMenu : MonoBehaviour
{
    #region Fields

    [Space(3), Header("Menu"), Space(3)]
    [SerializeField] private GameObject _gameplayMenu;
    [SerializeField] private GameObject _musicMenu;
    [SerializeField] private GameObject _controlMenu;
    [SerializeField] private ChangeControlPopup _changeControlPopUp;

    [Space(3), Header("Sensitivity"), Space(3)]
    [SerializeField] private float _minSensitivity = 40;
    [SerializeField] private float _maxSensitivity = 300;
    [SerializeField] private Slider _senseSlider;
    [SerializeField] private TextMeshProUGUI _senseValue;

    [Space(3), Header("Master"), Space(3)]
    [SerializeField] private float _minMasterVolume = 0;
    [SerializeField] private float _maxMasterVolume = 100;
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private TextMeshProUGUI _masterValue;

    [Space(3), Header("SFX"), Space(3)]
    [SerializeField] private float _minSfxVolume = 0;
    [SerializeField] private float _maxSfxVolume = 100;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private TextMeshProUGUI _sfxValue;

    [Space(3), Header("Music"), Space(3)]
    [SerializeField] private float _minMusicVolume = 0;
    [SerializeField] private float _maxMusicVolume = 100;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private TextMeshProUGUI _musicValue;

    [Space(3), Header("Dialogue"), Space(3)]
    [SerializeField] private float _minDialogueVolume = 0;
    [SerializeField] private float _maxDialogueVolume = 100;
    [SerializeField] private Slider _dialogueSlider;
    [SerializeField] private TextMeshProUGUI _dialogueValue;

    [Space(3), Header("Fullscreen"), Space(3)]
    [SerializeField] private Toggle _fullscreen;

    [Space(3), Header("Hold To Grab"), Space(3)]
    [SerializeField] private Toggle _holdGrab;

    [HideInInspector] public TextMeshProUGUI[] _keyNames;

    #endregion

    #region Variables
    public enum MenuScreen
    {
        Gameplay,
        Music,
        Controls
    }
    private MenuScreen _currentMenu;

    #endregion

    #region UnityFunctions

    private void OnValidate()
    {
        _maxSensitivity = _maxSensitivity > 100 ? _maxSensitivity : 100;
        _minSensitivity = _minSensitivity < 100 ? _minSensitivity : 100;

        _maxSfxVolume = _maxSfxVolume > 100 ? _maxSfxVolume : 100;
        _minSfxVolume = _minSfxVolume < 100 ? _minSfxVolume : 100;

        _maxMasterVolume = _maxMasterVolume > 100 ? _maxMasterVolume : 100;
        _minMasterVolume = _minMasterVolume < 100 ? _minMasterVolume : 100;

        _maxMusicVolume = _maxMusicVolume > 100 ? _maxMusicVolume : 100;
        _minMusicVolume = _minMusicVolume < 100 ? _minMusicVolume : 100;

        _maxDialogueVolume = _maxDialogueVolume > 100 ? _maxDialogueVolume : 100;
        _minDialogueVolume = _minDialogueVolume < 100 ? _minDialogueVolume : 100;
    }

    private void OnEnable()
    {
        GameplayScreen();
    }

    #endregion

    #region Methods

    public void Sensitivity()
    {
        if (_senseSlider != null
            && _senseValue != null)
        {
            _senseValue.text = ((int)_senseSlider.value).ToString() + "%";
            if (OptionsLoader.Instance != null) OptionsLoader.Instance.Sensitivity = _senseSlider.value / 100;
        }
    }

    public void SFX()
    {
        if (_sfxSlider != null
            && _sfxValue != null)
        {
            _sfxValue.text = ((int)_sfxSlider.value).ToString() + "%";
            if (OptionsLoader.Instance != null) OptionsLoader.Instance.SfxVolume = _sfxSlider.value / 100;
        }
    }

    public void Music()
    {
        if (_musicSlider != null
            && _musicValue != null)
        {
            _musicValue.text = ((int)_musicSlider.value).ToString() + "%";
            if (OptionsLoader.Instance != null) OptionsLoader.Instance.MusicVolume = _musicSlider.value / 100;
        }
    }

    public void Master()
    {
        if (_masterSlider != null
            && _masterValue != null)
        {
            _masterValue.text = ((int)_masterSlider.value).ToString() + "%";
            if (OptionsLoader.Instance != null) OptionsLoader.Instance.MasterVolume = _masterSlider.value / 100;
        }
    }

    public void Dialogue()
    {
        if (_dialogueSlider != null
            && _dialogueValue != null)
        {
            _dialogueValue.text = ((int)_dialogueSlider.value).ToString() + "%";
            if (OptionsLoader.Instance != null) OptionsLoader.Instance.DialogueVolume = _dialogueSlider.value / 100;
        }
    }

    public void Fullscreen()
    {
        if (OptionsLoader.Instance != null) OptionsLoader.Instance.Fullscreen = _fullscreen.isOn;
        Screen.fullScreen = _fullscreen.isOn;
    }

    public void HoldGrab()
    {
        if (OptionsLoader.Instance != null) OptionsLoader.Instance.HoldGrab = _holdGrab.isOn;
    }

    public void ResetOptions()
    {
        if(OptionsLoader.Instance != null)
        {
            OptionsLoader.Instance.ResetOptions();
            UpdateSliders();
        }
    }

    public void Back()
    {
        gameObject.SetActive(false);
        if (OptionsLoader.Instance != null) OptionsLoader.Instance.Load();
    }

    public void Apply()
    {
        if (OptionsLoader.Instance != null) OptionsLoader.Instance.Save();
        Debug.Log("Apply");
    }

    public void ChangeScreen(MenuScreen screen)
    {
        _currentMenu = screen;
        switch (_currentMenu)
        {
            case MenuScreen.Controls:
                _controlMenu.SetActive(true);
                _musicMenu.SetActive(false);
                _gameplayMenu.SetActive(false);
                break;
            case MenuScreen.Gameplay:
                _controlMenu.SetActive(false);
                _musicMenu.SetActive(false);
                _gameplayMenu.SetActive(true);
                break;
            case MenuScreen.Music:
                _controlMenu.SetActive(false);
                _musicMenu.SetActive(true);
                _gameplayMenu.SetActive(false);
                break;
        }
        UpdateSliders();
    }
    public void GameplayScreen() { ChangeScreen(MenuScreen.Gameplay); }
    public void AudioScreen() { ChangeScreen(MenuScreen.Music); }
    public void ControlScreen() { ChangeScreen(MenuScreen.Controls); }

    public void OpenControlPopUp(int changeControl)
    {
        _changeControlPopUp.gameObject.SetActive(true);
        _changeControlPopUp.currentInput = changeControl;
    }

    public void UpdateSliders()
    {
        if(OptionsLoader.Instance != null)
        {
            if (_senseSlider != null)_senseSlider.value = OptionsLoader.Instance.Sensitivity * 100;
            if (_senseValue != null) _senseValue.text = ((int)_senseSlider.value).ToString() + "%";

            if (_masterSlider != null) _masterSlider.value = OptionsLoader.Instance.MasterVolume * 100;
            if (_masterValue != null) _masterValue.text = ((int)_masterSlider.value).ToString() + "%";
            if (_sfxSlider != null) _sfxSlider.value = OptionsLoader.Instance.SfxVolume * 100;
            if (_sfxValue != null) _sfxValue.text = ((int)_sfxSlider.value).ToString() + "%";
            if (_musicSlider != null) _musicSlider.value = OptionsLoader.Instance.MusicVolume * 100;
            if (_musicValue != null) _musicValue.text = ((int)_musicSlider.value).ToString() + "%";
            if (_dialogueSlider != null) _dialogueSlider.value = OptionsLoader.Instance.DialogueVolume * 100;
            if (_dialogueValue != null) _dialogueValue.text = ((int)_dialogueSlider.value).ToString() + "%";

            if (_fullscreen != null) _fullscreen.isOn = OptionsLoader.Instance.Fullscreen;

            if (_holdGrab != null) _holdGrab.isOn = OptionsLoader.Instance.HoldGrab;

            if (_keyNames != null)
                for (int i = 0; i < _keyNames.Length; i++)
                {
                    if (OptionsLoader.Instance.Keys[i] == KeyCode.Mouse0) _keyNames[i].text = "LMB";
                    else if (OptionsLoader.Instance.Keys[i] == KeyCode.Mouse1) _keyNames[i].text = "RMB";
                    else if (OptionsLoader.Instance.Keys[i] == KeyCode.Mouse2) _keyNames[i].text = "Mouse Wheel";
                    else _keyNames[i].text = OptionsLoader.Instance.Keys[i].ToString();
                }
        }
    }

    #endregion
}

#if UNITY_EDITOR

[CustomEditor(typeof(OptionsMenu))]
public class OptMenuEditor: Editor
{
    OptionsMenu opt;
    GUIStyle style;

    private void OnEnable()
    {
        opt = (OptionsMenu)target;
        style = new GUIStyle();
        style.normal.textColor = new Vector4(0.75f, 0.75f, 0.75f, 1);
        style.fontStyle = FontStyle.Bold;
        style.fontSize = 11;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (opt._keyNames == null || opt._keyNames.Length != Enum.GetValues(typeof(InputActions)).Length) opt._keyNames = new TextMeshProUGUI[Enum.GetValues(typeof(InputActions)).Length];
        else
        {
            EditorGUILayout.Space(13);
            EditorGUILayout.LabelField("Control Mapping", style);

            for (int i = 0; i < opt._keyNames.Length; i++)
            {
                opt._keyNames[i] = EditorGUILayout.ObjectField(((InputActions)i).ToString(), opt._keyNames[i], typeof(TextMeshProUGUI), true) as TextMeshProUGUI;
            }
        }
    }
}
#endif


