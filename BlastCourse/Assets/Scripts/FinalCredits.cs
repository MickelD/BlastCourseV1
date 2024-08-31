using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FinalCredits : MonoBehaviour
{
    [SerializeField] UnityEvent _onApproach;
    [SerializeField] UnityEvent _onEntry;
    private int _entries;

    private void OnTriggerEnter(Collider other)
    {
        _entries++;

        if (_entries == 1)
        {
            _onApproach.Invoke();
        }
        else if (_entries == 2) 
        { 
            _onEntry.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _entries--;
    }
}


