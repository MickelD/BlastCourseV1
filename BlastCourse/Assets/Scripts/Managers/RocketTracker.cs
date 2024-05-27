using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CustomMethods;

public class RocketTracker : MonoBehaviour
{
    #region Fields

    [Space(5), Header("Conditions"), Space(3)]
    [SerializeField] Transform _transform;
    [Tooltip("Radius of the SphereCast"), SerializeField] float _minObstacleSize;
    [SerializeField, Range(-1, 1f)] float _minDotProduct;
    [Tooltip("Only account for rockets withing this radius"), SerializeField] float _maxDistance;
    [Tooltip("Used to determine FOV"), SerializeField] private Camera _cam;

    [Space(5), Header("Overlap Operation Specifications"), Space(3)]
    [SerializeField] LayerMask _rocketsLayer;
    [SerializeField] LayerMask _obstacleLayer;
    [SerializeField] int _bufferSize;
    [Tooltip("If this array is empty, Overlap Operation will be performed each frame. Values beyond the first index are ignored"), SerializeField] float[] _operationInterval;

    #endregion

    #region Variables

    Collider[] _hitColliders;

    RocketBase _closestIncomingRocket;
    RocketBase _previousIncomingRocket;

    WaitForSecondsRealtime _operationDelay;

    bool _wasNulled;

    #endregion


    #region UnityFunctions

    private void Awake()
    {
        _hitColliders = new Collider[_bufferSize];

        if (_operationInterval.Length == 0)
        {
            StartCoroutine(UpdateEveryFrame());
        }
        else
        {
            _operationDelay = new WaitForSecondsRealtime(_operationInterval[0]);
            StartCoroutine(UpdateAtFixedIntervals());
        }
    }

    #endregion

    #region Methods

    private IEnumerator UpdateEveryFrame()
    {
        while (gameObject.activeInHierarchy)
        {
            if (EventManager.RocketCount > 0)
            {
                FindClosestIncomingRocket();
            }

            yield return null;
        }
    }

    private IEnumerator UpdateAtFixedIntervals()
    {
        while (gameObject.activeInHierarchy)
        {
            if (EventManager.RocketCount > 0)
            {
                FindClosestIncomingRocket();
            }

            yield return _operationDelay;
        }
    }

    private void FindClosestIncomingRocket()
    {
        int numColliders = Physics.OverlapSphereNonAlloc(_transform.position, _maxDistance, _hitColliders, _rocketsLayer, QueryTriggerInteraction.Ignore);
        float smallestDistanceSquared = _maxDistance * _maxDistance;

        _closestIncomingRocket = null;

        for (int i = 0; i < numColliders; i++)
        {
            RocketBase rocket = _hitColliders[i].GetComponent<RocketBase>();

            if (rocket == null) continue; //if rocket is not valid, abort
            if (ExtendedDataUtility.IsPointOnCamera(rocket.transform.position, _cam)) continue; //if rocket is on FOV, abort

            //Rocket must not intersect with any object that is large enough
            bool lineCheck = Physics.Linecast(rocket.transform.position, _transform.position, out RaycastHit hitInfo, _obstacleLayer, QueryTriggerInteraction.Ignore);
            if (lineCheck) lineCheck = hitInfo.collider.bounds.size.x > _minObstacleSize && hitInfo.collider.bounds.size.y > _minObstacleSize && hitInfo.collider.bounds.size.z > _minObstacleSize;
            if (lineCheck) continue;

            //rocket must be moving somewhat towards us
            if(Vector3.Dot((_transform.position - rocket.transform.position).normalized, rocket.Body.velocity.normalized) < _minDotProduct) continue;

            float d = Vector3.SqrMagnitude(_transform.position - rocket.transform.position);

            if (d <= smallestDistanceSquared)
            {
                smallestDistanceSquared = d;
                _closestIncomingRocket = rocket;
                _wasNulled = false;
            }
            
        }

        //should only update values when we have detected a different rocket
        if (_closestIncomingRocket != _previousIncomingRocket)
        {
            _previousIncomingRocket = _closestIncomingRocket;
            EventManager.OnUpdateClosestIncomingRocket?.Invoke(_closestIncomingRocket);
        }
        else if (_closestIncomingRocket == null && !_wasNulled)
        {
            _wasNulled = true;
            EventManager.OnUpdateClosestIncomingRocket?.Invoke(_closestIncomingRocket);
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(transform.position, _maxDistance);
    }
}

/*  I am the code hoarder hehe, I hoard code when I wanna try new things because I am afraid I will forget what I had done previosuly
 *             if (rocket != null
                && !ExtendedDataUtility.IsPointOnCamera(rocket.transform.position, _cam) && (Physics.SphereCast(new Ray(rocket.transform.position, rocket.transform.forward), _collisionPredictionRadius, _maxDistance, _playerLayer, QueryTriggerInteraction.Ignore)
                    || (Physics.Raycast(rocket.transform.position, rocket.Body.velocity.normalized, _collisionPredictionRadius, _playerLayer, QueryTriggerInteraction.Ignore)
                        && Vector3.Dot(transform.position - rocket.transform.position, rocket.Body.velocity) > -0.1f))
 * */