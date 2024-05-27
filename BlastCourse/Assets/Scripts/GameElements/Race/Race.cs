using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Race : MonoBehaviour
{
    #region Fields
    [Space(5), Header("Audio"), Space(3)]
    [SerializeField] AudioCue checkpointSound;
    [SerializeField] AudioCue startSound;
    [SerializeField] AudioCue finishSound;

    [HideInInspector] public GameObject g_player;
    [HideInInspector] public string _interactButtonName;

    [Space(5), Header("Checkpoint Values"), Space(3)]
    [HideInInspector] public GameObject g_startObject;
    [HideInInspector] public GameObject g_checkpointObject;
    [HideInInspector] public GameObject g_endObject;
    [HideInInspector] public GameObject g_creditObject;
    [HideInInspector] public int _numberOfCheckpoints;
    [HideInInspector] public int _creditsGiven;


    #endregion

    #region Variables

    private bool _isActive = false;
    private int _checkpointIndex = 0;

    [HideInInspector] public Start g_start;
    [HideInInspector] public CheckpointDEPRECATED[] g_checkpoints;
    [HideInInspector] public End g_end;
    [HideInInspector] public RaceCredits[] g_credits;

    [HideInInspector] public bool _trackCreated = false;

    #endregion

    #region Methods

    public void StartRace()
    {
        if (!_isActive)
        {
            AudioManager.TryPlayCueAtPoint(startSound, transform.position);
            _isActive = true;
            _checkpointIndex = 0;

            foreach(RaceCredits credit in g_credits)
            {
                credit.Activate(true);
            }

            g_player.GetComponent<Health>().SetRespawn(g_start.transform.position);

            RaceManager.Instance.SetRace(this);

            if (_numberOfCheckpoints > 0)
            {
                RaceManager.Instance.SetMarkerPosition(g_checkpoints[_checkpointIndex].transform.position);
                g_checkpoints[_checkpointIndex].Activate(true);
            }
            else
            {
                RaceManager.Instance.SetMarkerPosition(g_end.transform.position);
                g_end.Activate(true);
            }
        }
    }

    public void ReachCheckpoint()
    {
        AudioManager.TryPlayCueAtPoint(checkpointSound, transform.position);
        g_player.GetComponent<Health>().SetRespawn(g_checkpoints[_checkpointIndex].transform.position);

        _checkpointIndex++;
        if(_checkpointIndex >= g_checkpoints.Length)
        {
            RaceManager.Instance.SetMarkerPosition(g_end.transform.position);
            g_end.Activate(true);
        }
        else
        {
            RaceManager.Instance.SetMarkerPosition(g_checkpoints[_checkpointIndex].transform.position);
            g_checkpoints[_checkpointIndex].Activate(true);
        }
    }

    public void EndRace()
    {
        AudioManager.TryPlayCueAtPoint(finishSound, transform.position);
        g_player.GetComponent<Health>().SetRespawn(g_start.transform.position);
        g_start.Activate(true);

        foreach (RaceCredits credit in g_credits)
        {
            credit.Activate(false);
        }

        _checkpointIndex = 0;
        for (int i = 0; i < g_checkpoints.Length; i++)
        {
            g_checkpoints[i].Activate(false);
        }
        g_end.Activate(false);

        _isActive = false;
        RaceManager.Instance.SetRace(null);
    }

    public void LeaveRace()
    {
        g_start.Activate(true);

        foreach (RaceCredits credit in g_credits)
        {
            credit.Activate(false);
        }

        _checkpointIndex = 0;
        for(int i = 0; i < g_checkpoints.Length; i++)
        {
            g_checkpoints[i].Activate(false);
        }
        g_end.Activate(false);

        _isActive = false;
    }

    #endregion

    #region Debug

    private GameObject[] points;
    public void OnDrawGizmos()
    {
        if (_trackCreated)
        {
            if (points == null)
            {
                points = new GameObject[_numberOfCheckpoints + 2];
                points[0] = g_start.gameObject;
                for (int i = 0; i < _numberOfCheckpoints; i++)
                {
                    points[1 + i] = g_checkpoints[i].gameObject;
                }
                points[_numberOfCheckpoints + 1] = g_end.gameObject;
            }
            else
            {
                for (int i = 0; i <= points.Length; i++)
                {
                    if (i + 1 < points.Length)
                    {
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(points[i].transform.position, points[i + 1].transform.position);
                    }
                }
            }
        }
        else
        {
            points = null;
        }
    }

    #endregion
}
