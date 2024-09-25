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

    public List<AudioCueLogic> AudioCues;

    public string Id;

    #endregion

    #region Vars

    private int triggerCount;

    #endregion

    #region UnityFunctions

    public void Start()
    {
        if(SaveLoader.Instance != null)
        {
            int c = SaveLoader.Instance.GetDialogueCount(Id);
            if(c > 0) triggerCount = c;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            triggerCount++;
            SaveLoader.Instance.SetDialogueCount(Id, triggerCount);

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

[Serializable]
public class AudioCueLogic
{
    public int triggerNumber;
    public AudioCue audioCue;
}


