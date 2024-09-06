using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ParticleAnimEvent : MonoBehaviour
{
    [SerializeField] ParticleSystem _plop;
    public void Plop()
    {
        Debug.Log("Plop");
        _plop.Play();
    }
}


