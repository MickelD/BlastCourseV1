using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CustomMethods;
using UnityEngine.Events;

public class EventPropagator : MonoBehaviour
{
    //TRY TO EXTEND THIS CLASS FUNCTIONALITY TO BE ABLE TO TAKE ALL EVENTS/ACTIONS FROM A STATIC CLASS 
    //AND ASSIGN UNITY EVENTS TO THEM

    [SerializeField] UnityEvent _OnOpenWeaponWheel;
    [SerializeField] UnityEvent _OnCloseWeaponWheel;

    private void OnEnable()
    {
        EventManager.OnCloseWeaponWheel += CloseWW;
        EventManager.OnOpenWeaponWheel += OpenWW;
    }

    private void OnDisable()
    {
        EventManager.OnCloseWeaponWheel -= CloseWW;
        EventManager.OnOpenWeaponWheel -= OpenWW;
    }

    private void CloseWW()
    {
        _OnCloseWeaponWheel.Invoke();
    }

    private void OpenWW()
    {
        _OnOpenWeaponWheel.Invoke();
    }
}
