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
    public bool isPlaying;
    [SerializeField] Animator _animator;

    #endregion

    #region UnityFunctions

    public void Start()
    {
        if (DialogueManager.Instance != null) DialogueManager.Instance.ActivateSpeakers += TurnOnSpeaker;
    }
    public void OnDestroy()
    {
        if (DialogueManager.Instance != null) DialogueManager.Instance.ActivateSpeakers -= TurnOnSpeaker;
        BlaVFX.Stop();
    }

    #endregion

    #region Methods

    private void OnEnable()
    {
        if (isPlaying) BlaVFX.Play();
    }

    public void TurnOnSpeaker(bool isOn)
    {
        isPlaying = isOn;
        _animator.SetBool("talk", isOn);
        if (isOn) BlaVFX.Play();
        else BlaVFX.Stop();
    }

    #endregion
}


