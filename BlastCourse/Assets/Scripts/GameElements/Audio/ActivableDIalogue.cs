using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActivableDIalogue : ActivableBase
{
    #region Fields

    public AudioCue Audio;
    public string Id;

    #endregion

    #region AntiSave

    bool firstFrame = true;
    bool hasTriggered = false;
    private IEnumerator DontStart()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
        firstFrame = false;
    }
    private new void Start() 
    {
        base.Start();
        if (SaveLoader.Instance != null)
        {
            int c = SaveLoader.Instance.GetDialogueCount(Id);
            if (c > 0) hasTriggered = true;
        }
        StartCoroutine(DontStart()); 
    }

    #endregion

    #region Methods

    [ActivableAction]
    public void Sound(bool shouldTrigger)
    {
        if (Audio.SfxClip != null && shouldTrigger && !firstFrame && !hasTriggered) DialogueManager.Instance.TryPlayCueAtPoint(Audio, transform.position);
        hasTriggered = shouldTrigger;
    }

    #endregion
}


