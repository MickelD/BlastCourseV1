using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthExplosiveBarrel : Health
{
    #region Fields

    [Space(5), Header("Explosion"), Space(3)]
    [SerializeField] protected Explosion _explosion;
    [SerializeField] protected float _timeToExplode;

    #endregion

    #region Variables

    float _explosionTimer;

    #endregion

    #region Methods

    public override void SufferDamage(float amount, Source source)
    {
        if (source != Source.ENVIRONMENT) _explosionTimer = 0;
        else _explosionTimer = _timeToExplode;

        base.SufferDamage(amount, source);
    }

    public override void Die(bool n)
    {
        StartCoroutine(Explode());
    }

    protected virtual IEnumerator Explode()
    {
        yield return new WaitForSeconds(_explosionTimer);
        _explosion.Explode(transform.position, transform.up);
        Destroy(this.gameObject);
    }

    #endregion
}
