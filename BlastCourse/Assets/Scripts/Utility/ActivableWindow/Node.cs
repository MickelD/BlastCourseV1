using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class Node
{
    public enum nodeEventState
    {
        RIGHT,
        LEFT,
        UNSELECTED,
    }

    public Rect Box;
    public string Text;
    public GameObject GO;

    public IActibableAction action;
    public Rect inputBox;
    public IActivableTrigger trigger;
    public Rect outputBox;

    public Node(Vector2 pos, float width, float height, IActibableAction a, IActivableTrigger t)
    {
        Box = new Rect(pos, new Vector2(width, height));
        if(a != null)
        {
            action = a;
            GO = action.gameObject;
            Text = GO.name;

            inputBox = new Rect(pos - Vector2.right * height / 2 + Vector2.up * height / 4, new Vector2(height / 2, height / 2));
        }
        if (t != null)
        {
            trigger = t;
            if(GO == null)
            {
                GO = trigger.gameObject;
                Text = GO.name;
            }

            outputBox = new Rect(pos + Vector2.right * width + Vector2.up * height / 4, new Vector2(height / 2, height / 2));
        }
        if (GO == null) Text = "Empty";
    }

    #if UNITY_EDITOR

    public nodeEventState EventHandler(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDrag:
                if (Box.Contains(e.mousePosition))
                {
                    Debug.Log("Dragging");
                    Box.position += e.delta;
                    outputBox.position += e.delta;
                    inputBox.position += e.delta;
                    return nodeEventState.LEFT;
                }
                break;
            case EventType.MouseDown:
                if (e.button == 0 && Box.Contains(e.mousePosition))
                {
                    Selection.activeObject = GO;
                    return nodeEventState.LEFT;
                }
                else if (e.button == 1 && Box.Contains(e.mousePosition)) return nodeEventState.RIGHT;
                break;
        }
        return nodeEventState.UNSELECTED;
    }

    public void Paint()
    {
        GUI.color = Color.grey;
        GUI.Box(Box, Text);
        GUI.color = Color.cyan;
        GUI.Box(inputBox, "");
        GUI.color = Color.yellow;
        GUI.Box(outputBox, "");
    }

    #endif
}
