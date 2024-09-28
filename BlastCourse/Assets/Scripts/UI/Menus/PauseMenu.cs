using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenu : MonoBehaviour
{
    #region Singleton Framework
    public static PauseMenu Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    #region Fields

    [Space(3), Header("Components"), Space(5)]
    [SerializeField] private GameObject g_pauseMenu;
    [SerializeField] private GameObject g_confirmMenu;
    [SerializeField] private GameObject g_optionsMenu;
    [SerializeField] private OptionsMenu _options;

    [Space(3), Header("Visuals"), Space(5)]
    [SerializeField] private GameObject g_background;
    [SerializeField] private GameObject g_side;

    [Space(5), Header("OnlyClick Buttons"), Space(2)]
    [SerializeField] EventSystem _eventSystem;

    public delegate void PauseDelegate(bool isPaused);
    public PauseDelegate OnPause;

    [SerializeField] private AudioCue ButtonSound;

    #endregion

    #region Variables

    private bool _opened;

    #endregion

    #region Unity Functions
    private void Start()
    {
        OpenMenuNoSFX(false);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !EventManager.IsDead)
        {
            StartCoroutine(OpenMenu(!_opened));
        }
        if (_eventSystem.currentSelectedGameObject != null) _eventSystem.SetSelectedGameObject(null);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    #endregion

    #region Methods

    public IEnumerator OpenMenu(bool open)
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;

        bool close = true;

        if (!open)
        {
            g_confirmMenu.SetActive(false);
            if (g_optionsMenu.activeSelf) close = OptionsClose();
        }

        if(close || open)
        {
            _opened = open;
            Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = open;

            g_pauseMenu.SetActive(open);

            Time.timeScale = open ? 0f : 1f;
            OnPause?.Invoke(open);
        }
        
    }

    public void OpenMenuNoSFX(bool open)
    {
        _opened = open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;

        g_pauseMenu.SetActive(open);
        if (!open)
        {
            g_confirmMenu.SetActive(false);
            if (g_optionsMenu.activeSelf) OptionsClose();
        }
        Time.timeScale = open ? 0f : 1f;
        OnPause?.Invoke(open);
    }

    public void ContinueButton()
    {
        StartCoroutine(OpenMenu(false));
    }

    public void ExitButton(bool openConfirm) { StartCoroutine(ExitButtonC(openConfirm)); }
    public IEnumerator ExitButtonC(bool openConfirm)
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;

        g_confirmMenu.SetActive(openConfirm);
        g_side.SetActive(!openConfirm);
    }

    public void Restart() { StartCoroutine(RestartC()); }
    public IEnumerator RestartC()
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;

        SaveLoader.Instance.Load();
    }

    public void ConfirmExit() { StartCoroutine(ConfirmExitC()); }
    public IEnumerator ConfirmExitC()
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;

        Time.timeScale = 1f;
        if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(1);
        else SceneManager.LoadScene(1);
    }

    public void Options(bool open) { StartCoroutine(OptionsC(open)); }
    public IEnumerator OptionsC(bool open)
    {
        if (ButtonSound.SfxClip != null && ButtonSound.SfxClip.Length > 0)
        {
            AudioSource source = AudioManager.TryPlayCueAtPoint(ButtonSound, Vector3.zero);
            yield return new WaitForSecondsRealtime(source.clip.length);
        }
        else yield return null;


        g_optionsMenu.SetActive(true);
        if (open) _options.UpdateSliders();
        else _options.MenuBack();
        g_background.SetActive(!open);

    }

    public bool OptionsClose()
    {
        bool close = _options.MenuBack();
        if(close) g_background.SetActive(true);
        return close;

    }

    #endregion

#if UNITY_EDITOR



#endif
}


