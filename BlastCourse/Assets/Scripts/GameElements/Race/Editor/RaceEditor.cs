using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Race))]
[System.Serializable]
public class RaceEditor : Editor
{
    public Race race;

    public void Awake()
    {
        race = (Race)target;
    }

    public override void OnInspectorGUI()
    {

        GUIStyle style = new GUIStyle();

        style.richText = true;
        style.fontSize = 15;

        #region Variables

        race.g_player = RaceManager.Instance.g_player;
        race._interactButtonName = RaceManager.Instance._interactButtonName;
        race.g_startObject = RaceManager.Instance.g_startObject;
        race.g_checkpointObject = RaceManager.Instance.g_checkpointObject;
        race.g_endObject = RaceManager.Instance.g_endObject;
        race.g_creditObject = RaceManager.Instance.g_creditObject;

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Checkpoint Values", style);
        GUILayout.Space(5);

        int num = EditorGUILayout.IntField("Number of Checkpoints", race._numberOfCheckpoints);
        if(num != race._numberOfCheckpoints && race._trackCreated) Debug.LogError("Track already created, to change this value please delete the track first");
        else if(num >= 0) race._numberOfCheckpoints = num;
        else Debug.LogError("Number of checkpoints can't be negative");

        GUILayout.Space(25);
        EditorGUILayout.LabelField("Credits For Completing", style);
        GUILayout.Space(5);

        int credits = EditorGUILayout.IntField("Number of Credits", race._creditsGiven);
        if (credits >= 0) race._creditsGiven = credits;
        else Debug.LogError("Credits can't be negative");

        GUILayout.Space(30);
        #endregion

        GUILayout.BeginHorizontal();

        #region Genearte Track Points

        if (GUILayout.Button("Generate Track Points"))
        {
            if (race.g_startObject != null
            && race.g_endObject != null
            && (race._numberOfCheckpoints <= 0 || race.g_checkpointObject != null)
            && race.g_player != null
            && race._interactButtonName != ""
            && !race._trackCreated
            && (race._creditsGiven <= 0 || race.g_creditObject != null))
            {
                //Create the Start
                Start start = Instantiate(race.g_startObject, race.transform.position, Quaternion.identity, race.transform).GetComponent<Start>();
                start._race = race;
                start._buttonName = race._interactButtonName;
                start.g_player = race.g_player;
                race.g_start = start;

                //Create the Checkpoints
                if (race._numberOfCheckpoints > 0)
                {
                    CheckpointDEPRECATED checkpoint;
                    race.g_checkpoints = new CheckpointDEPRECATED[race._numberOfCheckpoints];
                    for (int i = 0; i < race._numberOfCheckpoints; i++)
                    {
                        checkpoint = Instantiate(race.g_checkpointObject, race.transform.position, Quaternion.identity,race.transform).GetComponent<CheckpointDEPRECATED>();
                        checkpoint._race = race;
                        checkpoint.g_player = race.g_player;
                        race.g_checkpoints[i] = checkpoint;
                    }
                }

                //Create the End
                End end = Instantiate(race.g_endObject, race.transform.position, Quaternion.identity, race.transform).GetComponent<End>();
                end._race = race;
                end.g_player = race.g_player;
                race.g_end = end;

                //Create Credits
                if (race._creditsGiven > 0)
                {
                    RaceCredits newCredits;
                    race.g_credits = new RaceCredits[race._creditsGiven];
                    for (int i = 0; i < race._creditsGiven; i++)
                    {
                        newCredits = Instantiate(race.g_creditObject, race.transform.position, Quaternion.identity, race.transform).GetComponent<RaceCredits>();
                        race.g_credits[i] = newCredits;
                    }
                }

                race._trackCreated = true;
                Debug.Log("Track Generated");
            }
            else if(!race._trackCreated)
            {
                Debug.LogError("Lacking one or more properties in " + race.gameObject.name);
            }
            else
            {
                Debug.LogWarning("Trying to create a track while a track already exists");
            }
        }

        #endregion

        #region Reset Track

        if(GUILayout.Button("Delete Track"))
        {
            if(race._trackCreated)
            {
                if(race.g_start != null)
                {
                    DestroyImmediate(race.g_start.gameObject);
                    race.g_start = null;
                }
                if (race.g_end != null)
                {
                    DestroyImmediate(race.g_end.gameObject);
                    race.g_end = null;
                }
                if (CheckForCheckpoints(race.g_checkpoints))
                {
                    for(int i = 0; i < race.g_checkpoints.Length; i++)
                    {
                        DestroyImmediate(race.g_checkpoints[i].gameObject);
                    }
                    race.g_checkpoints = null;
                }
                if (CheckForCredits(race.g_credits))
                {
                    for(int i = 0; i < race.g_credits.Length; i++)
                    {
                        DestroyImmediate(race.g_credits[i].gameObject);
                    }
                    race.g_credits = null;
                }

                race._trackCreated = false;
                Debug.Log("Race Deleted");
            }
            else
            {
                Debug.LogWarning("Trying to delete an empty race");
            }
        }

        #endregion

        GUILayout.EndHorizontal();

        //EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(race);
        base.OnInspectorGUI();
    }

    private bool CheckForCheckpoints(CheckpointDEPRECATED[] array)
    {
        bool value = false;
        foreach(CheckpointDEPRECATED checkpoint in array)
        {
            if(checkpoint != null)
            {
                value = true;
            }
        }

        return value;
    }

    private bool CheckForCredits(Credits[] array)
    {
        bool value = false;
        foreach (Credits credits in array)
        {
            if (credits != null)
            {
                value = true;
            }
        }

        return value;
    }
}
