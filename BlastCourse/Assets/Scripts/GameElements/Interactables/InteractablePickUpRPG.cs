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

    #endregion

    #region Variables

    //Variables from Interface
    public bool Locked { get; set; }

    #endregion

    #region UnityFunctions

    private void Awake()
    {
        if (SaveLoader.Instance != null
            && SaveLoader.Instance.UnlockedRpgs == null
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

            Destroy(gameObject);
        }
    }
    #endregion
}
