using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ActivableNode
{
    public enum nodeEventState
    {
        RIGHT,
        LEFT,
        UNSELECTED,
    }

    public Rect Box;
    public string Text;

    public ActivableNode(Vector2 pos, float width, float height)
    {
        Box = new Rect(pos, new Vector2(width, height));
        Text = "Name";
    }

    public virtual nodeEventState EventHandler(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDrag:
                if (Box.Contains(e.mousePosition))
                {
                    Box.position += e.delta;
                    return nodeEventState.LEFT;
                }
                break;
            case EventType.MouseDown:
                if (e.button == 0 && Box.Contains(e.mousePosition)) return nodeEventState.RIGHT;
                else if (e.button == 1 && Box.Contains(e.mousePosition)) return nodeEventState.LEFT;
                break;
        }
        return nodeEventState.UNSELECTED;
    }

    public void Paint()
    {
        GUI.Box(Box, Text);
    }
}
