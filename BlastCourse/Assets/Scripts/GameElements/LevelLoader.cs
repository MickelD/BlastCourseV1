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

    [DrawIf(nameof(Load), LoadStyle.ByName)] public string Name;
    [DrawIf(nameof(Load), LoadStyle.ByIndex)] public int Index;
    [DrawIf(nameof(Load), LoadStyle.ByIncrement)] public int Increment;

    public Vector3 SpawnPosition;

    private bool _loadinitiated;

    private void OnTriggerEnter(Collider other)
    {
        LoadLevel();
    }

    public void LoadLevel()
    {
        if (_loadinitiated) return;
        _loadinitiated = true;
        switch (Load)
        {
            case LoadStyle.ByName:
                int i = SceneManager.GetSceneByName(Name).buildIndex;
                if (SaveLoader.Instance != null) SaveLoader.Instance.NextScene(SpawnPosition, i);
                else if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(i);
                else SceneManager.LoadScene(i);
                break;

            case LoadStyle.ByIndex:
                if (SaveLoader.Instance != null) SaveLoader.Instance.NextScene(SpawnPosition, Index);
                else if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(Index);
                else SceneManager.LoadScene(Index);
                break;

            case LoadStyle.ByIncrement:
            default:
                int j = LoadingScreenManager.instance.currentSceneIndex + Increment;
                if (SaveLoader.Instance != null) SaveLoader.Instance.NextScene(SpawnPosition, j);
                else if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(j);
                else SceneManager.LoadScene(j);
                break;
        }
    }
}


