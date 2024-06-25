using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : ActivableBase
{
    private bool _isLockedState;
    private Animator _animator;
    public AudioCue _openSfx;
    public AudioCue _closeSfx;

    public GameObject _openVFX1;
    public GameObject _openVFX2;

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

            Destroy(Instantiate(Random.Range(0, 2) == 1 ? _openVFX1 : _openVFX2, transform.position + Vector3.up, Quaternion.identity), 5);

            if (Type != ActivableType.Action) SendAllActivations(set);
        }
    }

    [ActivableAction]
    public void LockState(bool set)
    {
        _isLockedState = set;
    }
}
