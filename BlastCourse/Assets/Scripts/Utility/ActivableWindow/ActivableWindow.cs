using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR

public class ActivableWindow : EditorWindow
{
    private ActivableData data= null;
    private Edge _pendingEdge;


    [MenuItem("Window/Custom Windows/Activable Window")]
    public static ActivableWindow OpenWindow()
    {
        return GetWindow<ActivableWindow>("Activables");
    }


    [UnityEditor.Callbacks.OnOpenAsset(1)]
    public static bool OnOpenDatabase(int InstanceID, int line)
    {
        ActivableData nData = EditorUtility.InstanceIDToObject(InstanceID) as ActivableData;
        if(nData != null && nData.name == SceneManager.GetActiveScene().name)
        {
            ActivableWindow window = OpenWindow();
            window.data = nData;
            return true;
        }
        else if(nData != null && nData.name != SceneManager.GetActiveScene().name)
        {
            OpenWindow();
        }
        return false;
    }


    public void OnGUI()
    {
        if(data != null && data.name == SceneManager.GetActiveScene().name) 
        {
            CheckForItems();
            GUILayout.Label("Nodes: " + data.Nodes.Count);
            GUILayout.Label("Edges: " + data.Edges.Count);

            HandleEvents(Event.current);

            PaintNodes();
            PaintEdges();
            Repaint();
        }
        else
        {
            if (GUILayout.Button("Generate", GUILayout.Width(140)))
            {
                ActivableData nData = ScriptableObject.CreateInstance<ActivableData>();
                string nName = SceneManager.GetActiveScene().name;
                nData.name = nName;
                Debug.Log(nData + " " + nData.name);
                data = nData;

                AssetDatabase.CreateAsset(nData, "Assets/Scripts/Utility/ActivableWindow/" + nData.name + ".asset");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                data.Nodes = new List<Node>();
                data.Edges = new List<Edge>();
            }
        }

        
    }

    //Check for Items
    public void CheckForItems()
    {
        float width = 60;
        float heigh = 40;

        //Needed Lists
        List<IActivableTrigger> triggers = FindObjectsOfType<MonoBehaviour>(true).OfType<IActivableTrigger>().ToList();
        List<IActibableAction> actions = FindObjectsOfType<MonoBehaviour>(true).OfType<IActibableAction>().ToList();
        List<GameObject> both = new List<GameObject>();
        List<Node> nodesToAdd = new List<Node>();
        List<IActivableTrigger> tRemove = new List<IActivableTrigger>();
        List<IActibableAction> aRemove = new List<IActibableAction>();

        //Check for duality
        for (int i = 0; i < triggers.Count; i++)
        {
            for (int j = 0; j < actions.Count; j++)
            {
                if (triggers[i].gameObject == actions[j].gameObject)
                {
                    both.Add(triggers[i].gameObject);
                    tRemove.Add(triggers[i]);
                    aRemove.Add(actions[j]);
                }
            }
        }

        //Remove excess
        foreach (IActivableTrigger t in tRemove) triggers.Remove(t);
        foreach (IActibableAction a in aRemove) actions.Remove(a);

        //Convert all objects to nodes
        for (int i = 0; i < triggers.Count; i++)
        {
            Node node = new Node(new Vector2(100, (heigh + 5) * i + 10), width, heigh, null, triggers[i]);
            nodesToAdd.Add(node);
        }
        for (int j = 0; j < actions.Count; j++)
        {
            Node node = new Node(new Vector2(650, (heigh + 5) * j + 10), width, heigh, actions[j], null);
            nodesToAdd.Add(node);
        }
        for (int k = 0; k < both.Count; k++)
        {
            Node node = new Node(new Vector2(425, (heigh + 5) * k + 10), width + 20, heigh, both[k].GetComponent<MonoBehaviour>() as IActibableAction, both[k].GetComponent<MonoBehaviour>() as IActivableTrigger);
            nodesToAdd.Add(node);
        }

        //Add and/or delete nodes added or deleted
        for (int i = 0; i < nodesToAdd.Count; i++)
        {
            bool add = true;
            for (int j = 0; j < data.Nodes.Count; j++)
            {
                bool delete = true;
                if (nodesToAdd[i].GO == data.Nodes[j].GO) add = false;
                for (int k = 0; k < nodesToAdd.Count; k++)
                {
                    if (nodesToAdd[k].GO == data.Nodes[j].GO) delete = false;
                }
                if (delete) data.Nodes.RemoveAt(j);
            }
            if (add) data.Nodes.Add(nodesToAdd[i]);
        }

        //Move Nodes that are placed inside each other

    }
    public void MoveNode(Node node)
    {

    }


    //Handler
    public void HandleEvents(Event e)
    {
        Node.nodeEventState nodeState = Node.nodeEventState.UNSELECTED;
        bool openBaseMenu = true;

        if(data.Nodes != null && data.Nodes.Count > 0)
            foreach(Node node in data.Nodes)
            {
                nodeState = node.EventHandler(e);

                switch (nodeState)
                {
                    case Node.nodeEventState.RIGHT:
                        openBaseMenu = false;
                        NodeContextMenu(node, e.mousePosition);
                        break;

                    case Node.nodeEventState.LEFT:
                        openBaseMenu = false;
                        if (node.action != null && _pendingEdge != null) ConnectEdge(node);
                        else if(_pendingEdge != null) _pendingEdge = null;
                        break;
                }
            }
        if (openBaseMenu)
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1) OpenContextMenu(e.mousePosition);
                    if (_pendingEdge != null) _pendingEdge = null;
                    break;
            }
    }
    public void OpenContextMenu(Vector2 mousePosition)
    {
        GenericMenu genMenu = new GenericMenu();
        genMenu.AddItem(new GUIContent("Add Gate Node/AND"), false, () => AddGate(mousePosition, LogicGate.GateType.AND));
        genMenu.AddItem(new GUIContent("Add Gate Node/OR"), false, () => AddGate(mousePosition, LogicGate.GateType.OR));
        genMenu.AddItem(new GUIContent("Add Gate Node/NOT"), false, () => AddGate(mousePosition, LogicGate.GateType.NOT));
        genMenu.AddItem(new GUIContent("Add Gate Node/XAND"), false, () => AddGate(mousePosition, LogicGate.GateType.XAND));
        genMenu.AddItem(new GUIContent("Add Gate Node/XOR"), false, () => AddGate(mousePosition, LogicGate.GateType.XOR));
        genMenu.AddItem(new GUIContent("Add Gate Node/XNOT"), false, () => AddGate(mousePosition, LogicGate.GateType.XNOT));

        genMenu.ShowAsContext();
    }
    public void NodeContextMenu(Node node, Vector2 mousePosition)
    {

    }


    //Draw
    public void PaintNodes()
    {
        foreach(Node n in data.Nodes) n?.Paint();
    }
    public void PaintEdges()
    {

    }


    public void ConnectEdge(Node node)
    {

    }
    public void AddGate(Vector2 mousePosition, LogicGate.GateType gateType)
    {
        GameObject GO = new GameObject();
        GO.AddComponent<LogicGate>()._gateType = gateType;
        GO.name = gateType + "_Gate";

        Node gateNode = new Node(mousePosition, 80, 40, GO.GetComponent<LogicGate>(), GO.GetComponent<LogicGate>());
        if (data.Nodes == null) data.Nodes = new List<Node>();
        data.Nodes.Add(gateNode);
    }

}
//    private void PaintEdges()
//    {
//        if (_pendingEdge != null) _pendingEdge.Paint(Event.current.mousePosition);

//        if(_edges != null && _edges.Count > 0)
//            foreach(var edge in _edges)
//            {
//                edge.Paint();
//            }
//    }
//    private void NodeContextMenu(Vector2 pos, ActivableNode node)
//    {
//        GenericMenu genericMenu = new GenericMenu();

//        switch (NodeDetect(node))
//        {
//            case 1:
//            case 3:
//                //CREATE CONNECTION
//                genericMenu.AddItem(new GUIContent("Create Connection"), false, () => CreateEdge(node));
//                break;
//            case 2:
//            case 0:
//                //Nothing
//                break;
//        }

//        //DELETE NODE or DELETE CONNECTION

//        //CONNECTIONS
//        if(_edges != null && _edges.Count > 0)
//            for(int i = 0; i < _edges.Count; i++)
//            {
//                Edge edge = _edges[i];
//                if(edge._fromNode == node)
//                {
//                    // Get which kind of Node the other is to get the name 
//                    string Name = "";
//                    switch (NodeDetect(edge._toNode))
//                    {
//                        case 1:
//                            Name = ((TriggerNode)edge._toNode).GO.gameObject.name;
//                            break;
//                        case 2:
//                            Name = ((ActionNode)edge._toNode).GO.gameObject.name;
//                            break;
//                        case 3:
//                            Name = ((GateNode)edge._toNode).GO.gameObject.name;
//                            break;
//                        case 0:
//                            //Nothing
//                            break;
//                    }

//                    genericMenu.AddItem(new GUIContent("Delete Connection/Connection with " + Name), false, () => DeleteEdge(i));
//                }
//                else if (edge._toNode == node)
//                {
//                    // Get which kind of Node the other is to get the name 
//                    string Name = "";
//                    switch (NodeDetect(edge._fromNode))
//                    {

//                        case 1:
//                            Name = (edge._fromNode as TriggerNode).GO.gameObject.name;
//                            break;
//                        case 2:
//                            Name = (edge._fromNode as ActionNode).GO.gameObject.name;
//                            break;
//                        case 3:
//                            Name = (edge._fromNode as GateNode).GO.gameObject.name;
//                            break;
//                        case 0:
//                            //Nothing
//                            break;
//                    }

//                    genericMenu.AddItem(new GUIContent("Delete Connection/Connection with " + Name), false, () => DeleteEdge(i));
//                }
//            }

//        //NODE
//        genericMenu.AddItem(new GUIContent("Delete Node"), false, () => DeleteNode(node));
//        genericMenu.ShowAsContext();
//    }

//    private void OnAddGateNode(Vector2 pos, LogicGate.GateType type)
//    {
//        GameObject GO = new GameObject();
//        GO.AddComponent<LogicGate>()._gateType = type;
//        GO.name = type + "_Gate";

//        _gates.Add(GO.GetComponent<LogicGate>());
//        GateNode gn = new GateNode(pos, 50, 50, GO.GetComponent<LogicGate>());
//        if (_gatesNodes == null) _gatesNodes = new List<GateNode>();
//        _gatesNodes.Add(gn);
//    }

//    private void ConnectEdge(ActivableNode node)
//    {
//        switch (NodeDetect(node))
//        {
//            case 1:
//                //Nothing
//                break;
//            case 2:
//                //Connect
//                //SUBSCRIBE TONODE TO FROMNODE
//                if (_pendingEdge != null && _pendingEdge._fromNode != node)
//                {
//                    if (_edges == null) _edges = new List<Edge>();

//                    //Subscribe
//                    switch (NodeDetect(_pendingEdge._fromNode))
//                    {
//                        case 1:
//                            if ((_pendingEdge._fromNode as TriggerNode).GO._triggerAction == null) (_pendingEdge._fromNode as TriggerNode).GO._triggerAction = new UnityEngine.Events.UnityEvent<bool>();
//                            (_pendingEdge._fromNode as TriggerNode).GO._triggerAction.AddListener((node as ActionNode).GO.Activate);
//                            break;
//                        case 3:
//                            if ((_pendingEdge._fromNode as GateNode).GO._resultTrigger == null) (_pendingEdge._fromNode as GateNode).GO._resultTrigger = new UnityEngine.Events.UnityEvent<bool>();
//                            (_pendingEdge._fromNode as GateNode).GO._resultTrigger.AddListener((node as ActionNode).GO.Activate);
//                            break;

//                    }

//                    //Connect
//                    _pendingEdge._toNode = node;
//                    _edges.Add(_pendingEdge);
//                    _pendingEdge = null;
//                }
//                break;
//            case 3:
//                //Connect
//                //SUBSCRIBE TONODE TO FROMNODE
//                if(_pendingEdge != null && _pendingEdge._fromNode != node)
//                {
//                    if (_edges == null) _edges = new List<Edge>();

//                    //Subscribe
//                    switch (NodeDetect(_pendingEdge._fromNode))
//                    {
//                        case 1:
//                            if ((node as GateNode).GO._inputsT == null) (node as GateNode).GO._inputsT = new List<iActivTrigger>();
//                            (node as GateNode).GO._inputsT.Add((_pendingEdge._fromNode as TriggerNode).GO);
//                            break;
//                        case 3:
//                            if ((node as GateNode).GO._inputsL == null) (node as GateNode).GO._inputsL = new List<LogicGate>();
//                            (node as GateNode).GO._inputsL.Add((_pendingEdge._fromNode as GateNode).GO);
//                            break;

//                    }

//                    //Connect
//                    _pendingEdge._toNode = node;
//                    _edges.Add(_pendingEdge);
//                    _pendingEdge = null;
//                }
//                break;
//            case 0:
//                //Delete Connection
//                break;
//        }
//    }

//    //DETECT WHICH KIND OF NODE IT IS
//    private int NodeDetect(ActivableNode node)
//    {
//        int output = 0;
//        /*
//         * 1 = trigger
//         * 2 = action
//         * 3 = gate
//         */

//        if(_triggersNodes != null) foreach (TriggerNode tn in _triggersNodes) if (node == tn) output = 1;
//        if (_actionsNodes != null) foreach (ActionNode an in _actionsNodes) if (node == an) output = 2;
//        if (_gatesNodes != null) foreach (GateNode gn in _gatesNodes) if (node == gn) output = 3;

//        return output;
//    }

//    private void DeleteNode(ActivableNode node)
//    {
//        //REMEMBER TO MODIFY WITH THIS IN UPDATE TRIGGERS/ACTIONS/GATES

//        //Delete all Edges Attached to node
//        //Remove Node from relevant collection
//    }

//    private void DeleteEdge(int edgeIndex) //Int instead of Edge because RemveAt is Better than Remove
//    {
//        //Unsubscribe action from toNode to trigger of fromNode
//        //Remove edge from collection
//    }

//    private void CreateEdge(ActivableNode node)
//    {
//        _pendingEdge = new Edge(node);
//    }

//    private void Dirtyfy()
//    {
//        foreach (iActivTrigger t in _triggers) EditorUtility.SetDirty(t.gameObject);
//        foreach (iActivAction a in _actions) EditorUtility.SetDirty(a.gameObject);
//        foreach (LogicGate g in _gates) EditorUtility.SetDirty(g.gameObject);
//    }
//}

#endif
