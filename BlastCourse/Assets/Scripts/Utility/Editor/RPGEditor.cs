using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(RpgHolder))]
public class RPGEditor : Editor
{
    #region Foldout Bools

    List<bool> f_rpgList;
    List<bool> f_rpgStats;
    List<bool> f_rpgBehaviors;
    bool f_anim;
    bool f_aim;
    bool f_inputs;
    bool f_audio;
    bool f_rpgValues;
    bool f_list;

    #endregion

    #region Other Variables

    RpgHolder _rpgHolder;
    SerializedProperty _failSound;
    SerializedProperty _rechargeSound;
    SerializedProperty _layerMask;
    SerializedProperty _intangibleMask;
    Editor _editor;

    #endregion

    #region Unity Functions

    private void OnEnable()
    {
        _rpgHolder = (RpgHolder)target;

        _failSound = serializedObject.FindProperty("failSound");
        _rechargeSound = serializedObject.FindProperty("rechargeSound");
        _layerMask = serializedObject.FindProperty("_aimLayerMask");
        _intangibleMask = serializedObject.FindProperty(nameof(RpgHolder.IntangibleMask));

        f_rpgList = new List<bool>();
        f_rpgStats = new List<bool>();
        f_rpgBehaviors = new List<bool>();
        for(int i = 0; i < _rpgHolder._rpgList.Count; i++)
        {
            f_rpgList.Add(false);
            f_rpgStats.Add(false);
            f_rpgBehaviors.Add(false);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        #region Inputs

        EditorGUILayout.Space(5);
        f_inputs = EditorGUILayout.Foldout(f_inputs, "INPUTS");
        EditorGUILayout.Space(5);
        if (f_inputs)
        {
            EditorGUI.indentLevel = 1;

            _rpgHolder._primaryFireButtonName = EditorGUILayout.TextField("Primary Fire Button", _rpgHolder._primaryFireButtonName);
            EditorGUILayout.Space(2);
            _rpgHolder._secondaryFireButtonName = EditorGUILayout.TextField("Secondary Fire Button", _rpgHolder._secondaryFireButtonName);
            EditorGUILayout.Space(2);
            _rpgHolder._downaimButton = EditorGUILayout.TextField("Aim Down Button", _rpgHolder._downaimButton);
            EditorGUILayout.Space(2);
            _rpgHolder._classicButtonName = EditorGUILayout.TextField("Select Classic Button", _rpgHolder._classicButtonName);
            EditorGUILayout.Space(2);
            _rpgHolder._remoteButtonName = EditorGUILayout.TextField("Select Remote Button", _rpgHolder._remoteButtonName);
            EditorGUILayout.Space(2);
            _rpgHolder._pipeButtonName = EditorGUILayout.TextField("Select Pipe Button", _rpgHolder._pipeButtonName);
            EditorGUILayout.Space(2);
            _rpgHolder._homingButtonName = EditorGUILayout.TextField("Select Homing Button", _rpgHolder._homingButtonName);

            EditorGUI.indentLevel = 0;
            EditorGUILayout.Space(10);
        }

        #endregion

        #region Aiming

        EditorGUILayout.Space(5);
        f_aim = EditorGUILayout.Foldout(f_aim, "AIMING");
        EditorGUILayout.Space(5);
        if (f_aim)
        {
            EditorGUI.indentLevel = 1;

            _rpgHolder._maxAimDistance = EditorGUILayout.FloatField("Max Aim Range", _rpgHolder._maxAimDistance);
            EditorGUILayout.Space(2);
            _rpgHolder.g_camera = EditorGUILayout.ObjectField("Main Camera", _rpgHolder.g_camera, typeof(Camera), true) as Camera;
            EditorGUILayout.Space(2);
            _rpgHolder._rotation = EditorGUILayout.ObjectField("Player Rotation", _rpgHolder._rotation, typeof(PlayerRotation), true) as PlayerRotation;
            EditorGUILayout.Space(2);
            _rpgHolder._player = EditorGUILayout.ObjectField("Player Movement", _rpgHolder._player, typeof(PlayerMovement), true) as PlayerMovement;
            EditorGUILayout.Space(2);
            EditorGUILayout.PropertyField(_layerMask, true);

            _rpgHolder.IntangibleDistance = EditorGUILayout.FloatField("Intangible Distance", _rpgHolder.IntangibleDistance);
            EditorGUILayout.Space(2);

            EditorGUILayout.PropertyField(_intangibleMask, true);
            EditorGUILayout.Space(2);

            EditorGUI.indentLevel = 0;
            EditorGUILayout.Space(10);
        }

        #endregion

        #region RPG Values

        EditorGUILayout.Space(5);
        f_rpgValues = EditorGUILayout.Foldout(f_rpgValues, "RPG VALUES");
        EditorGUILayout.Space(5);
        if (f_rpgValues)
        {
            EditorGUI.indentLevel = 1;

            _rpgHolder._fireOrigin = EditorGUILayout.ObjectField("Rocket Origin", _rpgHolder._fireOrigin, typeof(Transform), true) as Transform;
            EditorGUILayout.Space(2);
            _rpgHolder._fireMode = (FiringMode)EditorGUILayout.EnumPopup("RPG Selected", _rpgHolder._fireMode);
            EditorGUILayout.Space(2);
            _rpgHolder._energyRecoveryRate = EditorGUILayout.CurveField("Recovery Rate", _rpgHolder._energyRecoveryRate);
            EditorGUILayout.Space(2);
            _rpgHolder._fireSpeed = EditorGUILayout.FloatField("Fire Speed", _rpgHolder._fireSpeed);
            EditorGUILayout.Space(2);
            _rpgHolder.g_HUD = EditorGUILayout.ObjectField("HUD", _rpgHolder.g_HUD, typeof(HUD), true) as HUD;

            EditorGUI.indentLevel = 0;
            EditorGUILayout.Space(10);
        }

        #endregion

        #region RPG List

        /*
         RPG EDITOR COLORS
            CLASSIC: e5e1cf
            PIPE: cfe2e5
            TARGETED: e5cfcf
            REMOTE: cfe4d4
            HOMING: d7cee4
         */

        EditorGUILayout.Space(5);
        f_list = EditorGUILayout.Foldout(f_list, "RPG LIST");
        EditorGUILayout.Space(5);
        if (f_list)
        {
            EditorGUI.indentLevel = 1;

            //List
            for (int i = 0; i < _rpgHolder._rpgList.Count; i++)
            {
                if (_rpgHolder._rpgList[i]._rpgStats != null)
                {
                    Color guiColor = _rpgHolder._rpgList[i]._rpgStats.AssociatedVisuals.LightColor;
                    guiColor.a = 1f;
                    GUI.color = guiColor;
                }
                else { GUI.color = Color.white; }

                f_rpgList[i] = EditorGUILayout.Foldout(f_rpgList[i], _rpgHolder._rpgFiringMode[i].ToString().ToUpper());

                if (f_rpgList[i])
                {
                    EditorGUI.indentLevel = 2;

                    EditorGUILayout.LabelField("___________________________________________________________________________________________________________");
                    //Firing Mode
                    _rpgHolder._rpgFiringMode[i] = (FiringMode)EditorGUILayout.EnumPopup("Firing Mode", _rpgHolder._rpgFiringMode[i]);
                    if (i > 0) for (int j = i - 1; j > 0; j--)
                            if (_rpgHolder._rpgFiringMode[i] == _rpgHolder._rpgFiringMode[j])
                                Debug.LogError("Two RPGs with the same Firing Mode have been detected. Please change or remove one of them.");
                    EditorGUILayout.Space(8);

                    //Behavior
                    _rpgHolder._rpgList[i]._rpgBehaviour = EditorGUILayout.ObjectField("Behavior", _rpgHolder._rpgList[i]._rpgBehaviour, typeof(RpgBase), true) as RpgBase ;
                    if (_rpgHolder._rpgList[i]._rpgBehaviour != null)
                    {
                        f_rpgBehaviors[i] = EditorGUILayout.Foldout(f_rpgBehaviors[i], "BEHAVIOR DATA");
                        if (f_rpgBehaviors[i])
                        {

                            EditorGUI.indentLevel = 3;

                            CreateCachedEditor(_rpgHolder._rpgList[i]._rpgBehaviour, null, ref _editor);
                            _editor.DrawDefaultInspector();
                            //EditorUtility.SetDirty(_rpgHolder._rpgList[i]._rpgBehaviour);

                            EditorGUI.indentLevel = 2;
                        }
                    }
                    else Debug.LogError("Behavior of the " + _rpgHolder._rpgFiringMode[i] + " RPG is missing. Please add a behavior.");
                    EditorGUILayout.Space(8);


                    //Stats
                    _rpgHolder._rpgList[i]._rpgStats = EditorGUILayout.ObjectField("Stats", _rpgHolder._rpgList[i]._rpgStats, typeof(RpgStats), true) as RpgStats;
                    if (_rpgHolder._rpgList[i]._rpgStats != null)
                    {
                        f_rpgStats[i] = EditorGUILayout.Foldout(f_rpgStats[i], "STATS DATA");
                        if (f_rpgStats[i])
                        {
                            EditorGUI.indentLevel = 3;

                            CreateCachedEditor(_rpgHolder._rpgList[i]._rpgStats, null, ref _editor);
                            _editor.OnInspectorGUI();
                            //EditorUtility.SetDirty(_rpgHolder._rpgList[i]._rpgStats);


                            EditorGUI.indentLevel = 2;
                        }
                    }
                    else Debug.LogError("Stats of the " + _rpgHolder._rpgFiringMode[i] + " RPG is missing. Please add a stats.");

                    EditorGUILayout.LabelField("___________________________________________________________________________________________________________");
                    EditorGUILayout.Space(12);

                }
            }

            GUI.color = Color.white;
            EditorGUI.indentLevel = 0;

            //Add and Remove from List
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                _rpgHolder._rpgList.Add(new RpgData());
                _rpgHolder._rpgFiringMode.Add(FiringMode.Classic);

                f_rpgList.Add(true);
                f_rpgStats.Add(false);
                f_rpgBehaviors.Add(false);
            }
            if (GUILayout.Button("Remove"))
            {
                _rpgHolder._rpgList.RemoveAt(_rpgHolder._rpgList.Count - 1);
                _rpgHolder._rpgFiringMode.RemoveAt(_rpgHolder._rpgFiringMode.Count - 1);

                f_rpgList.RemoveAt(f_rpgList.Count - 1);
                f_rpgStats.RemoveAt(f_rpgStats.Count - 1);
                f_rpgBehaviors.RemoveAt(f_rpgBehaviors.Count - 1);
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
        }

        #endregion

        #region Audio

        EditorGUILayout.Space(5);
        f_audio = EditorGUILayout.Foldout(f_audio, "AUDIO");
        EditorGUILayout.Space(5);
        if (f_audio)
        {
            EditorGUI.indentLevel = 1;

            EditorGUILayout.PropertyField(_failSound, true);
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(_rechargeSound, true);

            EditorGUI.indentLevel = 0;
        }

        #endregion

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(_rpgHolder);
    }

    #endregion
}


#endif