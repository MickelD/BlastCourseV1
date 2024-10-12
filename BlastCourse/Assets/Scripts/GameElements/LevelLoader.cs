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
        switch (SaveLoader.Instance.SceneIndex)
        {
            case 2:
                if (!SaveLoader.Instance._levelSelect) SpeedLoader.Instance.tutoTimer = SpeedLoader.Instance.allTimer - SpeedLoader.Instance.prevTimer;
                if (SteamIntegrator.Instance != null) SteamIntegrator.Instance.BeatLevel(AchStatus.canColYellow);
                SpeedLoader.Instance.SaveTuto();
                break;
            case 3:
                if (!SaveLoader.Instance._levelSelect) SpeedLoader.Instance.wareTimer = SpeedLoader.Instance.allTimer - SpeedLoader.Instance.prevTimer;
                if (SteamIntegrator.Instance != null) SteamIntegrator.Instance.BeatLevel(AchStatus.canColGreen);
                SpeedLoader.Instance.SaveWare();
                break;
            case 4:
                if (!SaveLoader.Instance._levelSelect) SpeedLoader.Instance.cityTimer = SpeedLoader.Instance.allTimer - SpeedLoader.Instance.prevTimer;
                if (SteamIntegrator.Instance != null) SteamIntegrator.Instance.BeatLevel(AchStatus.canColBlue);
                SpeedLoader.Instance.SaveCity();
                break;
            case 5:
                if (!SaveLoader.Instance._levelSelect) SpeedLoader.Instance.labTimer = SpeedLoader.Instance.allTimer - SpeedLoader.Instance.prevTimer;
                if (SteamIntegrator.Instance != null) SteamIntegrator.Instance.BeatGame();
                SpeedLoader.Instance.SaveLab();
                SaveLoader.Instance.CompletedLevels[3] = true;
                SaveLoader.Instance.Save();
                break;
        }

        if (SaveLoader.Instance._levelSelect)
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
                if (SaveLoader.Instance != null) SaveLoader.Instance.NextScene(SpawnPosition, i, _rot);
                else if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(i);
                else SceneManager.LoadScene(i);
                break;

            case LoadStyle.ByIndex:
                if (SaveLoader.Instance != null) SaveLoader.Instance.NextScene(SpawnPosition, Index, _rot);
                else if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(Index);
                else SceneManager.LoadScene(Index);
                break;

            case LoadStyle.ByIncrement:
            default:
                int j = 0;
                if (LoadingScreenManager.instance != null) j = LoadingScreenManager.instance.currentSceneIndex + Increment;
                else j = SceneManager.GetActiveScene().buildIndex + 1;
                if (SaveLoader.Instance != null) SaveLoader.Instance.NextScene(SpawnPosition, j, _rot);
                else if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(j);
                else SceneManager.LoadScene(j);
                break;
        }
    }
}


