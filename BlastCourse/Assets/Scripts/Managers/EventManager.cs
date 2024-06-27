using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CustomMethods;

public static class EventManager
{
    #region Events

    //Player Velocity Events
    public static Action<float> OnUpdatePlayerSpeedXYZ;
    public static Action<float> OnUpdatePlayerSpeedXZ;
    public static Action<float> OnUpdatePlayerSpeedY;
    public static Action<float> OnUpdatePlayerSpeedX;
    public static Action<Vector3> OnUpdatePlayerVelocity;
    public static Action<float> OnPlayerLanded;

    //game events
    public static Action OnSaveGame;

    //action events
    public static Action<IInteractable> OnSelectNewInteractable;
    public static Action<bool> OnIsInteracting;

    //RPG events
    public static Action<float> OnFireRocketNotifyFireSpeed;
    public static Action<RpgData> OnChangeRpg;
    public static Action<RocketBase> OnUpdateClosestIncomingRocket;
    public static Action<RocketRemoteExplosion> OnUpdateSelectedRemoteRocket;
    public static Action<bool> OnFireOrDetonateRemote;

    //Weapon Wheel Events
    public static Action OnOpenWeaponWheel;
    public static Action OnCloseWeaponWheel;
    public static Action<FiringMode> OnSelectNewRpg;

    //Constant Value Updates
    public static Action<float> OnUpdateEnergy;
    public static Action<float> OnUpdateHealth;

    #endregion

    #region values

    public static RpgHolder GameRpgHolder;

    private static int _rocketCount;
    public static int RocketCount
    {
        get { return _rocketCount; }
        //ensure rocket count does not go below cero
        set 
        { 
            _rocketCount = value;

            if (_rocketCount <= 0f)
            {
                _rocketCount = 0;
                OnUpdateClosestIncomingRocket?.Invoke(null);
            }
        }
    }

    #endregion
}
