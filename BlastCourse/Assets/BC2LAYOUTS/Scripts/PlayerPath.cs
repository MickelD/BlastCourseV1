using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlayerPath : MonoBehaviour
{
    public Transform moveTo;
    [HideInInspector] public bool drawJump;
    [DrawIf("drawJump", true)] public Color moveColor = new Color(0f,0.75f,0f, 1f);
    [DrawIf("drawJump", true)] public Color moveColorBehind = new Color(0f, 0.5f, 0f, 1f);
    [DrawIf("drawJump", true)] public float moveThickness = 5f;
    [DrawIf("drawJump", true)] public bool moveStraight;
    [DrawIf("drawJump", true), DrawIf("moveStraight", false)] public Vector2 moveCurve = new Vector2(0.85f, 5f);

    [Space(5)] public Transform shootTo;
    [HideInInspector] public bool drawShoot;
    [DrawIf("drawShoot", true)] public Color shootColor = Color.green;
    [DrawIf("drawShoot", true)] public float shootThickness = 3f;
    [DrawIf("drawShoot", true)] public Vector2 shootHeights = new Vector2(2f, 1f);

    private void OnValidate()
    {
        drawJump = moveTo != null;
        drawShoot = shootTo != null;
    }

    private void OnDrawGizmos()
    {
        if (moveTo != null)
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
            DrawPath(moveColorBehind);
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            DrawPath(moveColor);
        }

        if (shootTo != null)
        {
            Vector3 p1 = transform.position + Vector3.up * shootHeights.x;
            Vector3 p2 = shootTo.position + shootTo.up * shootHeights.y;

            Handles.color = shootColor;
            Handles.DrawDottedLine(p1, p2, shootThickness);
        }
    }


    void DrawPath(Color color)
    {
        Vector3 p1 = transform.position;
        Vector3 p2 = moveTo.position;

        if (moveStraight)
        {
            Handles.color = color;
            Handles.DrawLine(p1, p2, moveThickness);
        }
        else
        {
            Vector3 t1 = p1;
            t1.y = p2.y + moveCurve.y;
            Vector3 t3 = p2;
            t3.y = t1.y;
            Vector3 t2 = Vector3.LerpUnclamped(t1, t3, moveCurve.x);

            Handles.DrawBezier(p1, p2, t1, t2, color, null, moveThickness);
        }
    }
}



