using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : ActivableBase
{
    private bool _isLockedState;
    private Animator _animator;
    public AudioCue _openSfx;
    public AudioCue _closeSfx;

    public ParticleSystem _openVFX1;
    public ParticleSystem _openVFX2;
    public float ActiveTime;
    private WaitForSeconds VfxActiveTime;
    private bool _open;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        VfxActiveTime = new WaitForSeconds(ActiveTime);
    }

    [ActivableAction]
    public void Open(bool set)
    {
        if (!_isLockedState && set != _open)
        {
            AudioManager.TryPlayCueAtPoint(set ? _openSfx : _closeSfx,transform.position);
            _animator.SetBool("Open", set);
            _open = set;
            
            if (gameObject.activeInHierarchy) StartCoroutine(VFX());
            if (Type != ActivableType.Action) SendAllActivations(set);
        }
    }

    [ActivableAction]
    public void LockState(bool set)
    {
        _isLockedState = set;
    }

    public IEnumerator VFX()
    {
        ParticleSystem ps = Random.Range(0, 2) == 1 ? _openVFX1 : _openVFX2;
        ps.gameObject.SetActive(true);
        ps.Play();
        yield return VfxActiveTime;
        ps.Stop();
        ps.gameObject.SetActive(false);
    }
}
