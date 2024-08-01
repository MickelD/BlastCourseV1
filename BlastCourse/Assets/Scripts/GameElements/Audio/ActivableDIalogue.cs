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

    #endregion

    #region AntiSave

    bool firstFrame = true;
    private IEnumerator DontStart()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForEndOfFrame();
        firstFrame = false;
        Debug.Log("CanActivate");
    }
    private new void Start() 
    {
        base.Start();
        StartCoroutine(DontStart()); 
    }

    #endregion

    #region Methods

    [ActivableAction]
    public void Sound(bool shouldTrigger)
    {
        if (Audio.SfxClip != null && shouldTrigger && !firstFrame) DialogueManager.Instance.TryPlayCueAtPoint(Audio, transform.position);
    }

    #endregion
}


