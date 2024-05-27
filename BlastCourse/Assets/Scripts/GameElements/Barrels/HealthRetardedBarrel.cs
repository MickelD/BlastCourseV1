using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRetardedBarrel : HealthExplosiveBarrel
{
    #region Fields

    [Space(5), Header("Particles"), Space(3)]
    [SerializeField] ParticleSystem g_particle;

    #endregion

    #region Methods

    protected override IEnumerator Explode()
    {
        g_particle.Play();

        yield return new WaitForSeconds(_timeToExplode);
        _explosion.Explode(transform.position, transform.up);

        g_particle.Stop();
        Destroy(g_particle.gameObject, 2f);
        g_particle.gameObject.transform.parent = null;

        Destroy(this.gameObject);
    }

    #endregion
}
