using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public GameObject gameObject { get;}

    public bool Locked { get; set; } // If it is being interacted with or not or if already has bee interacted in case of buttons
    public void SetInteraction(bool set, PlayerInteract interactor);
}
