using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoxCheckpoint : BoxVisualizer
{
    #region Fields



    #endregion

    #region Vars



    #endregion

    #region UnityFunctions

    private void OnTriggerEnter(Collider other)
    {
        UraniumBox b = other.GetComponent<UraniumBox>();
        if (b != null) SaveLoader.Instance.SetBoxPos(b);
    }

    #endregion

    #region Methods

    protected override Color GetColor()
    {
        return new Color(0f, 1f, 1f, 0.5f);
    }

    #endregion

#if UNITY_EDITOR



#endif
}


