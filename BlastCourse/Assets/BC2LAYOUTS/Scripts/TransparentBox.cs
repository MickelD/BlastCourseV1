using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TransparentBox : MonoBehaviour
{
    public Material normalMat;
    public Material transMat;
    public bool makeTrans;
    public Color outlineColor = Color.black;

    public Transform goTo;
    public Color lineColor;
    public float lineThickness;


    private void OnValidate()
    {
        gameObject.GetComponent<Renderer>().material = makeTrans? transMat : normalMat;
    }


    private void OnDrawGizmos()
    {
        if (makeTrans)
        {
            Gizmos.color = outlineColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        if (goTo != null)
        {
            Handles.color = lineColor;
            Handles.DrawDottedLine(transform.position, goTo.position, lineThickness);
        }
    }
}


