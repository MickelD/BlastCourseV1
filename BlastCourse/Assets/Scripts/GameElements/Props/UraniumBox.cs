using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UraniumBox : PhysicsObject
{
    #region Vars

    private bool _consuming;
    public string id;

    #endregion

    #region UnityFunctions

    public override void OnDestroy()
    {
        if (!_consuming)
        {
            base.OnDestroy();
        }
    }

    #endregion

    #region Methods

    public void SetConsuming(bool consuming)
    {
        _consuming = consuming;
        if (!SaveLoader.Instance.UsedBoxes.Contains(id))
        {
            SaveLoader.Instance.UsedBoxes.Add(id);
            SaveLoader.Instance.Save();
        }
    }

    public string GetIndex() { return id; }
    public void SetIndex(string i) { id = i; }

    #endregion
}


