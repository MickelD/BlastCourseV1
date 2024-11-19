using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SteamIntegrator : MonoBehaviour
{
    public static SteamIntegrator Instance;
    public AchievementData _AchievementData;
    public int _levelIndex;
    private bool _connected;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        SteamAPI.Init();
    }

    private void Start()
    {
        //try
        //{
        //    SteamClient.Init(2655220);
        //    _connected = true;
        //}
        //catch
        //{
        //    Debug.Log("Steam Client not found, achievements disabled");
        //    _connected = false;
        //}

        _connected = SteamManager.Initialized;


        if(!SaveSystem.AchievementsCheck()) SaveSystem.AchievementsSave(true, true, true, true);

        _AchievementData = SaveSystem.AchievementsLoad();
    }

    public void LevelLoad(int index)
    {
        _levelIndex = index;

        switch (_levelIndex)
        {
            case 0: //RECEPTION
                if (!_AchievementData.canColYellow && IsFirstCheckpoint(0)) //reset banana achievement
                    AllowAchievement(AchStatus.canColYellow);

                gameObject.name = IsFirstCheckpoint(0) ? "tutoFirst" : "tuto";

                break;
            case 1: //WAREHOUSE
                if (!_AchievementData.canColGreen && IsFirstCheckpoint(1)) //reset avocado achievement
                    AllowAchievement(AchStatus.canColGreen);

                gameObject.name = IsFirstCheckpoint(1) ? "wareFirst" : "ware";

                break;
            case 2: //SULFUR VALLEY
                if (!_AchievementData.canColBlue && IsFirstCheckpoint(2)) //reset blueberry achievement
                    AllowAchievement(AchStatus.canColBlue);

                gameObject.name = IsFirstCheckpoint(2) ? "cityFirst" : "city";

                break;
            case 3:
                if (!_AchievementData.canBeatNodeaths && IsFirstCheckpoint(3)) //reset no deaths achievement
                    AllowAchievement(AchStatus.canBeatNodeaths);

                gameObject.name = IsFirstCheckpoint(3) ? "labFirst" : "lab";

                break;
            default:
                break;
        }
    }


    public void AllowAchievement(AchStatus id)
    {
        switch (id)
        {
            case AchStatus.canBeatNodeaths:
                _AchievementData.canBeatNodeaths = true;
                break;
            case AchStatus.canColYellow:
                _AchievementData.canColYellow = true;
                break;
            case AchStatus.canColGreen:
                _AchievementData.canColGreen = true;
                break;
            case AchStatus.canColBlue:
                _AchievementData.canColBlue = true;
                break;
            default:
                break;
        }

        SaveSystem.AchievementsSave(_AchievementData);
    }

    public void DisallowAchievement(AchStatus id)
    {
        switch (id)
        {
            case AchStatus.canBeatNodeaths:
                _AchievementData.canBeatNodeaths = false;
                break;
            case AchStatus.canColYellow:
                _AchievementData.canColYellow = false;
                break;
            case AchStatus.canColGreen:
                _AchievementData.canColGreen = false;
                break;
            case AchStatus.canColBlue:
                _AchievementData.canColBlue = false;
                break;
            default:
                break;
        }

        SaveSystem.AchievementsSave(_AchievementData);
    }

    private bool IsFirstCheckpoint(int index)
    {
        if(SaveLoader.Instance != null)
        {
            Vector3 startingPos = index switch
            {
                0 => new Vector3(-3f, 0.5f, -12f),
                1 => new Vector3(-15f, 0f, -4f),
                2 => new Vector3(-64f, -5f, -54f),
                3 => new Vector3(-21f, -13.5f, 11.5f),
                _ => Vector3.zero
            };

            return Vector3.Distance(SaveLoader.Instance.GetSpawn(), startingPos) <= 1f;
        }
        else return false;
    }

    public void BeatGame()
    {
        if (SaveLoader.Instance != null)
        {
            if (!SaveLoader.Instance._levelSelect) // beat game achievements
            {
                UnlockAchievement("achBeatGame");

                //NO DEATHS
                if (_AchievementData.canBeatNodeaths) UnlockAchievement("achBeatNodeaths");

                //TIME
                if (SpeedLoader.Instance != null && SpeedLoader.Instance.allTimer <= 1200f) UnlockAchievement("achBeatFast");
            }
        }
    }

    public void BeatLevel()
    {
        if (_levelIndex <= 0)
        {
            if (_AchievementData.canColYellow) UnlockAchievement("achColYellow");
        }
        else if (_levelIndex == 1)
        {
            if (_AchievementData.canColGreen) UnlockAchievement("achColGreen");
        }
        else if (_levelIndex == 2)
        {
            if (_AchievementData.canColBlue) UnlockAchievement("achColBlue");
        }
        else if (_levelIndex == 3)
        {
            BeatGame();
        }
    }


    public void FireRocket(FiringMode fireMode)
    {
        //Firing a normal rocket on the Reception blocks the Banana achievement
        if (_AchievementData.canColYellow && _levelIndex == 0 && fireMode == FiringMode.Classic)
            DisallowAchievement(AchStatus.canColYellow);

        //Firing anything that is not a remote on the Warehouse blocks the Avocado achievement
        if (_AchievementData.canColGreen && _levelIndex == 1 && fireMode != FiringMode.Remote)
            DisallowAchievement(AchStatus.canColGreen);

        //Firing anything that is not a grenade on Sulfur Valley blocks the Blueberry achievement
        if (_AchievementData.canColBlue && _levelIndex == 2 && fireMode != FiringMode.Pipe)
            DisallowAchievement(AchStatus.canColBlue);
    }


    private void OnApplicationQuit()
    {
        SteamAPI.Shutdown();
    }

    public void UnlockAchievement(string id)
    {
        if (!_connected) return;

        SteamUserStats.SetAchievement(id);
        SteamUserStats.StoreStats();
    }
    public void ClearAchievement(string id)
    {
        if (!_connected) return;

        SteamUserStats.ClearAchievement(id);
        SteamUserStats.StoreStats();

        //var ach = new Steamworks.Data.Achievement(id);
        //if (ach.State == true) ach.Clear();
    }

    [ContextMenu("Reset")]
    public void ResetStatus()
    {
        SaveSystem.AchievementsDelete();
    }

    [ContextMenu("Clear")]
    public void ClearAll()
    {
        if (!_connected) return;

        SteamUserStats.ResetAllStats(true);
        SteamUserStats.StoreStats();
    }
}


