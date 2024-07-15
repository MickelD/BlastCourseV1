using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
    #region Fields

    [Space(3), Header("Particles"), Space(5)]
    [SerializeField] private ParticleSystem g_particles;
    [SerializeField, Tooltip("The time the Smoke takes to go out of the way")] private float _particleMoveTime;
    [SerializeField] float _defaultDirection;
    [SerializeField] float _menuDirection;

    [Space(3), Header("Menus"), Space(5)]
    [SerializeField] private GameObject g_warningMenu;
    [SerializeField] private GameObject g_optionsMenu;
    [SerializeField] private OptionsMenu _options;

    [Space(3), Header("Buttons"), Space(5)]
    [SerializeField] private RectTransform g_continue;
    [SerializeField] private RectTransform g_newGame;

    #endregion

    #region Vars

    private float _currentDirection;
    private float _desiredDirection;

    ParticleSystem.VelocityOverLifetimeModule _particleDirection;

    private float _particleStartTime;

    private bool _isOpened;

    #endregion

    #region UnityFunctions

    private void Start()
    {
        Time.timeScale = 1f;

        _currentDirection = _isOpened ? _menuDirection : _defaultDirection;
        if(g_particles != null) _particleDirection = g_particles.velocityOverLifetime;
        _particleDirection.x = _currentDirection;
        g_warningMenu.SetActive(_isOpened);

        if (!SaveSystem.DataCheck())
        {
            Debug.Log("No Save");

            g_continue.gameObject.SetActive(false);
            g_newGame.localPosition -= Vector3.right * 200;
            g_newGame.sizeDelta = new Vector2(400, g_newGame.sizeDelta.y);
        }
    }

    private void Update()
    {
        if(_currentDirection != _desiredDirection)
        {
            _currentDirection = Mathf.Clamp(_currentDirection, _desiredDirection, (Time.time - _particleStartTime) / _particleMoveTime);

            _particleDirection.x = _currentDirection;
        }
    }

    #endregion

    #region Methods

    #region Buttons
    public void ContinueButton()
    {
        SaveLoader.Instance.Load();
    }
    public void NewGameButton(int sceneLoad)
    {
        if (!SaveSystem.DataCheck())
        {
            //SaveLoader.Instance.Load();
            if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(sceneLoad);
            else SceneManager.LoadScene(sceneLoad);
        }
        else WarningMenu(true);
    }
    public void ExitButton()
    {
        Application.Quit();
    }
    public void ConfirmNewGameButton(int sceneLoad)
    {
        SaveLoader.Instance.Delete();
        //SaveLoader.Instance.Load();
        if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(sceneLoad);
        else SceneManager.LoadScene(sceneLoad);
    }
    #endregion

    public void WarningMenu(bool open)
    {
        _isOpened = open;
        _desiredDirection = open ? _menuDirection : _defaultDirection;
        _particleStartTime = Time.time;

        g_warningMenu.SetActive(open);
    }

    public void OptionsMenu(bool open)
    {
        _desiredDirection = open ? _menuDirection : _defaultDirection;
        _particleStartTime = Time.time;

        g_optionsMenu.SetActive(true);
        if (open) _options.UpdateSliders();
        else _options.Back();
    }

    #endregion
}


