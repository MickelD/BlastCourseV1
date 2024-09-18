using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MailTrigger : ActivableBase, IInteractable
{
    #region Fields
    [SerializeField] public AudioCue _interactSfx;
    [SerializeField] public Animator _animator;
    [SerializeField] public ParticleSystem _particles; 
    public bool Locked { get; set; }

    #endregion

    #region UnityFunctions


    public virtual void SetInteraction(bool set, PlayerInteract interactor)
    {
        if (set)
        {
            if (interactor != null) interactor.CancelCurrentInteraction();
            RPGAnimator.Instance.TakeBox();
            SendAllActivations(true);
        }
    }

    #endregion

    #region Methods

    public override void SendAllActivations(bool isActive)
    {
        Locked = true;
        _animator.SetTrigger("Deliver");
        _particles.Emit(20);
        AudioManager.TryPlayCueAtPoint(_interactSfx, transform.position);
        base.SendAllActivations(isActive);
    }

    #endregion
}


