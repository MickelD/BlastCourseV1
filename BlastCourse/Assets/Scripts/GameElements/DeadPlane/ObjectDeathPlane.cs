using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectDeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PhysicsObject physicsObject))
        {
            physicsObject.DestroyObject();
        }
    }
}


