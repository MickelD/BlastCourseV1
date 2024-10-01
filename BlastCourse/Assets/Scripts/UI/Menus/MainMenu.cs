using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    #region Fields

    //[Space(3), Header("Particles"), Space(5)]
    //[SerializeField] private ParticleSystem g_particles;
    //[SerializeField, Tooltip("The time the Smoke takes to go out of the way")] private float _particleMoveTime;
    //[SerializeField] float _defaultDirection;
    //[SerializeField] float _menuDirection;

    [Space(3), Header("Menus"), Space(5)]
    [SerializeField] private GameObject g_warningMenu;
    [SerializeField] private GameObject g_optionsMenu;
    [SerializeField] private OptionsMenu _options;

    [Space(3), Header("Buttons"), Space(5)]
    [SerializeField] private Button g_continue;
    [SerializeField] private TextMeshProUGUI _continueText;
    [SerializeField] private Color _disabledColor;
    [SerializeField] private AudioCue ButtonSound;

    [Space(3), Header("StartScreen"), Space(5)]
    [SerializeField] private RectTransform _title;
    [SerializeField] private TextMeshProUGUI _pressToStartText;
    [SerializeField] private GameObject g_buttons;

    [Space(3), Header("StartScreenAnimations"), Space(5)]
    [SerializeField] private AnimationCurve _titleAnimation;
    [SerializeField] private Vector2 _desiredStartPosition;
    [SerializeField] private Vector2 _desiredMenuPosition;
    [SerializeField] private float _transitionToMenuDuration;
    [SerializeField] private float _blinkSpeed;

    #endregion

    #region Vars

    //private float _currentDirection;
    //private float _desiredDirection;

    //ParticleSystem.VelocityOverLifetimeModule _particleDirection;

    //private float _particleStartTime;

    private bool _menuActive;
    private bool _toStart;
    private bool _toMenu;
    private float _startScreenTimer;
    private float _startDuration;
    private float _menuScreenTimer;
    private Vector2 _startPosition;

    private bool _isOpened;



    #endregion

    #region UnityFunctions

    private void Start()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _menuActive = false;
        _toMenu = false;
        g_warningMenu.SetActive(false);
        _startPosition = _title.anchoredPosition;
        _startDuration = _titleAnimation.keys[_titleAnimation.length - 1].time;
        g_buttons.SetActive(false);
        _toStart = true;
    }

    private void Update()
    {
        if (!_menuActive && !_toMenu && !_toStart)
        {
            _pressToStartText.color = new Color(0, 0, 0, Mathf.Sin(Time.time * _blinkSpeed) / 8 + 0.875f);
            foreach(KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(k))
                {
                    PressAnyToStart();
                }
                if (!(!_menuActive && !_toMenu && !_toStart))
                {
                    return;
                }
            }
        }
        else if (_toStart)
        {
            _startScreenTimer += Time.deltaTime;
            _title.anchoredPosition = Vector2.Lerp(_startPosition, _desiredStartPosition, _titleAnimation.Evaluate(_startScreenTimer));

            if (_startScreenTimer >= _startDuration)
            {
                _title.anchoredPosition = _desiredStartPosition;
                _toStart = false;
                _pressToStartText.gameObject.SetActive(true);
            }
        }
        else if (_toMenu)
        {
            _menuScreenTimer += Time.deltaTime;
            _title.anchoredPosition = Vector2.Lerp(_desiredStartPosition, _desiredMenuPosition, _menuScreenTimer / _transitionToMenuDuration);

            if (_menuScreenTimer >= _transitionToMenuDuration)
            {
                _title.anchoredPosition = _desiredMenuPosition;
                _toMenu = false;
                _menuActive = true;
                LoadMainMenu(true);
            }
        }
    }

    #endregion

    #region Methods

    public void PressAnyToStart()
    {
        _toMenu = true;

        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
        }

        _pressToStartText.gameObject.SetActive(false);
    }

    public void Discord()
    {
        Application.OpenURL("https://discord.gg/GeDQnjdpba");
    }

    public void LoadMainMenu(bool load)
    {
        if (load)
        {
            g_buttons.SetActive(true);

            if (!SaveSystem.DataCheck())
            {
                g_continue.interactable = false;
                _continueText.color = _disabledColor;
            }
        }
    }

    #region Buttons
    public void ContinueButton()
    {
        StartCoroutine(ContinueButtonC());
    }
    public IEnumerator ContinueButtonC()
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;

        SaveLoader.Instance.Load();
    }

    public void NewGameButton(int sceneLoad)
    {
        StartCoroutine (NewGameButtonC(sceneLoad));
    }
    public IEnumerator NewGameButtonC(int sceneLoad)
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;

        if (!SaveSystem.DataCheck())
        {
            //SaveLoader.Instance.Load();
            if (OptionsLoader.Instance != null) 
            {
                SpeedLoader.Instance.allTimer = SpeedLoader.Instance.prevTimer = 0f;
            }

            if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(sceneLoad);
            else SceneManager.LoadScene(sceneLoad);
        }
        else OpenWarningMenu();
    }

    public void ExitButton()
    {
        StartCoroutine(ExitButtonC());
    }
    public IEnumerator ExitButtonC()
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;

        Application.Quit();
    }
    public void ConfirmNewGameButton(int sceneLoad)
    {
        StartCoroutine(ConfirmNewGameButtonC(sceneLoad));
    }
    public IEnumerator ConfirmNewGameButtonC(int sceneLoad)
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;

        SaveLoader.Instance.Delete();

        if (OptionsLoader.Instance != null)
        {
            SpeedLoader.Instance.allTimer = SpeedLoader.Instance.prevTimer = 0f;
        }

        //SaveLoader.Instance.Load();
        if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(sceneLoad);
        else SceneManager.LoadScene(sceneLoad);
    }

    #endregion

    public void WarningMenu(bool open)
    {
        StartCoroutine (WarningMenuC(open));
    }
    public IEnumerator WarningMenuC(bool open)
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;

        _isOpened = open;

        g_warningMenu.SetActive(open);
    }

    public void OpenWarningMenu()
    {
        _isOpened = true;

        g_warningMenu.SetActive(true);
    }

    public void OptionsMenu(bool open)
    {
        StartCoroutine(OptionsMenuC(open));
    }
    public IEnumerator OptionsMenuC(bool open)
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;

        g_optionsMenu.SetActive(true);
        if (open) _options.UpdateSliders();
        else _options.Back();
    }

    #endregion
}


