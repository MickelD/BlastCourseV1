using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpeakerVFX : MonoBehaviour
{
    #region Fields

    public ParticleSystem BlaVFX;

    #endregion

    #region UnityFunctions

    public void Start()
    {
        if (DialogueManager.Instance != null) DialogueManager.Instance.ActivateSpeakers += TurnOnSpeaker;
        BlaVFX.Stop();
    }
    public void OnDestroy()
    {
        if (DialogueManager.Instance != null) DialogueManager.Instance.ActivateSpeakers -= TurnOnSpeaker;
        BlaVFX.Stop();
    }

    #endregion

    #region Methods

    public void TurnOnSpeaker(bool isOn)
    {
        if (isOn) BlaVFX.Play();
        else BlaVFX.Stop();
    }

    #endregion
}


