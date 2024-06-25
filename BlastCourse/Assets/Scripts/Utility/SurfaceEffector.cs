using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SurfaceEffector : MonoBehaviour
{
    [SerializeField] Vector3 _targetPos;
    [SerializeField] float _acceleration;
    private Rigidbody _actingRb;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out _actingRb))
            _actingRb.AddForce((transform.position + _targetPos - _actingRb.position).normalized * _acceleration * Time.deltaTime, ForceMode.Acceleration);
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + _targetPos, 0.25f);
    }

#endif
}


