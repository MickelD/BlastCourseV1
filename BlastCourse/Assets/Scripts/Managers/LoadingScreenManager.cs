using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{


    #region Singleton

    public static LoadingScreenManager instance;

    #endregion

    [SerializeField] private GameObject g_LoadingScreen;
    [SerializeField] private Image g_bar;

    private int _currentScene;
    public int currentSceneIndex
    {
        get { return _currentScene; }
        set 
        { 
            _currentScene = value;
            if (SaveLoader.Instance != null) SaveLoader.Instance.SceneIndex = value;
        }
    }
    private List<AsyncOperation> loaders;
    private float loadersPercent;

    #region UnityFunctions

    private void Awake()
    {
        instance = this;
        currentSceneIndex = 1;

        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        loaders = new List<AsyncOperation>();
        //StartCoroutine(Test());
    }

    #endregion

    #region Methods
    public void LoadScene(int nextScene)
    {
        g_LoadingScreen.SetActive(true);
        loaders.Add(SceneManager.UnloadSceneAsync(currentSceneIndex));
        currentSceneIndex = nextScene;
        loaders.Add(SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive));

        StartCoroutine(LoadingScreen());
    }

    public IEnumerator LoadingScreen()
    {
        for(int i = 0; i < loaders.Count; i++)
        {
            

            while (!loaders[i].isDone)
            {
                loadersPercent = 0;

                foreach(AsyncOperation o in loaders)
                {
                    loadersPercent += o.progress;
                }

                loadersPercent = (loadersPercent / loaders.Count);

                g_bar.fillAmount = loadersPercent;

                yield return null;
            }
        }
        g_LoadingScreen.SetActive(false);
    }

    //public IEnumerator Test()
    //{
    //    yield return new WaitForSeconds(3);

    //    LoadScene(2);
    //}
    #endregion
}


