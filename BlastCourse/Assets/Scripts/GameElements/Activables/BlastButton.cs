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
    public float _MaxRadius;
    public Vector3 _RayOrigin;

    #endregion

    #region Methods

    public void ExplosionBehaviour(Vector3 origin, Explosion exp, Vector3 normal)
    {

        if (!Locked && Vector3.Distance(origin, transform.position) <= _MaxRadius 
                    && !(Physics.Linecast(origin + normal * 0.01f, transform.position + _RayOrigin, out RaycastHit hitInfo, ExplosionBlockingMask, QueryTriggerInteraction.Ignore)
                        && !hitInfo.transform.IsChildOf(transform))) 
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

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.5f, 0.25f, 0f, 0.5f);
        Gizmos.DrawSphere(transform.position + _RayOrigin, _MaxRadius);
    }

#endif
}