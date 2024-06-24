using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoxCheckpoint : BoxVisualizer
{
    private List<string> _passedIds =  new List<string>();

    #region UnityFunctions

    private void OnTriggerEnter(Collider other)
    {
        UraniumBox b = other.GetComponent<UraniumBox>();

        if (b == null) return;
        //if (_passedIds.Count > 0 && _passedIds.Contains(b.id)) return;

        //_passedIds.Add(b.id);
        SaveLoader.Instance.SetBoxPos(b);
    }

    #endregion

    #region Methods

    protected override Color GetColor()
    {
        return new Color(0f, 1f, 1f, 0.5f);
    }

    #endregion

}


