using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RpgStats))]
public class StatEditor : Editor
{
    #region Other Variables

    RpgStats _stats;
    Editor _editor;
    SerializedProperty _explosion;
    GUIStyle style;

    #endregion

    #region Unity Fuctions

    private void OnEnable()
    {
        _stats = (RpgStats)target;
        style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = new Vector4(0.7f,0.7f,0.7f,1);
    }

    public override void OnInspectorGUI()
    {
        //EditorGUILayout.Space(5);
        //_stats.ClipSize = EditorGUILayout.FloatField("Clip Size", _stats.ClipSize);

        //EditorUtility.SetDirty(_stats);

        base.OnInspectorGUI();
    }

    #endregion
}
