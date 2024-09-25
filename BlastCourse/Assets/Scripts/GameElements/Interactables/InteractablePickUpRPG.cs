using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractablePickUpRPG : MonoBehaviour, IInteractable
{
    #region Fields

    [Space(5), Header("Properties"), Space(3)]
    public FiringMode RpgUnlocked;

    [Space(5), Header("Components"), Space(3)]
    public RpgHolder Rpgs;
    public AudioCue Audio;

    #endregion

    #region Variables

    //Variables from Interface
    public bool Locked { get; set; }

    #endregion

    #region UnityFunctions

    private void Start()
    {
        if (SaveLoader.Instance != null
            && SaveLoader.Instance.UnlockedRpgs != null
            && SaveLoader.Instance.UnlockedRpgs.Length == 4
            && SaveLoader.Instance.UnlockedRpgs[(int)RpgUnlocked])
            gameObject.SetActive(false);
    }

    #endregion

    #region Methods

    //Functions from Interface
    public void SetInteraction(bool set, PlayerInteract interactor)
    {
        if (set)
        {
            Rpgs.AcquireRpg(RpgUnlocked);

            interactor.SetInteractWith(this, false);

            if(SaveLoader.Instance != null) SaveLoader.Instance.Save();
            if (Audio.SfxClip != null && DialogueManager.Instance != null && Audio.SfxClip.Length > 0) DialogueManager.Instance.TryPlayCueAtPoint(Audio, transform.position);

            Destroy(gameObject);
        }
    }
    #endregion
}
