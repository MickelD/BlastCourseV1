using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

[CustomEditor(typeof(KeyObjManager))]
public class KeyObjManagerEditor : Editor
{
    KeyObjManager _manager;

    private void OnEnable()
    {
        _manager = (KeyObjManager)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("KEY INTERACTABLES");

        EditorGUILayout.Space(5);

        if (_manager != null && _manager._keyObj != null && _manager._keyObj.Count > 0)
        {
            for (int i = 0; i < _manager._keyObj.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_manager._keyObj[i]._index);
                EditorGUILayout.ObjectField(_manager._keyObj[i], typeof(KeyInteractable), true);
                EditorUtility.SetDirty(_manager._keyObj[i]);
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space(5);

        if (GUILayout.Button("Search for Key Interactables"))
        {
            Debug.Log("Phase 1");

            _manager._keyObj = new List<KeyInteractable>();

            KeyInteractable[] coll = (KeyInteractable[])FindObjectsOfType(typeof(KeyInteractable));
            Debug.Log("Phase 2");
            if (coll.Length > 0)
            {
                Debug.Log("Phase 3");
                for (int i = 0; i < coll.Length; i++)
                {
                    _manager._keyObj.Add(coll[i]);
                    coll[i]._index = SceneManager.GetActiveScene().buildIndex + "s" + (i + 1) + "k";
                }
                Debug.Log("Phase 4");
            }
            Debug.Log("Phase End");

            Debug.Log(_manager._keyObj.Count);
        }

        EditorUtility.SetDirty(_manager);
    }
}

