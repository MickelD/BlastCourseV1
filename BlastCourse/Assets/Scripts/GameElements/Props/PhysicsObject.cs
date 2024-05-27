using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMethods;
using DG.Tweening;
using System.Linq;

[RequireComponent(typeof(Rigidbody), typeof(GravityController))]
public class PhysicsObject : ScaledTimeMonoBehaviour, IBounceable, IExplodable, IInteractable, IMagnetable
{
    #region Fields

    [Space(5), Header("Properties"), Space(3)]
    [Tooltip("Should the object respawn when it exits a Spawner's Bound Box or it gets destroyed")] public bool ShouldRespawn;
    [DrawIf(nameof(ShouldRespawn), true),
    Tooltip("Area where the object can be, leave empty for an object that can go anywhere")] 
    public ObjectSpawner Spawner;

    [Tooltip("Affected by Magnetic Plates"), SerializeField] bool _magnetable;
    [DrawIf(nameof(_magnetable), true), SerializeField] bool _canBeGrabbedIfMagnetized;
    [DrawIf(nameof(_magnetable), true), SerializeField] float _demagnetizationImpulse;

    [Tooltip("Can be picked up by player"), SerializeField] bool _canBeGrabbed;
    [DrawIf(nameof(_canBeGrabbed), true), SerializeField] string _idleLayerName;
    [DrawIf(nameof(_canBeGrabbed), true), SerializeField] string _grabbedLayerName;
    [DrawIf(nameof(_canBeGrabbed), true), SerializeField] LayerMask _obstacleLayerMask;
    [DrawIf(nameof(_canBeGrabbed), true), SerializeField] float _grabForceMultiplier = 1f;
    [DrawIf(nameof(_canBeGrabbed), true), SerializeField, Tooltip("The vertical impulse added to this object when we stop grabbing it")] float _dropVerticalImpulse;
    [DrawIf(nameof(_canBeGrabbed), true), SerializeField] float _grabRotationSpeed;
    [DrawIf(nameof(_canBeGrabbed), true), SerializeField, Tooltip("How much of our Velocity is kept when thrown")] float _throwInertia;
    [DrawIf(nameof(_canBeGrabbed), true), SerializeField, Tooltip("The distance at which the box will attemtpt to remain")] float _minHoldDistance = 0.1f;
    [DrawIf(nameof(_canBeGrabbed), true), SerializeField, Tooltip("The distance at which the box will be dropped")] float _maxHoldDistance = 10f;

    [Space(5), Header("Explosion Values"), Space(3)]
    [Tooltip("Applied only to explosion impulses"), SerializeField] float _explosionMass;
    [Tooltip("Multiple of force to add as rotational speed"), SerializeField] float _rotationalInertia;
    [SerializeField] float _bouncePadMultiplier;

    [Space(5), Header("Components"), Space(3)]
    [SerializeField] Rigidbody c_rb;
    [SerializeField] Collider c_collider;
    [SerializeField] GravityController c_gravity;

    #endregion

    #region Variables
    public bool Locked { get { return !_canBeGrabbed; } set { _canBeGrabbed = !value; } }

    private bool _alreadyRespawned;
    private bool _grabbed;
    private bool _magnetized;
    float _angleOffset;
    float _distanceFromPlayer;
    private MagneticPlate _currentMagnet;
    private PlayerInteract _currentInteractor;
    private float _normalDrag;
    public float _stuckDrag = 10f;
    private PhysicMaterial _phyMat;

    private System.Func<Quaternion> DesiredRotation;
    private float _closestDot;
    private Vector3[] _transformDirections;

    private bool _frozen;

    private bool _isQuitting;

    private RaycastHit _ground;
    private bool _grounded;

    private Vector3 _realScale;
    private Vector3 _localScale;
    private Vector3 _lastValidMagnetDir;

    public System.Action OnObjectDestroyed;

    #endregion

    #region Unity Functions

    protected override void Start()
    {
        base.Start();

        if (ShouldRespawn && Spawner != null) Spawner.SpawnedObject = this;

        _normalDrag = c_rb.drag;
        _phyMat = c_collider.material;
        _transformDirections = new Vector3[] { transform.right, transform.up, transform.forward};

        _realScale = transform.lossyScale;
        _localScale = transform.localScale;

        SaveLoader.Instance._loading = false;
    }

    private void FixedUpdate()
    {
        if (_magnetized && _currentMagnet != null) MagnetAction();
        else if (_grabbed && _currentInteractor != null) GrabAction();
        
        if(_currentInteractor == null
            && !_grounded && Physics.Raycast(transform.position,
                                            Vector3.down,
                                            out _ground,
                                            LayerMask.NameToLayer(_idleLayerName))
            && Vector3.Distance(transform.position, _ground.point) <= 0.51f
            && !_ground.transform.GetComponent<PhysicsObject>())
        {
            ParentToGround(true);
        }

        //if (Spawner != null) CheckBoundBox();
    }

    public virtual void OnDestroy()
    {
        if (!_isQuitting && !SaveLoader.Instance._loading) //VERIFY THAT WE ARE NOT CHANGING SCENES
        {
            OnObjectDestroyed?.Invoke();

            if (_currentInteractor != null) _currentInteractor.SetInteractWith(this, false);

            if (ShouldRespawn && Spawner != null && !_alreadyRespawned)
            {
                _alreadyRespawned = true;
                Spawner.SpawnedObject = null;
                Spawner.TrySpawn(true);
            }
        }
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    #endregion

    #region Methods

    #region Updates

    private void MagnetAction()
    {
        c_rb.AddForce(-_currentMagnet.PullStrenght * _stuckDrag *  _currentMagnet.transform.forward, ForceMode.Acceleration);
    }

    private void GrabAction()
    {
        _distanceFromPlayer = Vector3.Distance(transform.position, _currentInteractor.GrabAnchor.position);
        if (_distanceFromPlayer > _minHoldDistance)
        {
            Vector3 moveDir = (_currentInteractor.GrabAnchor.position - transform.position);
            c_rb.AddForce(_currentInteractor.GrabForce * _grabForceMultiplier * _stuckDrag * moveDir, ForceMode.Acceleration);

            //transform.Rotate(Vector3.up, _angleOffset, Space.World);

            //transform.rotation = Quaternion.AngleAxis(_currentInteractor.PointerTransform.eulerAngles.y * Mathf.Sign(_closestDot), Vector3.up) * pickRot;

            //if (DesiredRotation != null)
            //transform.rotation = DesiredRotation();
            //transform.rotation = Quaternion.Euler(transform.eulerAngles.x, DesiredRotation().eulerAngles.y, transform.eulerAngles.z);

            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x,
                                                       Mathf.LerpAngle(transform.localEulerAngles.y,
                                                                       _currentInteractor.PointerTransform.localEulerAngles.y + _angleOffset,
                                                                       _grabRotationSpeed * Time.deltaTime),
                                                       transform.localEulerAngles.z);

            if (_distanceFromPlayer > _maxHoldDistance || Physics.Linecast(transform.position, _currentInteractor.transform.position + Vector3.up * 1.75f, _obstacleLayerMask, QueryTriggerInteraction.Ignore))
            {
                _currentInteractor.CancelCurrentInteraction();
            }
        }
    }

    public void CheckBoundBox()
    {
        if (!ExtendedDataUtility.IsPointInArea(transform.position, Spawner.transform.position + Spawner.Center, Spawner.Size))
        {
            DestroyObject();
        }
    }

    #endregion

    #region Magnetization

    //Implement from IMagnetable Interface
    public void SetMagnetization(bool set, MagneticPlate magnet)
    {
        if (!_magnetable) return;

        if (magnet != null) _lastValidMagnetDir = magnet.transform.forward;

        _currentMagnet = magnet;
        if (_currentInteractor != null) 
        { 
            if (!set)
            {
                Grab(true); 
                return;
            }
            else
            {
                return;
            }
        }
        Magnetize(set);
    }

    //Set Magnetized Behaviour (override Grab behaviour if it was occurring)
    private void Magnetize(bool set)
    {
        if (!set && !_grabbed) { 
            c_rb.AddForce(_lastValidMagnetDir * _demagnetizationImpulse, ForceMode.VelocityChange);
            c_rb.AddTorque(_lastValidMagnetDir * _demagnetizationImpulse, ForceMode.VelocityChange);
        }

        //override current interaction
        if (_grabbed && _currentInteractor != null) _currentInteractor.CancelCurrentInteraction();
        Locked = set && !_canBeGrabbedIfMagnetized;

        //change movement behaviour
        _magnetized = set;
        c_rb.velocity = Vector3.zero;
        c_gravity.EnableGravity = !set;
        c_rb.drag = set ? _stuckDrag : _normalDrag;

        //disable bounciness
        c_collider.material = set ? null : _phyMat;

        //DetermineOrientation
        if (_currentMagnet != null)
        {
            //Store and Array of All Dot products
            float[] dirDotProducts = (from dir in _transformDirections let myDot = Vector3.Dot(dir, _currentMagnet.transform.forward) select myDot).ToArray();

            //Find Infex of closest Dot product
            int i = dirDotProducts.ToList().IndexOf(dirDotProducts.OrderBy(dot => Mathf.Abs(dot)).LastOrDefault());

            if (i == 0) //Align to Right
            {
                transform.DORotate(new Vector3(0f, transform.eulerAngles.y, transform.eulerAngles.z), 0.2f);
            }
            else if (i == 1) // Align to up
            {
                transform.DORotate(new Vector3(transform.eulerAngles.x, 0f, transform.eulerAngles.z), 0.2f);
            }
            else if (i == 2) // align to forward
            {
                transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0f), 0.2f);
            }

            //Ground Parenting
            if (set)
            {
                ParentToGround(false);
            }
        }
    }

    #endregion

    #region Interaction
    //Set Grab Behaviour (override magnet behaviour if it was occurring)
    public void SetInteraction(bool set, PlayerInteract interactor)
    {
        if (interactor != null && interactor.transform.parent == transform) return;

        _currentInteractor = interactor;
        if (!set && _currentMagnet != null) 
        { 
            Magnetize(true); 
            Grab(false); 
            return; 
        }
        Grab(set);

        //interactor?.SetInteracting(set ? this : null);
    }

    public void Grab(bool set)
    {
        if (_currentInteractor != null)
        {
            //Store and Array of All Dot products
            float[] dirDotProducts = (from dir in _transformDirections let myDot = Vector3.Dot(dir, _currentInteractor.PointerTransform.forward) select myDot).ToArray();

            //Find Infex of closest Dot product
            int i = dirDotProducts.ToList().IndexOf(dirDotProducts.OrderBy(dot => Mathf.Abs(dot)).LastOrDefault());
            _closestDot = dirDotProducts[i];

            if (i == 0) //Align to Right
            {
                _angleOffset = Mathf.Sign(_closestDot) * -90f;
            }
            else if(i == 1) // Align to up
            {
                _angleOffset = _closestDot < 0 ? 0 : 180;
            }
            else if(i == 2) // align to forward
            {
                _angleOffset = _closestDot > 0 ? 0 : 180;
            }
        }

        //override current interaction
        if (_magnetized) Magnetize(false);

        //change movement behaviour
        _grabbed = set;
        if(c_rb.velocity != null) c_rb.velocity *= _throwInertia;
        if(c_gravity !=  null) c_gravity.EnableGravity = !set;

        //define grab changes
        if (set)
        {
            c_rb.drag = _stuckDrag;

            LocalTimeScale = 1f;
            c_rb.constraints = RigidbodyConstraints.FreezeRotation;

            gameObject.layer = LayerMask.NameToLayer(_grabbedLayerName);

            ParentToGround(false);
        }
        else
        {
            c_rb.drag = _normalDrag;

            LocalTimeScale = _frozen ? 0f : 1f;
            c_rb.constraints = _frozen ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;

            gameObject.layer = LayerMask.NameToLayer(_idleLayerName);

            c_rb.AddForce(Vector3.up * _dropVerticalImpulse, ForceMode.VelocityChange);
        }
    }

    #endregion

    #region Explosion & Bounce

    //BOUNCE 
    public void BouncePadInteraction(Vector3 dir, float force)
    {
        if (!_grabbed) 
        {
            c_rb.velocity = _bouncePadMultiplier * force * dir.normalized;
            c_rb.angularVelocity = Mathf.Sign(c_rb.angularVelocity.y) * force * 0.5f * _rotationalInertia * Vector3.up;

            ParentToGround(false);
        }
    }

    //EXPLODE
    public void ExplosionBehaviour(Vector3 origin, Explosion exp, Vector3 normal)
    {
        if (_magnetized || _grabbed) return;

        //we need to check if the explosion originated on the surface of this object
        Vector3 xzDir = c_collider.bounds.Contains(origin) ? 
                            ExtendedMathUtility.HorizontalDirection(exp.SourcePos, transform.position).normalized : 
                            ExtendedMathUtility.HorizontalDirection(origin, transform.position).normalized;

        c_rb.velocity = Vector3.zero;

        c_rb.AddForce(
            (exp.BlastForce / _explosionMass) *
             exp.ApplyDirectionModifier(xzDir + Vector3.up, exp.ExplosionRules.ObjectDirectionDistribution), 
             ForceMode.VelocityChange);

        c_rb.angularVelocity = exp.BlastForce * _rotationalInertia * Vector3.up;

        ParentToGround(false);
    }

    #endregion

    #region FreezeTime


    protected override void OnFreezeTime(bool freeze)
    {
        _frozen = freeze;
        LocalTimeScale = _grabbed ? 1f : _frozen ? 0f : 1f;
        Body.constraints = _grabbed ? RigidbodyConstraints.FreezeRotation : _frozen ? RigidbodyConstraints.FreezeAll : RigidbodyConstraints.None;

        if(freeze) ParentToGround(false);
    }

    #endregion

    public void DestroyObject()
    {
        ParentToGround(false);

        if (GetComponentInChildren<PlayerMovement>() != null)
        {
            GetComponentInChildren<PlayerMovement>().transform.parent = null;
        }
        Destroy(gameObject);
    }

    public void Push(Vector3 direction)
    {
        if (_grabbed) Grab(false);
        if(!_magnetized && !_frozen)
        {
            c_rb.AddForce(direction, ForceMode.Force);
        }

        ParentToGround(false);
    }

    public void ParentToGround(bool set)
    {
        //transform.parent = set ? _ground.transform : null;
        //transform.SetParent(ExtendedDataUtility.Select(set, _ground.transform, null), true);
        //if (transform.parent != null)
        //    transform.localScale = new Vector3(1f / transform.parent.localScale.x,
        //                                       1f / transform.parent.localScale.y,
        //                                       1f / transform.parent.localScale.z);
        //else transform.localScale = Vector3.one;
        _grounded = set;
    }

    #endregion

    #region Debug

    private void OnDrawGizmosSelected()
    {
        if (Spawner)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(Spawner.transform.position + Spawner.Center, Spawner.Size);
        }
    }

    #endregion
}
