using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Tips&Texts", menuName = "Tips&Texts/New")]
public class LoadingScreenTextSO : ScriptableObject
{
    #region Fields

    public string[] Tips;

    #endregion
}


