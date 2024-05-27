using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovingPlatform : ActivableBase
{
    #region Variables

    [OverrideBaseActivableEditor]
    public List<Movement> movements;

    [OverrideBaseActivableEditor]
    public bool beginActive;
    [OverrideBaseActivableEditor]
    public bool shouldReverseOnEnd;

    [HideInInspector]
    public Vector3 frameStep;

    //Private Variables

    private Movement _activeMovement;
    private Vector3 _origin;
    private bool _active;
    private bool _keepActive;

    private Vector3 _lastFramePos;
    private Vector3 _thisFramePos;

    private int _currentIndex;

    private float _startTime;
    private float _timeStamp;
    private float _waitTimer;

    private bool _isReversed;

    #endregion

    #region Unity Functions

    public void FixedUpdate()
    {
        if (_active)
        {
            _lastFramePos = _thisFramePos;

            //Moving
            _timeStamp = ((Time.time - _startTime) * _activeMovement.speed) / Vector3.Distance(_activeMovement.destination, _origin);
            if(_timeStamp <= 1)transform.position = Vector3.Lerp(_origin, _activeMovement.destination, _timeStamp);

            //End Moving
            else
            {
                //Set Position
                if (transform.position != _activeMovement.destination) transform.position = _activeMovement.destination;

                //Wait
                if (_waitTimer <= 0) _waitTimer = _activeMovement.waitTime;
                _waitTimer -= Time.deltaTime;

                if(_waitTimer <= 0)
                {
                    //Keep Active
                    if (!_keepActive)
                    {
                        _active = false;
                    }
                    else
                    {
                        AdvanceIndex();
                        _origin = transform.position;
                        _activeMovement = movements[_currentIndex];
                        _startTime = Time.time;
                    }
                }  
            }

            //Frame Step
            _thisFramePos = transform.position;
            frameStep = _thisFramePos - _lastFramePos;
        }
    }

    #endregion

    #region Methods

    [ActivableAction]
    public void MoveTo(int index)
    {
        _origin = transform.position;
        if (movements != null && movements.Count > index && movements[index] != null) _activeMovement = movements[index];
        _active = true;
        _keepActive = false;
        _currentIndex = index;
        _startTime = Time.time;
    }

    [ActivableAction]
    public void Activate(bool isActive)
    {
        _origin = transform.position;
        if (movements != null && movements.Count > _currentIndex && movements[_currentIndex] != null) _activeMovement = movements[_currentIndex];
        _active = isActive;
        _keepActive = isActive;
        _startTime = Time.time;
    }

    [ActivableAction]
    public void MoveOnce(bool isActive)
    {
        _origin = transform.position;
        if (movements != null)
        {
            AdvanceIndex();
            _activeMovement = movements[_currentIndex];
        }
        _active = true;
        _keepActive = false;
        _startTime = Time.time;
    }

    private void AdvanceIndex()
    {
        if (shouldReverseOnEnd)
        {
            _currentIndex += _isReversed ? -1 : 1;
            if (_currentIndex <= 0 || _currentIndex >= movements.Count - 1) _isReversed = !_isReversed;
        }
        else
        {
            _currentIndex++;
            _currentIndex %= movements.Count;
        }
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        for(int i = 0; i < movements.Count; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(movements[i].destination, Vector3.one);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(movements[i].destination, movements[(i + 1) % movements.Count].destination);
        }
    }

    #endregion
}

[System.Serializable]
public class Movement
{
    public Vector3 destination;

    public float speed;

    public float waitTime;

    public Movement(Vector3 newDes, float newSp)
    {
        destination = newDes;
        speed = newSp;
    } 
}


