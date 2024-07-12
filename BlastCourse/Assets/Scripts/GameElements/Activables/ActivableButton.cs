using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using CustomMethods;

public class ActivableButton : ActivableBase
{
    #region Variables

    private Animator _animator;

    public bool InverseSignal;
    public bool ResetOnUnpress;

    [Space(5), Header("Audio"), Space(3)]
    public AudioCue _interactSfx;
    public ParticleSystem ClickVFX;

    public bool Locked { get; set; }

    #endregion


    #region Methods

    protected override void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        if (ClickVFX != null) ClickVFX.Stop();

        base.Start();
    }

    public virtual void Press(bool press)
    {
        if(_animator != null)_animator.SetBool("Press", press);
        if (ClickVFX != null && press) ClickVFX.Play();
        else if (ClickVFX != null) ClickVFX.Stop();

        if(press)AudioManager.TryPlayCueAtPoint(_interactSfx, transform.position);

        if (press || ( !press && ResetOnUnpress)) SendAllActivations(ExtendedDataUtility.Select(InverseSignal, !press, press));
    }

    protected void SetActiveAnim(bool state)
    {
        if(_animator != null && state != _animator.GetBool("Press")) _animator.SetBool("Press", state);
    }

    #endregion
}
