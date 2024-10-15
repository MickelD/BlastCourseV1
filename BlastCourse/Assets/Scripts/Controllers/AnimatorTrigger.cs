using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Animator))]
public class AnimatorTrigger : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] bool _sendOnce;
    private bool _blocked;
    [SerializeField] bool _sendBoolEvent;
    [DrawIf(nameof(_sendBoolEvent), true), SerializeField] string _animatorBoolEvent;
    [SerializeField] bool _sendTriggerEvent;
    [DrawIf(nameof(_sendTriggerEvent), true), SerializeField] string _animatorTriggerName;
    [SerializeField] bool _sendIntEvent;
    [DrawIf(nameof(_sendIntEvent), true), SerializeField] bool _onlyAdd;
    [DrawIf(nameof(_sendIntEvent), true), SerializeField] string _animatorIntName;
    private int _intCounter;
    [SerializeField] UnityEvent _onTriggerEnter;
    [SerializeField] UnityEvent _onTriggerExit;
    public AudioCue _sfxBreak;

    private void Start()
    {
        _animator = gameObject.GetComponent<Animator>();

        if (_sendIntEvent) _intCounter = _animator.GetInteger(_animatorIntName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_blocked) return;
        if (_sendOnce) _blocked = true;

        if (_sendBoolEvent) _animator.SetBool(_animatorBoolEvent, true);
        if (_sendTriggerEvent) _animator.SetTrigger(_animatorTriggerName); 
        if (_sendIntEvent)
        {
            _intCounter++;
            _animator.SetInteger(_animatorIntName, _intCounter);
        }
        _onTriggerEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (_blocked) return;
        if (_sendOnce) _blocked = true;

        if (_sendBoolEvent) _animator.SetBool(_animatorBoolEvent, false);
        if (_sendIntEvent && !_onlyAdd)
        {
            _intCounter--;
            _animator.SetInteger(_animatorIntName, _intCounter);
        }
        AudioManager.TryPlayCueAtPoint(_sfxBreak, gameObject.transform.position);
        _onTriggerExit?.Invoke();
    }
}


