using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public enum LoadStyle
    {
        ByName,
        ByIndex, 
        ByIncrement
    }

    public LoadStyle Load;
    public bool lastLevel;

    [DrawIf(nameof(Load), LoadStyle.ByName)] public string Name;
    [DrawIf(nameof(Load), LoadStyle.ByIndex)] public int Index;
    [DrawIf(nameof(Load), LoadStyle.ByIncrement)] public int Increment;

    public Vector3 SpawnPosition;
    [SerializeField] float _rot;
    private bool _loadinitiated;

    private void OnTriggerEnter(Collider other)
    {
        if (SteamIntegrator.Instance != null) SteamIntegrator.Instance.BeatLevel();

        switch (SaveLoader.Instance.SceneIndex)
        {
            case 2:
                if (!SaveLoader.Instance._levelSelect) SpeedLoader.Instance.tutoTimer = SpeedLoader.Instance.allTimer - SpeedLoader.Instance.prevTimer;
                else if (SaveLoader.Instance._continuedPlay) SpeedLoader.Instance.tutoTimer = SpeedLoader.Instance.continuedPlayTimer - SpeedLoader.Instance.prevContinuedPlayTimer;
                SpeedLoader.Instance.SaveTuto();
                break;
            case 3:
                if (!SaveLoader.Instance._levelSelect) SpeedLoader.Instance.wareTimer = SpeedLoader.Instance.allTimer - SpeedLoader.Instance.prevTimer;
                else if (SaveLoader.Instance._continuedPlay) SpeedLoader.Instance.wareTimer = SpeedLoader.Instance.continuedPlayTimer - SpeedLoader.Instance.prevContinuedPlayTimer;
                SpeedLoader.Instance.SaveWare();
                break;
            case 4:
                if (!SaveLoader.Instance._levelSelect) SpeedLoader.Instance.cityTimer = SpeedLoader.Instance.allTimer - SpeedLoader.Instance.prevTimer;
                else if (SaveLoader.Instance._continuedPlay) SpeedLoader.Instance.cityTimer = SpeedLoader.Instance.continuedPlayTimer - SpeedLoader.Instance.prevContinuedPlayTimer;
                SpeedLoader.Instance.SaveCity();
                break;
            case 5:
                if (!SaveLoader.Instance._levelSelect) SpeedLoader.Instance.labTimer = SpeedLoader.Instance.allTimer - SpeedLoader.Instance.prevTimer;
                else if (SaveLoader.Instance._continuedPlay) SpeedLoader.Instance.labTimer = SpeedLoader.Instance.continuedPlayTimer - SpeedLoader.Instance.prevContinuedPlayTimer;
                SpeedLoader.Instance.SaveLab();
                SaveLoader.Instance.CompletedLevels[3] = true;
                if(!SaveLoader.Instance._speedrunMode) SaveLoader.Instance.Save();
                break;
        }

        if (SaveLoader.Instance._levelSelect && !SaveLoader.Instance._continuedPlay)
        {
            if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(1);
            else SceneManager.LoadScene(1);
        }
        else if (!lastLevel)
        {
            LoadLevel();
        }
        
    }

    public void LoadLevel()
    {
        SpeedLoader.Instance.SetPrevTimer();

        if (_loadinitiated) return;
        _loadinitiated = true;
        switch (Load)
        {
            case LoadStyle.ByName:
                int i = SceneManager.GetSceneByName(Name).buildIndex;
                if(!SaveLoader.Instance._levelSelect || SaveLoader.Instance.CompletedLevels[i - 2])
                {
                    if (SaveLoader.Instance != null) SaveLoader.Instance.NextScene(SpawnPosition, i, _rot);
                    else if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(i);
                    else SceneManager.LoadScene(i);
                }
                else
                {
                    if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(1);
                    else SceneManager.LoadScene(1);
                }
                break;

            case LoadStyle.ByIndex:
                if (!SaveLoader.Instance._levelSelect || SaveLoader.Instance.CompletedLevels[Index - 2])
                {
                    if (SaveLoader.Instance != null) SaveLoader.Instance.NextScene(SpawnPosition, Index, _rot);
                    else if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(Index);
                    else SceneManager.LoadScene(Index);
                }
                else
                {
                    if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(1);
                    else SceneManager.LoadScene(1);
                }
                break;

            case LoadStyle.ByIncrement:
            default:
                int j = 0;
                if (LoadingScreenManager.instance != null) j = LoadingScreenManager.instance.currentSceneIndex + Increment;
                else j = SceneManager.GetActiveScene().buildIndex + Increment;

                if (!SaveLoader.Instance._levelSelect || SaveLoader.Instance.CompletedLevels[j - 2])
                {
                    if (SaveLoader.Instance != null) SaveLoader.Instance.NextScene(SpawnPosition, j, _rot);
                    else if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(j);
                    else SceneManager.LoadScene(j);
                }
                else
                {
                    if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(1);
                    else SceneManager.LoadScene(1);
                }
                break;
        }
    }
}


