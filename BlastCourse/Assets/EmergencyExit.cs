using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EmergencyExit : MonoBehaviour, IInteractable
{
    [SerializeField] Animator _animator;
    [SerializeField] AudioCue _openDoorSfx;
    public bool Locked { get; set; }

    

    public virtual void SetInteraction(bool set, PlayerInteract interactor)
    {
        if (set)
        {
            AudioManager.TryPlayCueAtPoint(_openDoorSfx, gameObject.transform.position);
            _animator.SetTrigger("Open");
            Locked = true;
            if (interactor != null) interactor.CancelCurrentInteraction();
        }
    }
}
