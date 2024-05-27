using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundCheck))]
public class PositionAnchorer : MonoBehaviour
{
    #region Fields



    #endregion

    #region Vars

    private GroundCheck _groundCheck;
    private Rigidbody _rb;
    private FixedJoint _joint;
    private PlayerMovement _movement;

    private Vector3 _displacement;

    private Vector3 _oldPos;

    #endregion

    #region UnityFunctions

    private void Start()
    {
        _groundCheck = gameObject.GetComponent<GroundCheck>();
        _rb = gameObject.GetComponent<Rigidbody>();
        _movement = gameObject.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (_groundCheck.GroundRaycastHit.transform != null)
        {
            _displacement = _groundCheck.GroundRaycastHit.transform.localPosition - _oldPos;

            _oldPos = _groundCheck.GroundRaycastHit.transform.localPosition;
        }

        if(!_movement.GetLadder() && _groundCheck.GroundRaycastHit.transform != null && !_groundCheck.GroundRaycastHit.transform.GetComponent<PhysicsObject>())transform.parent = _groundCheck.GroundRaycastHit.transform;

        //transform.localPosition += _displacement;

        //_rb.MovePosition(_rb.position + _displacement);
    }

    #endregion

}


