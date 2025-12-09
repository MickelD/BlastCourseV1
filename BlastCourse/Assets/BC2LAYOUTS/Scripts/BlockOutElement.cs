using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum LineOrder
{
    XZY,
    ZXY,
    YXZ,
    YZX
}

public class BlockOutElement : MonoBehaviour
{
    public Texture Icon;
    public Vector3 IconOffset = Vector3.up;
    public Color IconTint = Color.white;

    public Color LineColor = Color.cyan;
    public Vector2 LineThickness;
    public LineOrder lineOrder;
    public bool alwaysDotted;
    public Transform[] connectTo;

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.TransformPoint(IconOffset), Icon.name, true, IconTint);
        //transform.forward = Camera.current.transform.forward;


        Handles.color = LineColor;
        if (!alwaysDotted)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            ConnectLine(false);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
        }
        ConnectLine(true);

    }

    private void ConnectLine(bool dotted)
    {
        foreach (Transform t in connectTo)
        {
            Vector3 p1, p2, p3, p4;

            p1 = transform.position;
            p4 = t.position;

            switch (lineOrder)
            {
                default:
                case LineOrder.XZY:

                    p2 = p1;
                    p2.x = t.position.x;
                    p3 = p2;
                    p3.z = t.position.z;
                    break;

                case LineOrder.ZXY:

                    p2 = p1;
                    p2.z = t.position.z;
                    p3 = p2;
                    p3.x = t.position.x;
                    break;

                case LineOrder.YXZ:

                    p2 = p1;
                    p2.y = t.position.y;
                    p3 = p2;
                    p3.x = t.position.x;
                    break;

                case LineOrder.YZX:
                    p2 = p1;
                    p2.y = t.position.y;
                    p3 = p2;
                    p3.z = t.position.z;
                    break;
            }


            if (dotted)
            {
                Handles.DrawDottedLines(new Vector3[] { p1, p2, p2, p3, p3, p4 }, LineThickness.y);
            }
            else
            {
                Handles.DrawLine(p1, p2, LineThickness.x);
                Handles.DrawLine(p2, p3, LineThickness.x);
                Handles.DrawLine(p3, p4, LineThickness.x);
            }
        }
    }
}


