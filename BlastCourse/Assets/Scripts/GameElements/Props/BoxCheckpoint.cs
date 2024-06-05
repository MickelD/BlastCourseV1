using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoxCheckpoint : MonoBehaviour
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



    #endregion

#if UNITY_EDITOR



#endif
}


