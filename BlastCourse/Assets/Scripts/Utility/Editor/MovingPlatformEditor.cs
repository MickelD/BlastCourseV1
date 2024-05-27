using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : ActivableEditor
{
    MovingPlatform platform;
    GUIStyle style;
    bool useLocal;

    private void OnEnable()
    {
        platform = (MovingPlatform)target;
        style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
    }

    public override void OnInspectorGUI()
    {
        #region Positions

        //Positions
        EditorGUILayout.LabelField("Positions", style);
        EditorGUILayout.LabelField("___________________________________________________________________________________________________________",style);
        GUILayout.Space(5);
        if (platform.movements == null)
        {
            platform.movements = new List<Movement>();
            platform.movements.Add(new Movement(platform.transform.position, 0));
            platform.movements.Add(new Movement(platform.transform.position, 0));
        }
        for(int i = 0; i < platform.movements.Count; i++)
        {
            EditorGUI.indentLevel = 1;
            EditorGUILayout.LabelField("Position " + i);
            GUILayout.Space(5);

            // Destination
            GUILayout.BeginHorizontal();
            if (useLocal)
            {
                EditorGUILayout.LabelField("X", GUILayout.Width(30));
                platform.movements[i].destination.x = EditorGUILayout.FloatField(platform.movements[i].destination.x - platform.transform.position.x, GUILayout.Width(70)) + platform.transform.position.x;
                EditorGUILayout.LabelField("Y", GUILayout.Width(30));
                platform.movements[i].destination.y = EditorGUILayout.FloatField(platform.movements[i].destination.y - platform.transform.position.y, GUILayout.Width(70)) + platform.transform.position.y;
                EditorGUILayout.LabelField("Z", GUILayout.Width(30));
                platform.movements[i].destination.z = EditorGUILayout.FloatField(platform.movements[i].destination.z - platform.transform.position.z, GUILayout.Width(70)) + platform.transform.position.z;
            }
            else
            {
                EditorGUILayout.LabelField("X", GUILayout.Width(30));
                platform.movements[i].destination.x = EditorGUILayout.FloatField(platform.movements[i].destination.x, GUILayout.Width(70));
                EditorGUILayout.LabelField("Y", GUILayout.Width(30));
                platform.movements[i].destination.y = EditorGUILayout.FloatField(platform.movements[i].destination.y, GUILayout.Width(70));
                EditorGUILayout.LabelField("Z", GUILayout.Width(30));
                platform.movements[i].destination.z = EditorGUILayout.FloatField(platform.movements[i].destination.z, GUILayout.Width(70));
            }
            GUILayout.EndHorizontal();

            // Speed && Advanced Speed
            platform.movements[i].speed = EditorGUILayout.FloatField("Speed", platform.movements[i].speed);
            if(platform.movements[i].speed < 0.01f)
            {
                platform.movements[i].speed = 0.01f;
                Debug.LogWarning("Speed must be a positive number");
            } 
            GUILayout.Space(5);
            platform.movements[i].waitTime = EditorGUILayout.FloatField("Wait Time", platform.movements[i].waitTime);
            if (platform.movements[i].waitTime < 0)
            {
                platform.movements[i].waitTime = 0;
                Debug.LogWarning("Wait Time must be a positive number");
            }
            GUILayout.Space(5);
            EditorGUI.indentLevel = 0;
            EditorGUILayout.LabelField("___________________________________________________________________________________________________________", style);
            GUILayout.Space(5);
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            Movement m = new Movement(platform.transform.position, 0);
            platform.movements.Add(m);
        }
        if (GUILayout.Button("Remove"))
        {
            platform.movements.RemoveAt(platform.movements.Count - 1);
            if(platform.movements.Count < 2)
            {
                Debug.LogWarning("Platforms must have at least 2 positions");
                Movement m = new Movement(platform.transform.position, 0);
                platform.movements.Add(m);
            }
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("___________________________________________________________________________________________________________", style);
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Use Local Position");
        useLocal = EditorGUILayout.Toggle(useLocal);
        GUILayout.EndHorizontal();
        GUILayout.Space(15);

        EditorGUILayout.HelpBox("Remember to keep the first position unchanged.", MessageType.Info);
        GUILayout.Space(30);

        #endregion

        #region Other

        //Other Variables
        platform.beginActive = EditorGUILayout.Toggle("Begin Active", platform.beginActive);
        GUILayout.Space(2);
        platform.shouldReverseOnEnd = EditorGUILayout.Toggle("Should Reverse On End", platform.shouldReverseOnEnd);
        GUILayout.Space(15);

        #endregion

        EditorUtility.SetDirty(platform);

        base.OnInspectorGUI();
    }
}
