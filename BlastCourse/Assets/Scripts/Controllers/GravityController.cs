using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityController : MonoBehaviour
{
    public float Acceleration;

    public float Scale = 1f;

    bool _lock;

    public bool EnableGravity 
    {
        get { return _enableGravity;  }
        set
        {
            if (_lock) return;

            _enableGravity = value;
            if (!enabled) 
            {
                if (_rb == null) _rb = gameObject.GetComponent<Rigidbody>();
                _rb.useGravity = value;
            }
        }
    }

    [SerializeProperty(nameof(EnableGravity)), SerializeField]
    private bool _enableGravity;

    private Rigidbody _rb;

    private void OnValidate()
    {
        InitializeRigidBody();
    }

    private void Awake()
    {
        InitializeRigidBody();
    }

    public void LockState(bool set)
    {
        enabled = EnableGravity = set;
        _lock = true;
    }

    private void FixedUpdate()
    {
        _rb.velocity += (100f * Acceleration * Scale) * EnableGravity.GetHashCode() * Time.deltaTime * Time.deltaTime * Vector3.down;
    }

    public void InitializeRigidBody()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        _rb.useGravity = !enabled;
    }
}
