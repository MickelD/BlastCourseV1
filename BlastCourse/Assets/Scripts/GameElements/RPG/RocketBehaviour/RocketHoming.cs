using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketHoming : RocketBase
{
    private RpgHoming rpgHoming;
    private WaitForSeconds homingSuspensionDelay;
    private bool lockedHoming;

    protected override void Start()
    {
        base.Start();

        rpgHoming = (RpgHoming)rpg;

        homingSuspensionDelay = new WaitForSeconds(rpgHoming.homingSuspensionInterval);
    }

    public override void SetVelocity(Vector3 vel)
    {
        if (!lockedHoming) base.SetVelocity(vel);
    }

    public override void BouncePadInteraction(Vector3 dir, float force)
    {
        base.BouncePadInteraction(dir, force);

        StartCoroutine(HomingSuspensionDelay());
    }

    private IEnumerator HomingSuspensionDelay()
    {
        lockedHoming = true;

        yield return homingSuspensionDelay;

        lockedHoming = false;
    }
}
