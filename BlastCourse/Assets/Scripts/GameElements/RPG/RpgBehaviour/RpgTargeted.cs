using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetedBehaviour", menuName = "RPG Behaviour/Targeted Position")]
public class RpgTargeted : RpgBase
{
    private bool _isPositionMarked;
    private Vector3 _target;

    public override void SecondaryFire()
    {
        _isPositionMarked = Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hitInfo, _maxAimDistance, _aimLayerMask, QueryTriggerInteraction.Ignore);
        if (_isPositionMarked)
        {
            _target = hitInfo.point;
        }
    }

    public override void PrimaryFire()
    {
        if (_isPositionMarked)
        {
            FireRocketAtPosition(_target, false);
        }
    }
}
