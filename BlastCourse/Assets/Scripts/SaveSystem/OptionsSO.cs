using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable, CreateAssetMenu(fileName = "OptionsData", menuName = "New Options")]
public class OptionsSO : ScriptableObject
{
    #region Fields

    public float Sensitivity;
    public float MasterVolume;
    public float SfxVolume;
    public float MusicVolume;
    public float DialogueVolume;
    public bool HoldToGrab;
    public bool Fullscreen;
    public bool ExtraHUD;
    public float CameraShake;
    public float Fov;
    [HideInInspector] public KeyCode[] Keys;
    

    #endregion
}

#if UNITY_EDITOR

[CustomEditor(typeof(OptionsSO))]
public class OptionsSOEditor : Editor
{
    OptionsSO opt;

    private void OnEnable()
    {
        opt = (OptionsSO)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (opt.Keys == null) opt.Keys = new KeyCode[Enum.GetValues(typeof(InputActions)).Length];
        for(int i = 0; i < opt.Keys.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(((InputActions)i).ToString());
            opt.Keys[i] = (KeyCode)EditorGUILayout.EnumPopup(opt.Keys[i]);

            EditorGUILayout.EndHorizontal();
        }
    }
}

#endif


