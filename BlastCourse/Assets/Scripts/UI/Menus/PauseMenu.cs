using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenu : MonoBehaviour
{
    #region Fields

    [Space(3), Header("Components"), Space(5)]
    [SerializeField] private GameObject g_pauseMenu;
    [SerializeField] private GameObject g_confirmMenu;
    [SerializeField] private GameObject g_optionsMenu;

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
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
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
    }

    public void ContinueButton()
    {
        OpenMenu(false);
    }

    public void ExitButton()
    {
        g_confirmMenu.SetActive(true);
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
        if (open) g_optionsMenu.GetComponent<OptionsMenu>().UpdateSliders();
        else g_optionsMenu.GetComponent<OptionsMenu>().Back();
    }

    #endregion

    #if UNITY_EDITOR

    

    #endif
}


