using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class BlastButton : ActivableButton, IExplodable
{
    #region Fields

    [Space(5), Header("Blast Button Properties"), Space(3)]

    [Tooltip("Unpress itself after the delay")] public bool ResetSelf;
    [ExcludeFromActivableEditor(nameof(ResetSelf))] public float Delay;
    [Tooltip("Objects in this layers block line of sight to the explosion")] public LayerMask ExplosionBlockingMask;
    public float MaxRadius;

    #endregion

    #region Methods

    public void ExplosionBehaviour(Vector3 origin, Explosion exp, Vector3 normal)
    {
        if (!Locked && Vector3.Distance(origin, transform.position) <= MaxRadius && !Physics.Raycast(transform.position + transform.up * 0.375f, (origin - (transform.position + transform.up * 0.375f)).normalized, Vector3.Distance(transform.position + transform.up * 0.375f, origin) - 0f, ExplosionBlockingMask, QueryTriggerInteraction.Ignore)) 
        {
            Press(true);

            Locked = true;

            if (ResetSelf)
            {
                this.Invoke(() => Press(false), Delay) ;
            }
        }
    }

    [ActivableAction]

    public override void Press(bool press)
    {
        base.Press(press);

        Locked = press;
    }

    #endregion
}