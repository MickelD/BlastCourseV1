using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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

    public float tutoT = float.MaxValue/2;
    public float wareT = float.MaxValue/2;
    public float cityT = float.MaxValue/2;
    public float labT = float.MaxValue/2;
    public float tutoTimer;
    public float wareTimer;
    public float cityTimer;
    public float labTimer;

    #endregion

    #region Methods

    private void Update()
    {
        if(!SaveLoader.Instance._levelSelect)allTimer += Time.deltaTime * (SaveLoader.Instance.SceneIndex > 1).GetHashCode();
        if (OptionsLoader.Instance.ExtraHUD)
        {
            if (SaveLoader.Instance._levelSelect)
            {
                switch (SaveLoader.Instance.SceneIndex)
                {
                    case 2:
                        EventManager.OnTimeTick?.Invoke(tutoTimer); break;
                    case 3:
                        EventManager.OnTimeTick?.Invoke(wareTimer); break;
                    case 4:
                        EventManager.OnTimeTick?.Invoke(cityTimer); break;
                    case 5:
                        EventManager.OnTimeTick?.Invoke(labTimer); break;
                }
            }
            else EventManager.OnTimeTick?.Invoke(allTimer);
        }

        switch (SaveLoader.Instance.SceneIndex)
        {
            case 2:
                tutoTimer += Time.deltaTime;
                break;
            case 3:
                wareTimer += Time.deltaTime;
                break;
            case 4:
                cityTimer += Time.deltaTime;
                break;
            case 5:
                labTimer += Time.deltaTime;
                break;
        }
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
        else
        {
            allTimer = 0;
            prevTimer = 0;

            tutoT = float.MaxValue /2;
            wareT = float.MaxValue /2;
            cityT = float.MaxValue /2;
            labT = float.MaxValue /2;
        }
    }

    public void Save()
    {
        SaveSystem.SpeedrunSave(allTimer,
            tutoT,
            wareT,
            cityT,
            labT,
            prevTimer);
    }
    public void SaveTuto()
    {
        tutoT = tutoTimer < tutoT && tutoTimer > 0 ? tutoTimer : tutoT;
        Save();
    }
    public void SaveWare()
    {
        wareT = wareTimer < wareT && wareTimer > 0 ? wareTimer : wareT;
        Save();
    }
    public void SaveCity()
    {
        cityT = cityTimer < cityT && cityTimer > 0 ? cityTimer : cityT;
        Save(); 
    }
    public void SaveLab()
    {
        labT = labTimer < labT && labTimer > 0 ? labTimer : labT;
        Save();
    }

    public void SetPrevTimer()
    {
        prevTimer = allTimer;
    }

    public void ResetLevelTimers()
    {
        tutoTimer = 0;
        wareTimer = 0;
        cityTimer = 0;
        labTimer = 0;
    }

    public float GetPrevTime()
    {
        if (SaveLoader.Instance._levelSelect)
        {
            switch (SaveLoader.Instance.SceneIndex)
            {
                case 2:
                    return tutoT;
                case 3:
                    return wareT;
                case 4:
                    return cityT;
                case 5:
                    return labT;
            }
        }

        return prevTimer;
    }

    #endregion
}


