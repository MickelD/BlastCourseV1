using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class RaceManager : MonoBehaviour
{
    #region Fields

    [Space(5), Header("Common Variables"), Space(3)]
    public GameObject g_player;
    public string _interactButtonName;
    public GameObject g_startObject;
    public GameObject g_checkpointObject;
    public GameObject g_endObject;
    public GameObject g_creditObject;

    [Space(5), Header("UI"), Space(3)]
    [SerializeField] Camera _camera;
    [SerializeField] HUDManager _hud;
    [SerializeField] Transform _cameraAngle;

    #endregion

    #region Variables

    public static RaceManager Instance;
    private Race _activeRace;
    private Vector3 _markerWorldPosition;
    private Vector3 _markerScreenPosition;
    private bool _showingMarker;

    #endregion

    #region UnityFunction

    public void Update()
    {
        if(Instance == null)Instance = this;

        if (_showingMarker && Vector3.Angle(_cameraAngle.forward,_markerWorldPosition - g_player.transform.position) < 90) 
        {
            _markerScreenPosition = _camera.WorldToScreenPoint(_markerWorldPosition);
            _hud.SetRaceMarker(_markerScreenPosition);
        }
        else
        {
            _hud.ActivateRaceMarker(false);
        }
        
    }

    #endregion

    #region Methods

    public void SetRace(Race _newRace)
    {
        if (_activeRace != null && _newRace != null) _activeRace.LeaveRace();
        _activeRace = _newRace;
        if (_activeRace == null)
        {
            _showingMarker = false;
            _hud.ActivateRaceMarker(false);
        }
    }

    public void SetMarkerPosition(Vector3 _realWorldPosition)
    {
        _markerWorldPosition = _realWorldPosition;
        _showingMarker = true;
    }

    #endregion
}
