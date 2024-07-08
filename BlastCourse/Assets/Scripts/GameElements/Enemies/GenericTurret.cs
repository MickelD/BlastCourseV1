using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GenericTurret : ActivableBase
{
    #region Fields
    [Space(5), Header("Components"), Space(3)]
    [SerializeField] public GameObject g_head;
    [SerializeField] public GameObject g_cannon;
    [SerializeField] public GameObject g_gunPoint;
    [SerializeField] public SphereCollider c_collider;
    [SerializeField] public LayerMask _aimingLayers;

    [Space(5), Header("Variables"), Space(3)]
    [SerializeField] public float _rotationSpeed;
    [SerializeField] public float _backRotationSpeed;
    public float _idleRotSpeed;
    [SerializeField] public float _shootAngle;
    [SerializeField] public float _rotationAngle;
    [SerializeField] public float _shootDelay;
    [SerializeField] public float _range;
    [SerializeField] public float _instantRange;
    [SerializeField] public Vector3 _offset;

    public Animator _animator;

    [Space(5), Header("Audio"), Space(3)]
    [SerializeField] public AudioCue shootSound;

    [Space(5), Header("Deactivation"), Space(3)]
    [SerializeField] public float _deactivationAnimDuration;
    [SerializeField] public float _angleDown;

    #endregion

    #region Variables
    protected bool _inRange;
    protected bool _inSight;
    protected float _shotTimer;
    protected float _startTime;
    protected float _startDeacRotation;

    protected Vector3 _headOrientation;
    protected Vector3 _cannonOrientation;

    protected Transform _target;
    protected List<Transform> _posibleTargets;

    #endregion

    #region UnityFunctions

    protected void OnValidate()
    {
        c_collider.center = g_head.transform.position - transform.position;
        c_collider.radius = _range;
    }

    protected override void Start()
    {
        base.Start();
        _posibleTargets = new List<Transform>();
        Activate(Active);
    }

    public virtual void Update()
    {
        //Checking for posible targets
        if(Active && _posibleTargets.Count > 0)
        {

            foreach(Transform t in _posibleTargets)
            {
                //Visibility && Angle of Missile
                if (t != null)
                {

                    Vector3 distance = t.transform.position + _offset - g_head.transform.position;
                    if (distance.magnitude <= _range && Physics.Raycast(g_gunPoint.transform.position, t.transform.position + _offset - g_gunPoint.transform.position, out RaycastHit hit, _range, _aimingLayers, QueryTriggerInteraction.Ignore))
                    {
                        if (RaycastCheck(hit.transform))
                        {
                            _target = t;

                            _inRange = true;
                            Vector3 playerDirection = t.transform.position + _offset - g_head.transform.position;
                            playerDirection.y = 0;
                            if (Mathf.Abs(Vector3.Angle(g_head.transform.forward, playerDirection)) <= _rotationAngle || distance.magnitude <= _instantRange)
                            {
                                _inSight = true;
                            }
                            else _inSight = false;
                        }
                        else _inRange = false;
                    }
                    else _inRange = false;
                }
                else
                {
                    _posibleTargets.RemoveAt(_posibleTargets.IndexOf(t));
                    return;
                }
            }
        }

        if (Active)
        {
            float currentRotSpeed = 0f;


            if (_target != null && _inRange) //shooting
            {

                _animator.SetInteger("state", 1);
                currentRotSpeed = _inSight ? _rotationSpeed : _backRotationSpeed;

                //Rotate Y Axis (Head)
                _headOrientation = (_target.transform.position + _offset - g_head.transform.position);
                _headOrientation.y = 0f;

                //Rotate X Axis (Cannon)
                _cannonOrientation = (_target.transform.position + _offset - g_cannon.transform.position);

                //Apply rotations
                g_head.transform.rotation = Quaternion.Slerp(g_head.transform.rotation, Quaternion.LookRotation(_headOrientation, transform.up), currentRotSpeed * Time.deltaTime);
                g_cannon.transform.localRotation = Quaternion.Euler(Vector3.Angle(_headOrientation, _cannonOrientation) * -Vector3.right);

                //Shoot
                if (_shotTimer <= 0f && Mathf.Abs(Vector3.Angle(g_head.transform.forward, _headOrientation)) <= _shootAngle)
                {
                    Shoot();
                }

                _shotTimer -= Time.deltaTime;
            }
            else
            {
                _animator.SetInteger("state", 0);

                //revert canon to neutral position
                if (g_cannon.transform.localEulerAngles.x != 0f)
                {
                    currentRotSpeed = _idleRotSpeed;
                    g_cannon.transform.localRotation = Quaternion.Slerp(g_cannon.transform.localRotation, Quaternion.identity, currentRotSpeed * Time.deltaTime);

                    if (g_cannon.transform.localEulerAngles.x <= 3f) g_cannon.transform.localEulerAngles = Vector3.zero;
                }
            }
        }
        else //deactivated
        {
            _animator.SetInteger("state", 2);
        }
    }

    #endregion

    #region Methods

    protected virtual void Shoot()
    {
        _shotTimer = _shootDelay;
        if(shootSound.SfxClip != null) AudioManager.TryPlayCueAtPoint(shootSound, Vector3.zero);
    }

    [ActivableAction]
    public void Activate(bool active)
    {
        Active = active;

        _startDeacRotation = g_cannon.transform.localEulerAngles.x;
        _startTime = Time.time;
    }

    protected void DeactivateAnimation()
    {
        if (Time.time <= _startTime + _deactivationAnimDuration)
        {
            float angle;
            if (_startDeacRotation <= 180) angle = Mathf.Lerp(_startDeacRotation, _angleDown, (Time.time - _startTime) / _deactivationAnimDuration);
            else angle = Mathf.Lerp(_startDeacRotation, _angleDown + 360, (Time.time - _startTime) / _deactivationAnimDuration);


            g_cannon.transform.localEulerAngles = Vector3.right * angle;
        }
    }

    protected void ActivateAnimation()
    {
        if (Time.time <= _startTime + _deactivationAnimDuration)
        {
            float angle;
            if (_startDeacRotation <= 180) angle = Mathf.Lerp(_startDeacRotation, 0, (Time.time - _startTime) / _deactivationAnimDuration);
            else angle = Mathf.Lerp(_startDeacRotation, 360, (Time.time - _startTime) / _deactivationAnimDuration);


            g_cannon.transform.localEulerAngles = Vector3.right * angle;
        }
    }

    protected virtual bool RaycastCheck(Transform obj)
    {
        bool check = false;

        //if (obj.tag.Equals("Rocket") || obj.tag.Equals("PhysicsBox")) check = true;

        return check;
    }

    #endregion

    #region Trigger&Collision

    protected virtual void OnTriggerStay(Collider other)
    {

        //var collidingRocket = other.GetComponent<RocketBase>();
        //var collidingBox = other.GetComponent<PhysicsObject>();
        //if (_target == null
        //    && collidingRocket != null
        //    && collidingRocket.GetComponent<RocketPipe>() == null)
        //{
        //    //Checks for Rockets

        //    _target = collidingRocket.transform;
        //    _targetingRocket = true;
        //}
        //else if (_target == null
        //    && collidingBox != null)
        //{
        //    //Checks for Box

        //    _target = collidingBox.transform;
        //    _targetingRocket = false;
        //}
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        //var collidingRocket = other.GetComponent<RocketBase>();
        //var collidingBox = other.GetComponent<PhysicsObject>();
        //if ((collidingRocket != null
        //    && _target == collidingRocket.transform)
        //    || (collidingBox != null
        //    && _target == collidingBox.transform))
        //{
        //    _target = null;
        //}

    }

    #endregion

    #region Debug
    protected void OnDrawGizmosSelected()
    {
        //Draw Range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(g_head.transform.position, _range);
        //Draw Instant Range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(g_head.transform.position, _instantRange);
        //Draw Angle
        Gizmos.DrawLine(g_head.transform.position, Quaternion.Euler(0, _rotationAngle, 0) * g_head.transform.forward * _range + g_head.transform.position);
        Gizmos.DrawLine(g_head.transform.position, Quaternion.Euler(0, -_rotationAngle, 0) * g_head.transform.forward * _range + g_head.transform.position);
        //Draw Shoot Range
        Gizmos.color = Color.red;
        Gizmos.DrawLine(g_head.transform.position, Quaternion.Euler(0, _shootAngle, 0) * g_head.transform.forward * _range + g_head.transform.position);
        Gizmos.DrawLine(g_head.transform.position, Quaternion.Euler(0, -_shootAngle, 0) * g_head.transform.forward * _range + g_head.transform.position);
    }

    #endregion
}


