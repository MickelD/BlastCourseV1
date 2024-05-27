
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GroundCheck : MonoBehaviour
{
    #region Fields

    [Tooltip("Number of side rays around of the object (Base: 16)"), SerializeField] private int _numberOfRays = 16;
    [Tooltip("The distance from the side rays to the center (Base: 0.5)"), SerializeField] private float _radius = 0.5f;

    #endregion

    #region Vars

    public RaycastHit GroundRaycastHit;
    float _rayCount;
    bool _checking;

    #endregion

    #region UnityFunctions

    #endregion

    #region Methods

    public RaycastHit CheckGround(Vector3 pos, float length, LayerMask lm)
    {
        _rayCount = 0;
        _checking = true;
        while (_checking && _rayCount <= _numberOfRays + 1) // +1 for the middle one
        {
            if(_rayCount == 0)
            {
                if (Physics.Raycast(pos, Vector3.down, out GroundRaycastHit, length, lm, QueryTriggerInteraction.Ignore))
                {
                    //Debug.Log("Detected from 0");
                    _checking = false;
                    return GroundRaycastHit;
                }
            }
            else
            {
                float rotation = _rayCount;
                rotation /= 2;
                rotation = (int)rotation;
                rotation *= Mathf.Pow(-1, _rayCount);
                rotation /= (_numberOfRays / 2);
                rotation *= 180;
                rotation %= (rotation + 360);
                if (Physics.Raycast(pos + (Quaternion.Euler(rotation * Vector3.up)  * (- transform.forward) * _radius), Vector3.down, out GroundRaycastHit, length, lm, QueryTriggerInteraction.Ignore))
                {
                    //Debug.Log("Detected from " + _rayCount);
                    _checking = false;
                    return GroundRaycastHit;
                }
            }

            _rayCount++;
        }

        return GroundRaycastHit;
    }

   

    [ContextMenu("Test")]
    public void Test()
    {
        if(_numberOfRays > 0)
        for(int i = 1; i <= _numberOfRays; i++)
        {
            float rotation = i;
            rotation /= 2;
            rotation = (int)rotation;
            rotation *= Mathf.Pow(-1, i);
            rotation /= (_numberOfRays / 2);
            rotation *= 180;
            rotation %= (rotation + 360);
            Debug.Log(rotation + " : " + i);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if(_numberOfRays > 0)
        for (int i = 1; i <= _numberOfRays; i++)
        {
            float rotation = i;
            rotation /= 2;
            rotation = (int)rotation;
            rotation *= Mathf.Pow(-1, i);
            rotation /= (_numberOfRays / 2);
            rotation *= 180;
            rotation %= (rotation + 360);


            //Gizmos.DrawRay(transform.position + (Quaternion.Euler(rotation * Vector3.up) * (-transform.forward) * _radius), Vector3.down);
        }
    }

    #endregion
}


