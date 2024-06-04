using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : ActivableBase
{
    private bool _isLockedState;
    private Animator _animator;
    public AudioCue _openSfx;
    public AudioCue _closeSfx;

    private void Awake()
    {
        _animator = GetComponent<Animator>();   
    }

    [ActivableAction]
    public void Open(bool set)
    {
        if (!_isLockedState)
        {
            AudioManager.TryPlayCueAtPoint(set ? _openSfx : _closeSfx,transform.position);
            _animator.SetBool("Open", set);

            if (Type != ActivableType.Action) SendAllActivations(set);
        }
    }

    [ActivableAction]
    public void LockState(bool set)
    {
        _isLockedState = set;
    }
}
