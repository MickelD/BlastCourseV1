using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLoader : MonoBehaviour
{
    #region Singleton

    public static SpeedLoader Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        Load();
    }

    #endregion

    #region Vars

     public float allTimer;
     public float prevTimer;

    [HideInInspector] public float tutoT;
    [HideInInspector] public float wareT;
    [HideInInspector] public float cityT;
    [HideInInspector] public float labT;

    #endregion

    #region Methods

    private void Update()
    {
        allTimer += Time.deltaTime * (SaveLoader.Instance.SceneIndex > 1).GetHashCode();
        if (OptionsLoader.Instance.ExtraHUD) EventManager.OnTimeTick?.Invoke(allTimer);
    }

    public void Load()
    {
        SpeedrunData data = SaveSystem.SpeedrunLoad();

        if(data != null)
        {
            allTimer = data.SavedTime;
            prevTimer = data.previousTimer;

            tutoT = data.TutoTimer;
            wareT = data.WareTimer;
            cityT = data.CityTimer;
            labT = data.LabTimer;
        }
    }

    public void Save()
    {
        SaveSystem.SpeedrunSave(allTimer,tutoT,wareT,cityT,labT,prevTimer);
    }

    public void SetPrevTimer()
    {
        prevTimer = allTimer;
    }

    #endregion
}


