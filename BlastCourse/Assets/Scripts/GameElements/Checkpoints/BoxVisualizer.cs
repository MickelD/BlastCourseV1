using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider))]
public class BoxVisualizer : MonoBehaviour
{
    private BoxCollider _collider;
    protected BoxCollider _Collider
    {
        get { 
            if (_collider == null)
            {
                _Collider = gameObject.GetComponent<BoxCollider>();
            }
            return _collider; 
        }
        set { _collider = value; }
    }

    protected virtual Color GetColor()
    {
        return Color.white;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = GetColor();
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.DrawCube(_Collider.center, _Collider.size);
    }
}


