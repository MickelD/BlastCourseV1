using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
using UnityEditor;
#endif

public class BoxManager : MonoBehaviour
{
    #region Fields

    public List<UraniumBox> Boxes;

    #endregion

    #region Vars



    #endregion

    #region UnityFunctions

    public void Start()
    {
        if (Boxes != null)
        {
            for (int i = 0; i < SaveLoader.Instance.Boxes.Count; i++)
            {
                foreach (UraniumBox b in Boxes)
                    if (
                        !SaveLoader.Instance.UsedBoxes.Contains(SaveLoader.Instance.Boxes[i])
                        && SaveLoader.Instance.Boxes[i] == b.GetIndex())
                        b.transform.position = SaveLoader.Instance.GetBoxPos(i);
            }

            for (int i = 0; i < SaveLoader.Instance.UsedBoxes.Count; i++)
            {
                foreach (UraniumBox b in Boxes)
                    if (SaveLoader.Instance.UsedBoxes[i] == b.GetIndex())
                    {
                        Debug.Log(b.GetIndex());
                        b.SetConsuming(true);
                        Destroy(b.gameObject,Time.deltaTime);
                    }
            }
        }
            
    }

    #endregion

    #region Methods



    #endregion

#if UNITY_EDITOR

    [CustomEditor(typeof(BoxManager))]
    class BoxManagerEditor : Editor
    {
        BoxManager m;

        private void OnEnable()
        {
            m = (BoxManager)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("KEY INTERACTABLES");

            EditorGUILayout.Space(5);

            if (m != null && m.Boxes != null && m.Boxes.Count > 0)
            {
                for (int i = 0; i < m.Boxes.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(m.Boxes[i].GetIndex());
                    EditorGUILayout.ObjectField(m.Boxes[i], typeof(UraniumBox), true);
                    if(m.Boxes[i] != null) EditorUtility.SetDirty(m.Boxes[i]);
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space(5);

            if (GUILayout.Button("Search for Key Interactables"))
            {
                Debug.Log("Phase 1");

                m.Boxes = new List<UraniumBox>();

                UraniumBox[] coll = (UraniumBox[])FindObjectsOfType(typeof(UraniumBox));
                Debug.Log("Phase 2");
                if (coll.Length > 0)
                {
                    Debug.Log("Phase 3");
                    for (int i = 0; i < coll.Length; i++)
                    {
                        m.Boxes.Add(coll[i]);
                        coll[i].SetIndex(SceneManager.GetActiveScene().buildIndex + "s" + (i + 1) + "k");
                        EditorUtility.SetDirty(coll[i]);
                    }
                    Debug.Log("Phase 4");
                }
                Debug.Log("Phase End");

                Debug.Log(m.Boxes.Count);
            }

            EditorUtility.SetDirty(m);
        }
    }

    #endif
}


