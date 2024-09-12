using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPipe : RocketBase
{
    private RpgPipe _grenadeLauncher;
    private int _bounces;
    [SerializeField] TrailRenderer _trail;
    private bool _lock;
    [SerializeField] float _bounceCooldown;

    protected override void Start()
    {
        base.Start();

        _grenadeLauncher = (RpgPipe)rpg;
        this.Invoke(() =>  Explode(transform.position, Vector3.up), _grenadeLauncher.FuseTime);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        if (!_lock) 
        {
            _bounces++; 
            _lock = true;
            this.Invoke(() =>  _lock = false, _bounceCooldown);
        }

        if (_bounces >= _grenadeLauncher.BounceCount || collision.transform.GetComponent<DestructibleObject>())
        {
            base.OnCollisionEnter(collision);
        }
    }

    public override void BouncePadInteraction(Vector3 dir, float force)
    {
        if (rpg.ExplodeOnPlayerUponReflection) c_playerTrigger.enabled = true;

        _bounces++;

        float mag = Body.velocity.magnitude;
        SetVelocity((dir + _grenadeLauncher.VerticalForceMult * Vector3.up).normalized * force * 2f);
    }

    protected override void OnDestroy()
    {
        _trail.transform.parent = null;
        _trail.time = 0.5f;
        Destroy(_trail, 1f);
        base.OnDestroy();
    }
}
