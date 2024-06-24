using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaledTimeMonoBehaviour : MonoBehaviour
{
    [HideInInspector] public float LocalTimeScale = 1f;

    public Rigidbody Body;
    public GravityController GravityController;

    private bool _hasGravity;
    private Tweener _tweener;
    private Vector3 _velocity;

    private bool _animated;
    private Animator _animator;

    protected virtual void Start()
    {
        Body = gameObject.GetComponent<Rigidbody>();

        _hasGravity = gameObject.TryGetComponent(out GravityController);
        _animated = gameObject.TryGetComponent(out _animator);
    }

    protected virtual void LateUpdate()
    {
        if (_hasGravity) GravityController.Scale = LocalTimeScale;
        if(_animated) _animator.speed = LocalTimeScale;

        Body.angularVelocity *= LocalTimeScale;

        Body.velocity *= LocalTimeScale;
    }

    public void FreezeTime(bool freeze) 
    {
        if (freeze)
        {
            _velocity = Body.velocity;
            float spd = _velocity.magnitude;

            if (spd > 8f)
            {
                float delay = 0.5f / (spd * 0.08f);
                Vector3 oldPos = transform.position;
                LocalTimeScale = 1f;
                _tweener = DOTween.To(() => LocalTimeScale, x => LocalTimeScale = x, 0f, delay);
                this.Invoke(() => { OnFreezeTime(true); LocalTimeScale = 0f; }, delay);
            }
            else
            {
                LocalTimeScale = 0f;
                OnFreezeTime(true);
            }
        }
        else
        {
            _tweener?.Kill();
            LocalTimeScale = 1f;
            Body.velocity = _velocity;

            OnFreezeTime(false);
        }
    }

    protected virtual void OnFreezeTime(bool freeze)
    {

    }
}


