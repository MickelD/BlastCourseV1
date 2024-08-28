using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDroneHealth : Health
{
    [Space(5), Header("Explosion"), Space(3)]
    [SerializeField] protected Explosion _explosion;


    [Space(5), Header("Audio"), Space(3)]
    [SerializeField] AudioCue laserDeathSound;
    [SerializeField] AudioCue takeDamageSound;


    public override void SufferDamage(float amount, Source source)
    {
        base.SufferDamage(amount, source);
        AudioManager.TryPlayCueAtPoint(takeDamageSound, transform.position);
    }

    public override void Die(bool n)
    {
        _explosion.Explode(transform.position, transform.up);
        AudioManager.TryPlayCueAtPoint(laserDeathSound, transform.position);
        Destroy(this.gameObject);
    }
}
