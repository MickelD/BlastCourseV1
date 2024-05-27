using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GateNode : ActivableNode
{
    public LogicGate GO;
    public GateNode(Vector2 pos, float width, float height, LogicGate go) : base(pos, width, height) 
    {
        GO = go;
        Text = go.gameObject.name;
    }

    public override nodeEventState EventHandler(Event e)
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
                if (e.button == 0 && Box.Contains(e.mousePosition))
                {
                    Selection.activeObject = GO.gameObject;
                    return nodeEventState.LEFT;
                }
                else if (e.button == 1 && Box.Contains(e.mousePosition)) return nodeEventState.RIGHT;
                break;
        }
        return nodeEventState.UNSELECTED;
    }
}
