using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CollectibleManager))]
public class CollectManagerEditor : Editor
{
    CollectibleManager _collector;

    private void OnEnable()
    {
        _collector = (CollectibleManager)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("COLLECTIBLES");

        EditorGUILayout.Space(5);

        if(_collector?._collectibles?.Count > 0)
        {
            for (int i = 0; i < _collector._collectibles.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(_collector._collectibles[i]._index);
                EditorGUILayout.ObjectField(_collector._collectibles[i], typeof(Collectibles), true);
                _collector._collectibles[i]._collected = EditorGUILayout.Toggle(_collector._collectibles[i]._collected);
                EditorUtility.SetDirty(_collector._collectibles[i]);
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.Space(5);

        if(GUILayout.Button("Search for Collectibles"))
        {
            Debug.Log("Phase 1");

            _collector._collectibles = new List<Collectibles>();

            Collectibles[] coll = (Collectibles[])FindObjectsOfType(typeof(Collectibles));
            Debug.Log("Phase 2");
            if (coll.Length > 0)
            {
                Debug.Log("Phase 3");
                for (int i = 0; i < coll.Length; i++)
                {
                    _collector._collectibles.Add(coll[i]);
                    coll[i]._index = SceneManager.GetActiveScene().buildIndex + "s" + (i+1) + "n";
                }
                Debug.Log("Phase 4");
            }
            Debug.Log("Phase End");

            Debug.Log(_collector._collectibles.Count);
        }

        EditorUtility.SetDirty(_collector);
    }
}
