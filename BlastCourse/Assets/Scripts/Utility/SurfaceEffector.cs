using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMethods;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SurfaceEffector : MonoBehaviour
{
    [SerializeField] Vector3 _targetPos;
    [SerializeField] bool _inverseDirection;
    [SerializeField] bool _ignoreY;
    [SerializeField] bool _useAccelerationCurve;
    [SerializeField, DrawIf(nameof(_useAccelerationCurve), false)] float _acceleration;
    [SerializeField, DrawIf(nameof(_useAccelerationCurve), true)] AnimationCurve _accelerationCurve;
    [SerializeField] float _mult = 1f;

    private void OnCollisionStay(Collision collision)
    {
        ApplyAcceleration(collision.rigidbody);
    }

    private void OnTriggerStay(Collider other)
    {
        ApplyAcceleration(other.attachedRigidbody);
    }

    private void ApplyAcceleration(Rigidbody rb)
    {
        float a = _useAccelerationCurve ? _accelerationCurve.Evaluate(ExtendedDataUtility.Select(_ignoreY, 
                                                ExtendedMathUtility.VectorXZDistance(rb.position, transform.position), 
                                                Vector3.Distance(rb.position, transform.position))) 
                                        : _acceleration;

        rb.AddForce(ExtendedDataUtility.Select(_ignoreY, ExtendedMathUtility.HorizontalDirection(rb.position, transform.position + _targetPos), (transform.position + _targetPos - rb.position).normalized) * a * ExtendedDataUtility.Select(_inverseDirection, -_mult, _mult) * Time.deltaTime, ForceMode.Acceleration);

        //Debug.DrawRay(rb.position, ExtendedDataUtility.Select(_ignoreY, ExtendedMathUtility.HorizontalDirection(rb.position, transform.position + _targetPos), (transform.position + _targetPos - rb.position).normalized) * a * ExtendedDataUtility.Select(_inverseDirection, -0.1f, 0.1f), Color.green);
    }

    public void SetMult (float m) => _mult = m;

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _inverseDirection? Color.red : Color.blue;
        if (!_ignoreY) Gizmos.DrawSphere(transform.position + _targetPos, 0.25f);
        else Gizmos.DrawLine(transform.position + Vector3.up * 10f, transform.position - Vector3.up * 10f);
    }

#endif
}


