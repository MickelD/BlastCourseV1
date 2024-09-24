using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueStart : MonoBehaviour
{
    #region Fields

    #region Singleton Framework
    public static DialogueStart Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    public AudioCue dialogue;
    public float waitTime;
    public string Id;

    #endregion

    #region Variables

    public WaitForSeconds wait;

    #endregion

    #region UnityFunctions

    public void Start()
    {
        if(SaveLoader.Instance != null)
        {
            int c = SaveLoader.Instance.GetDialogueCount(Id);
            if(c > 0)
            {
                wait = new WaitForSeconds(waitTime);
                StartCoroutine(Play());
            }
            
        }
        
    }

    #endregion

    #region Methods

    public IEnumerator Play()
    {
        yield return wait;
        DialogueManager.Instance.TryPlayCueAtPoint(dialogue, transform.position);
        SaveLoader.Instance.SetDialogueCount(Id, 1);
    }

    #endregion
}


