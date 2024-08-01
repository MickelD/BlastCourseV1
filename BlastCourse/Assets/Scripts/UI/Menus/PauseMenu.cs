using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public delegate void PauseDelegate(bool isPaused);
    public PauseDelegate OnPause;

    #endregion

    #region Variables

    private bool _opened;

    #endregion

    #region Unity Functions
    private void Start()
    {
        OpenMenu(false);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !EventManager.IsDead)
        {
            OpenMenu(!_opened);
        }
    }

    #endregion

    #region Methods

    public void OpenMenu(bool open)
    {
        _opened = open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;

        g_pauseMenu.SetActive(open);
        if (!open)
        {
            g_confirmMenu.SetActive(false);
            if(g_optionsMenu.activeSelf)Options(false);
        }
        Time.timeScale = open ? 0f : 1f;
        OnPause?.Invoke(open);
    }

    public void ContinueButton()
    {
        OpenMenu(false);
    }

    public void ExitButton(bool openConfirm)
    {
        g_confirmMenu.SetActive(openConfirm);
        g_side.SetActive(!openConfirm);
    }

    public void Restart()
    {
        SaveLoader.Instance.Load();
    }

    public void ConfirmExit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void Options(bool open)
    {
        g_optionsMenu.SetActive(true);
        g_background.SetActive(!open);
        if (open) _options.UpdateSliders();
        else _options.Back();

    }

    #endregion

    #if UNITY_EDITOR

    

    #endif
}


