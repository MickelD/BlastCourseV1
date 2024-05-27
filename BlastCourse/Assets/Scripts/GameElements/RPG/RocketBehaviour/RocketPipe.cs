using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPipe : RocketBase
{
    private RpgPipe _grenadeLauncher;
    private int _bounces;

    protected override void Start()
    {
        base.Start();

        _grenadeLauncher = (RpgPipe)rpg;
        this.Invoke(() =>  Explode(transform.position, Vector3.up), _grenadeLauncher.FuseTime);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        _bounces++;

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
}
