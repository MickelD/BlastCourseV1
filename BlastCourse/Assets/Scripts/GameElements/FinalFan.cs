using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FinalFan : MonoBehaviour
{
    [SerializeField] float _power;

    private void OnTriggerStay(Collider other)
    {
        other.attachedRigidbody.AddForce(Vector3.up * _power * Time.deltaTime);
    }
}


