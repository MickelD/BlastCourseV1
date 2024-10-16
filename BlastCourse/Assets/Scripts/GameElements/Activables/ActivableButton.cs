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

    public bool Locked { get; set; }

    #endregion


    #region Methods

    protected override void Start()
    {
        _animator = gameObject.GetComponent<Animator>();

        base.Start();
    }

    public virtual void Press(bool press)
    {
        if(_animator != null)_animator.SetBool("Press", press);

        AudioManager.TryPlayCueAtPoint(_interactSfx, transform.position);

        if (press || ResetOnUnpress) SendAllActivations(ExtendedDataUtility.Select(InverseSignal, !press, press));
    }

    protected void SetActiveAnim(bool state)
    {
        if(_animator != null && state != _animator.GetBool("Press")) _animator.SetBool("Press", state);
    }

    #endregion
}
