using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class Edge
{
    public Node _fromNode;
    public Node _toNode;

    public Edge(Node node)
    {
        _fromNode = node;
        _toNode = null;
    }

    # if UNITY_EDITOR

    // Paint from point to point
    public void Paint()
    {
        if (_fromNode == null || _toNode == null) return;

        Handles.color = Color.white;

        //Get Points
        Vector2 start = new Vector2(_fromNode.Box.x + _fromNode.Box.width / 2, _fromNode.Box.y + _fromNode.Box.height / 2);
        Vector2 end = new Vector2(_toNode.Box.x + _toNode.Box.width / 2, _toNode.Box.y + _toNode.Box.height / 2);

        #region Draw

        // Draw connecting Line
        Vector3[] points = new Vector3[2];
        points[0] = start;
        points[1] = end;
        Handles.DrawAAPolyLine(4, points);

        #endregion
    }

    // Paint from point to mouse
    public void Paint(Vector2 mousePos)
    {
        if (_fromNode == null) return;

        Handles.color = Color.white;

        //Get Points
        Vector2 start = new Vector2(_fromNode.Box.x + _fromNode.Box.width / 2, _fromNode.Box.y + _fromNode.Box.height / 2);
        Vector2 end = mousePos;

        #region Draw

        // Draw connecting Line
        Handles.DrawLine(start, end);

        #endregion
    }

    #endif
}
