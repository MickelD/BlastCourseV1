using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HomingBehaviour", menuName = "RPG Behaviour/Homing Launcher")]
public class RpgHoming : RpgBase
{
    #region Fields
    public float turnSpeed;
    [Tooltip("The amount of time the rockets' homing abilities are disabled when interacting with a Bounce Pad")] public float homingSuspensionInterval;

    #endregion

    #region Variables

    public Vector3 targetPos;

    #endregion

    #region Methods

    public override void TickUnselected()
    {
        base.TickUnselected();
        if (rockets == null || rockets.Count <= 0) return;
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hitInfo, _maxAimDistance, _aimLayerMask, QueryTriggerInteraction.Ignore))
        {
            targetPos = hitInfo.point;
        }
        else
        {
            targetPos = _camera.transform.position + _camera.transform.forward * _maxAimDistance;
        }

        foreach (RocketHoming rocket in rockets)
        {
            Vector3 direction = Vector3.RotateTowards(rocket.transform.forward, (targetPos - rocket.transform.position).normalized, turnSpeed * Time.deltaTime, Mathf.Infinity);
            rocket.SetVelocity(direction.normalized * _stats.RocketSpeed);

            //rocket.Body.velocity = Vector3.RotateTowards(rocket.Body.velocity, (targetPos - rocket.transform.position).normalized * rocket.Body.velocity.magnitude, turnSpeed * Time.deltaTime, Mathf.Infinity);
        }

    }

    #endregion
}
