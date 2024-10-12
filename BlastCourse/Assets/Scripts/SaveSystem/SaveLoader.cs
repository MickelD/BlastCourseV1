using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveLoader : MonoBehaviour
{
    #region Variables

    public static SaveLoader Instance;

    public bool startWithAllUnlocks;

    public int SceneIndex;
    public float[] SpawnPos;
    [HideInInspector] public List<string> CollectiblesFound;
    [HideInInspector] public List<string> KeysReached;
    [HideInInspector] public bool[] UnlockedRpgs;
    [HideInInspector] public List<string> Boxes;
    [HideInInspector] public List<float> BoxesX;
    [HideInInspector] public List<float> BoxesY;
    [HideInInspector] public List<float> BoxesZ;
    [HideInInspector] public List<string> UsedBoxes;
    [HideInInspector] public List<string> DialoguesIds;
    [HideInInspector] public List<int> DialoguesCount;
    [HideInInspector] public bool[] CompletedLevels;
    public int DefaultPlaySceneIndex;

    [HideInInspector] public bool _loading;

    [HideInInspector] public bool _levelSelect = false;
    [HideInInspector] public bool _speedrunMode = false;

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

    //Spawn
    public void SetSpawn(Vector3 spawn, float rot = 0f)
    {
        SpawnPos = new float[4];
        SpawnPos[0] = spawn.x;
        SpawnPos[1] = spawn.y;
        SpawnPos[2] = spawn.z;
        SpawnPos[3] = rot;
        if (LoadingScreenManager.instance != null) SceneIndex = LoadingScreenManager.instance.currentSceneIndex;
        else SceneIndex = SceneManager.GetActiveScene().buildIndex;

        Save();
    }
    public Vector3 GetSpawn()
    {
        Vector3 spawn = new Vector3(SpawnPos[0], SpawnPos[1], SpawnPos[2]);

        return spawn;
    }


    //Boxes
    public Vector3 GetBoxPos(int i)
    {
        return new Vector3(BoxesX[i], BoxesY[i], BoxesZ[i]);
    }
    public bool SetBoxPos(UraniumBox b, Vector3 pos)
    {
        if (Boxes.Contains(b.GetIndex()))
        {
            for (int i = 0; i < Boxes.Count; i++)
            {
                if(Boxes[i] == b.GetIndex())
                {
                    if (Vector3.Distance(pos, new Vector3(BoxesX[i], BoxesY[i], BoxesZ[i])) <= 0.125f)
                        return false;

                    Boxes[i] = (b.GetIndex());
                    BoxesX[i] = (pos.x);
                    BoxesY[i] = (pos.y);
                    BoxesZ[i] = (pos.z);
                    Save();
                    return true;
                }
            }
            return false;
        }
        else
        {
            Boxes.Add(b.GetIndex());
            BoxesX.Add(pos.x);
            BoxesY.Add(pos.y);
            BoxesZ.Add(pos.z);
            Save();
            return true;
        }
    }


    //Scenes
    public int GetScene() { return SceneIndex; }
    public void NextScene(Vector3 spawnPosition, int sceneIndex, float rot = 0f)
    {
        SetSpawn(spawnPosition, rot);
        if (CompletedLevels == null || CompletedLevels.Length != 4) CompletedLevels = new bool[4];
        CompletedLevels[SceneIndex-2] = true;
        SceneIndex = sceneIndex;
        Save();
        Load();
    }


    //Dialogues
    public int GetDialogueCount(string id)
    {
        if (DialoguesIds != null && DialoguesCount != null
            && DialoguesIds.Contains(id)) 
            for (int i = 0; i < DialoguesIds.Count; i++)
            {
                if (DialoguesIds[i] == id) return DialoguesCount[i];
            }
        return -1;
    }
    public void SetDialogueCount(string id, int count)
    {
        if (DialoguesIds.Contains(id))
            for (int i = 0; i < DialoguesIds.Count; i++)
            {
                if (DialoguesIds[i] == id) DialoguesCount[i] = count;
            }
        else
        {
            DialoguesIds.Add(id);
            DialoguesCount.Add(count);
        }
        if (DialoguesCount.Count != DialoguesIds.Count) Debug.LogWarning("Dialogues IDs and Counters are not equal.");
    }

    [ContextMenu("Save")]
    public void Save()
    {
        EventManager.OnSaveGame?.Invoke();
        if (!_levelSelect) SaveSystem.DataSave(SceneIndex, SpawnPos, CollectiblesFound, KeysReached, UnlockedRpgs, Boxes, BoxesX, BoxesY, BoxesZ, UsedBoxes, DialoguesIds, DialoguesCount, CompletedLevels);
        else if (!_speedrunMode) SaveSystem.LevelDataSave(SceneIndex, SpawnPos, CollectiblesFound, KeysReached, UnlockedRpgs, Boxes, BoxesX, BoxesY, BoxesZ, UsedBoxes, DialoguesIds, DialoguesCount, CompletedLevels);
        if (!_speedrunMode) SpeedLoader.Instance.Save();
    }
    [ContextMenu("Load")]
    public void Load()
    {
        _loading = true;

        //Get Data
        SaveData data = null;
        if (!_levelSelect) data = SaveSystem.DataLoad();
        else data = SaveSystem.LevelDataLoad();

        if (data != null)
        {
            SceneIndex = data._scene;

            SpawnPos = new float[4];
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

            DialoguesIds = new List<string>();
            if (data.DialoguesIds.Count > 0)
                for (int i = 0; i < data.DialoguesIds.Count; i++)
                    DialoguesIds.Add(data.DialoguesIds[i]);

            DialoguesCount = new List<int>();
            if (data.DialoguesCount.Count > 0)
                for (int i = 0; i < data.DialoguesCount.Count; i++)
                    DialoguesCount.Add(data.DialoguesCount[i]);

            CompletedLevels = new bool[4];
            if(data.CompletedLevels.Length > 0)
                for(int i = 0; i < data.CompletedLevels.Length; i++)
                    CompletedLevels[i] = data.CompletedLevels[i];

            if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(SceneIndex);
            else SceneManager.LoadScene(SceneIndex);
        }
        else
        {
            CreateEmptySave();

            if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(SceneIndex);
            else SceneManager.LoadScene(SceneIndex);
        }

        if (SpeedLoader.Instance != null) SpeedLoader.Instance.Load();
    }
    public void LoadDataWithoutSceneChange()
    {
        SaveData data = SaveSystem.DataLoad();
        if (data != null)
        {
            SceneIndex = 1;

            SpawnPos = new float[4];
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
            if (data._rpgs.Length > 0)
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

            DialoguesIds = new List<string>();
            if (data.DialoguesIds.Count > 0)
                for (int i = 0; i < data.DialoguesIds.Count; i++)
                    DialoguesIds.Add(data.DialoguesIds[i]);

            DialoguesCount = new List<int>();
            if (data.DialoguesCount.Count > 0)
                for (int i = 0; i < data.DialoguesCount.Count; i++)
                    DialoguesCount.Add(data.DialoguesCount[i]);

            CompletedLevels = new bool[4];
            if (data.CompletedLevels.Length > 0)
                for (int i = 0; i < data.CompletedLevels.Length; i++)
                    CompletedLevels[i] = data.CompletedLevels[i];
        }
        else
        {
            CreateEmptySave();
        }
    }
    public void CreateEmptySave()
    {
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
        DialoguesIds = new List<string>();
        DialoguesCount = new List<int>();
        CompletedLevels = new bool[4];
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
        DialoguesIds = new List<string>();
        DialoguesCount = new List<int>();


        SaveSystem.DataDelete(); 
    }

    public void DeleteLevelData()
    {
        SaveSystem.LevelDataDelete();
    }

    #endregion
}
