using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueTrigger : MonoBehaviour
{
    #region Fields

    [Serializable]
    public class AudioCueLogic
    {
        public int triggerNumber;
        public AudioCue audioCue;
    }

    public List<AudioCueLogic> AudioCues;

    #endregion

    #region Vars

    private int triggerCount;

    #endregion

    #region UnityFunctions

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            triggerCount++;

            foreach(AudioCueLogic a in AudioCues)
            {
                if(a.triggerNumber == triggerCount)
                {
                    DialogueManager.Instance.TryPlayCueAtPoint(a.audioCue, transform.position);

                    return;
                }
            }
        }
    }

    #endregion
}


