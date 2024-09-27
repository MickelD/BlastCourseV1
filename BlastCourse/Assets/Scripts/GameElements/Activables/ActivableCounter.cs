using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActivableCounter : MonoBehaviour
{
    #region Fields

    public List<AudioCueLogic> _dialogues;
    public string Id;

    #endregion

    #region Vars

    private int count;

    #endregion

    #region UnityFunctions

    private void Start()
    {
        if (SaveLoader.Instance != null)
        {
            count = Mathf.Clamp(SaveLoader.Instance.GetDialogueCount(Id),0,5);
        }
    }

    #endregion

    #region Methods

    public void Count(bool up)
    {
        count += up ? 1 : -1;
        foreach(AudioCueLogic d in _dialogues) if(d.triggerNumber == count)
            {
                DialogueManager.Instance.TryPlayCueAtPoint(d.audioCue, transform.position);
                SaveLoader.Instance.SetDialogueCount(Id, count);
                return;
            }
        
    }

    #endregion
}




