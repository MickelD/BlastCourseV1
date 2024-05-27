using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Activable Data", fileName = "New Activable Data")]
public class ActivableData : ScriptableObject
{
    #region Variables

    public List<Node> Nodes;

    public List<Edge> Edges;

    #endregion

    #region Methods

    #endregion
}
