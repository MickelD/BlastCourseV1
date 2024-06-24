using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoader : MonoBehaviour
{
    #region Variables

    public static SaveLoader Instance;

    public bool startWithAllUnlocks;

    [HideInInspector] public int SceneIndex;
    [HideInInspector] public float[] SpawnPos;
    [HideInInspector] public List<string> CollectiblesFound;
    [HideInInspector] public List<string> KeysReached;
    [HideInInspector] public bool[] UnlockedRpgs;
    [HideInInspector] public List<string> Boxes;
    [HideInInspector] public List<float> BoxesX;
    [HideInInspector] public List<float> BoxesY;
    [HideInInspector] public List<float> BoxesZ;
    [HideInInspector] public List<string> UsedBoxes;
    public int DefaultPlaySceneIndex;

    [HideInInspector] public bool _loading;

    #endregion

    #region Unity Functions

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Methods

    public void SetSpawn(Vector3 spawn)
    {
        SpawnPos = new float[3];

        SpawnPos[0] = spawn.x;
        SpawnPos[1] = spawn.y;
        SpawnPos[2] = spawn.z;
        if (LoadingScreenManager.instance != null) SceneIndex = LoadingScreenManager.instance.currentSceneIndex;
        else SceneIndex = SceneManager.GetActiveScene().buildIndex;

        Save();
    }
    public Vector3 GetSpawn()
    {
        Vector3 spawn = new Vector3(SpawnPos[0], SpawnPos[1], SpawnPos[2]);

        return spawn;
    }

    public Vector3 GetBoxPos(int i)
    {
        return new Vector3(BoxesX[i], BoxesY[i], BoxesZ[i]);
    }
    public void SetBoxPos(UraniumBox b)
    {
        if (Boxes.Contains(b.GetIndex()))
        {
            for(int i = 0; i < Boxes.Count; i++)
            {
                if(Boxes[i] == b.GetIndex())
                {
                    Boxes[i] = (b.GetIndex());
                    BoxesX[i] = (b.transform.position.x);
                    BoxesY[i] = (b.transform.position.y);
                    BoxesZ[i] = (b.transform.position.z);
                }
            }
        }
        else
        {
            Boxes.Add(b.GetIndex());
            BoxesX.Add(b.transform.position.x);
            BoxesY.Add(b.transform.position.y);
            BoxesZ.Add(b.transform.position.z);
        }
        Save();
    }

    public int GetScene() { return SceneIndex; }

    public void NextScene(Vector3 spawnPosition, int sceneIndex)
    {
        SetSpawn(spawnPosition);
        SceneIndex = sceneIndex;
        Save();
        Load();
    }


    [ContextMenu("Save")]
    public void Save()
    {
        EventManager.OnSaveGame?.Invoke();
        SaveSystem.DataSave(SceneIndex, SpawnPos, CollectiblesFound, KeysReached, UnlockedRpgs, Boxes, BoxesX, BoxesY, BoxesZ, UsedBoxes);
    }
    [ContextMenu("Load")]
    public void Load()
    {
        _loading = true;

        //Get Data
        SaveData data = SaveSystem.DataLoad();

        if(data != null)
        {
            SceneIndex = data._scene;

            SpawnPos = new float[3];
            if (data._spawnPosition.Length > 0)
                for (int i = 0; i < data._spawnPosition.Length; i++)
                    SpawnPos[i] = data._spawnPosition[i];

            CollectiblesFound = new List<string>();
            if (data._collectiblesAquired.Count > 0)
                for (int i = 0; i < data._collectiblesAquired.Count; i++)
                    CollectiblesFound.Add(data._collectiblesAquired[i]);

            KeysReached = new List<string>();
            if (data._keyObjects.Count > 0)
                for (int i = 0; i < data._keyObjects.Count; i++)
                    KeysReached.Add(data._keyObjects[i]);

            UnlockedRpgs = new bool[4];
            if(data._rpgs.Length > 0)
                for (int i = 0; i < data._rpgs.Length; i++)
                    UnlockedRpgs[i] = data._rpgs[i];

            Boxes = new List<string>();
            if (data.Boxes.Count > 0)
                for (int i = 0; i < data.Boxes.Count; i++)
                    Boxes.Add(data.Boxes[i]);

            BoxesX = new List<float>();
            if (data.BoxesX.Count > 0)
                for (int i = 0; i < data.BoxesX.Count; i++)
                    BoxesX.Add(data.BoxesX[i]);

            BoxesY = new List<float>();
            if (data.BoxesY.Count > 0)
                for (int i = 0; i < data.BoxesY.Count; i++)
                    BoxesY.Add(data.BoxesY[i]);

            BoxesZ = new List<float>();
            if (data.BoxesZ.Count > 0)
                for (int i = 0; i < data.BoxesZ.Count; i++)
                    BoxesZ.Add(data.BoxesZ[i]);

            UsedBoxes = new List<string>();
            if (data.UsedBoxes.Count > 0)
                for (int i = 0; i < data.UsedBoxes.Count; i++)
                    UsedBoxes.Add(data.UsedBoxes[i]);

            if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(SceneIndex);
            else SceneManager.LoadScene(SceneIndex);
        }
        else
        {
            Debug.Log("No Data Found");

            SceneIndex = DefaultPlaySceneIndex;
            SpawnPos = new float[0];
            CollectiblesFound = new List<string>();
            KeysReached = new List<string>();
            UnlockedRpgs = new bool[4];
            Boxes = new List<string>();
            BoxesX = new List<float>();
            BoxesY = new List<float>();
            BoxesZ = new List<float>();
            UsedBoxes = new List<string>();

            if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(SceneIndex);
            else SceneManager.LoadScene(SceneIndex);
        }
    }
    [ContextMenu("Delete")]
    public void Delete() 
    {
        SceneIndex = 0;
        SpawnPos = null;
        CollectiblesFound = new List<string>();
        KeysReached = new List<string>();
        UnlockedRpgs = null;
        Boxes = new List<string>();
        BoxesX = new List<float>();
        BoxesY = new List<float>();
        BoxesZ = new List<float>();
        UsedBoxes = new List<string>();


        SaveSystem.DataDelete(); 
    }

    #endregion
}
