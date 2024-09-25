using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using CustomMethods;
using Unity.Mathematics;

public class PlayerMovement : MonoBehaviour, IBounceable, IExplodable, IMagnetable
{
    #region Fields
    [Header("Horizontal Movement"), Space(3)]
    [SerializeField] float _groundSpeed;
    [SerializeField] float _groundFriction;
    [Space(4), SerializeField] float _airSpeed;
    [SerializeField] float _airDrag;
    [SerializeField] float _groundTraction;
    [SerializeField] float _airTraction;
    [SerializeField] float _rocketJumpTraction;

    [Space(5), Header("Situational Movement"), Space(3)]
    [SerializeField] float _magentizedSpeed;
    [SerializeField] float _magnetizedDrag;
    [SerializeField] float _magnetDeadAngle;
    [SerializeField, Tooltip("X is multiplied to up direction, Y is multiplied to Magnet's Forward")] Vector2 _magnetizedJumpDivision;
    [SerializeField] private float _magnetizedGravityReduction;

    [Space(4), SerializeField] float _bouncePlayerSpeedMultiplier;
    [SerializeField] float _bouncePadForceMultiplier;

    float _maxSlideTime;
    float _minSpeedToSlide;
    float _slideCooldown;
    float _slideScale;
    float _slideScalingSpeed;

    [Space(5), Header("Vertical Movement"), Space(3)]
    [SerializeField] float _jumpForce;
    [SerializeField] float _verticalReflection;
    [SerializeField] float _groundcheckRadius;
    [SerializeField] GravityController c_gravity;
    [SerializeField, Tooltip("Vertical Impulse of explosions is severely reduced if not moving horizontally")] bool _restrictVerticalSpam;
    [SerializeField, Tooltip("If vertical impulse is restricted, how many stationary jumps are allowed before dampening is applied (Counter resets when the floor is touched)")] int _stationaryJumps;
    [DrawIf(nameof(_restrictVerticalSpam), false, DrawIfAttribute.DisablingType.ReadOnly)] public int _statJumpsCount;
    [SerializeField] float _minHorSpeed;

    [Space(5), Header("Input"), Space(3)]
    [SerializeField] string _horAxisName;
    [SerializeField] string _verAxisName;
    [SerializeField] string _jumpButtonName;
    [SerializeField] string _slideButtonName;
    [SerializeField] string _rjCancelButtonName;

    [Space(5), Header("Surface Interaction"), Space(3)]
    [SerializeField] float _playerRadius = 0.5f;
    [SerializeField] float _stepHeight = 0.3f;
    [SerializeField] float _surfaceCheckDistance;
    [SerializeField] float _maxSlopeAngle;
    [SerializeField] LayerMask _groundLayerMask;
    [SerializeField] LayerMask _wallLayerMask;

    [Space(5), Header("Components"), Space(3)]
    [SerializeField] private Rigidbody c_rb;
    [SerializeField] private Collider c_capsuleHitbox;
    [SerializeField] private Collider c_cilinderHitbox;
    [SerializeField] private Transform g_orientation;
    [SerializeField] private Transform g_centerOfGravity;
    [SerializeField] private Transform g_cam;
    [SerializeField] private GroundCheck g_groundCheck;

    [Space(5), Header("QoL"), Space(3)]
    [SerializeField] float _coyoteTime;
    [SerializeField] float _inputBufferTime;
    [SerializeField] float _jumpCooldown;

    [Space(5), Header("Data"), Space(3)]
    [SerializeField] float _notifyVelocityInterval;

    [Space(5), Header("Sounds"), Space(3)]
    [SerializeField] AudioCue landingSound;
    private AudioSource _landingSource;
    [SerializeField] AudioCue _stepsSfx;
    [SerializeField] AudioCue _ladderStepsSfx;
    [SerializeField] AudioCue jumpSound;
    private AudioSource _jumpSource;
    [SerializeField] float _timeBetweenSteps;
    private WaitForSeconds _stepsTimer;

    #endregion

    #region Vars

    //CONDITIONS
    private bool _isGrounded;
    private bool _isOnLadder;
    private bool _canJumpFromLadder = false;
    private bool _canGetOnLadders = true;
    private bool _isNoclip;
    private WaitForSeconds _ladderCooldown;
    private WaitForSeconds _coyoteCooldown;

    private bool _isRocketJumping;
    private bool _onJumpCooldown;

    private bool _slideInput;
    private bool _isSliding;
    private bool _readyToSlide = true;
    private bool _expiredSlideInput;

    private bool _onGraviPad;

    private bool _isMagnetized;

    private bool _shouldChangeCollider;

    private Vector3 _coyoteSSSPosition;

    //ACTIVE COROUTINES
    private Coroutine _currentlyActiveSlideTimer;

    //SPEED VALUES
    private float _playerAbsoluteSpeed;
    private float _playerHorizontalSpeed;
    private float _playerVerticalVelocity;
    private float _lastRecordedFallSpeed;

    //VECTORS
    private Vector2 _inputVec;
    private Vector3 _movementDirection;

    //SURFACE DETECTION
    private float _walkAngle;
    private float _previousSurfaceAngle;
    private RaycastHit hitGround;
    private RaycastHit hitRamp;
    private Ladder _currentLadder;
    private MagneticPlate _currentMagnet;

    //QoL
    private float _coyoteTimer;
    private float _jumpBufferTimer;

    #endregion

    #region UnityFunctions

    private IEnumerator Start()
    {
        c_rb.freezeRotation = true;
        _ladderCooldown = new WaitForSeconds(0.2f);
        _coyoteCooldown = new WaitForSeconds(_coyoteTime);
        _stepsTimer = new WaitForSeconds(_timeBetweenSteps);

        StartCoroutine(StepsCoroutine());
        StartCoroutine(LadderStepsCoroutine());

        yield return null;

        if (SaveLoader.Instance != null && SaveLoader.Instance.SpawnPos?.Length > 0) transform.position = SaveLoader.Instance.GetSpawn();
        else SaveLoader.Instance.SetSpawn(transform.position);
        //SaveLoader.Instance.SetSpawn(transform.position);
        //stick To Ground
        if (Physics.Raycast(transform.position + Vector3.up * 0.25f, Vector3.down, out RaycastHit hit, 5f, _groundLayerMask, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point;
        }
        c_rb.velocity = Vector3.zero;
        c_rb.Sleep();

        //StartCoroutine(SetCapsuleCollider(true));
        Time.timeScale = 1f;
        StartCoroutine(NotifyUpdateSpeed());
    }
    private void Update()
    {
        GroundCheck();
        ReadMovementInput();
        JumpInput();
        SlideInput();
    }

    private void FixedUpdate()
    {
        if (!_isOnLadder && (!_isMagnetized || _isGrounded)) HorizontalMovement();
        else if (_isOnLadder) LadderMovement();
        else MagnetMovement();

        UpdateDrag();
    }

    #endregion

    #region Methods

    //Read Inputs
    private void ReadMovementInput()
    {
        _inputVec = new Vector2(OptionsLoader.TryGetAxisRaw(InputActions.Right,InputActions.Left,_horAxisName), OptionsLoader.TryGetAxisRaw(InputActions.Forward, InputActions.Back, _verAxisName));

        if (!_isOnLadder)
        {
            //prevent players from going directly back while jumping
            _inputVec.y = !_onJumpCooldown ? _inputVec.y : Mathf.Clamp01(_inputVec.y);
        }
        else
        {
            if (_isGrounded &&
                _inputVec.magnitude > 0 &&
                Mathf.Abs(Vector3.Angle(ExtendedMathUtility.LocalToWordDirection(_inputVec, g_orientation), _currentLadder.transform.forward)) < 45f)
            {
                SetLadder(false);
            }
        }

    }
    private void JumpInput()
    {
        if (_isNoclip) return;

        //handle jump buffering (we will still jump for a short time after letting go of the key)
        _jumpBufferTimer = Mathf.Clamp(_jumpBufferTimer + Time.deltaTime, 0f, _inputBufferTime * 2);

        //as long as jump is held, reset its timer so that we jump as soon as we it the ground
        if (OptionsLoader.TryGetKey(InputActions.Jump,_jumpButtonName))
        {
            if(!_isOnLadder || _isOnLadder && _canJumpFromLadder) _jumpBufferTimer = 0;
        }
        else
        {
            _canJumpFromLadder = true;
        }

        //We should not be able to jump off ladders and magnets by holding the key, as that makes us automatically jump off them if we jump into them
        if (OptionsLoader.TryGetKeyDown(InputActions.Jump, _jumpButtonName) && !_onJumpCooldown)
        {
            if (_isMagnetized) //MAGNET JUMP
            {
                JumpOffMagnet();
                BeginJumpCooldown();
                return;
            }
            else if (_isOnLadder && _canJumpFromLadder) //LADDER JUMP
            {
                JumpOffLadder();
                BeginJumpCooldown();
                return;
            }
        }

        if (_jumpBufferTimer <= _inputBufferTime && !_onJumpCooldown) 
        {
            if (_coyoteTimer <= _coyoteTime && !_isMagnetized && !_isOnLadder && !_isRocketJumping) //NORMAL JUMP
            {
                JumpOnGround();
                BeginJumpCooldown();
            }
        }
    }

    private void SlideInput()
    {
        _slideInput = Input.GetButton(_slideButtonName);

        if (!_expiredSlideInput)
        {
            if (_slideInput && _isGrounded && _playerHorizontalSpeed >= _minSpeedToSlide && _inputVec.y == 1)
            {
                TryStartSlide();
            }
        }
        else
        {
            _expiredSlideInput = !Input.GetButtonDown(_slideButtonName);
        }

        if (_isSliding && (_playerHorizontalSpeed < _minSpeedToSlide || !_slideInput || !_isGrounded))
        {
            StopSlide();
        }
    }

    //Move
    private void HorizontalMovement()
    {
        //_movementDirection = (g_orientation.forward * _inputVec.y + g_orientation.right * _inputVec.x).normalized;
        //Debug.Log(g_orientation.forward * _inputVec.y + g_orientation.right * _inputVec.x);
        if(_isNoclip)
        {
            c_rb.drag = _groundFriction;
            c_rb.AddForce((g_cam.forward * _inputVec.y + g_cam.right * _inputVec.x).normalized * 100f, ForceMode.Acceleration);
            return;
        }

        _movementDirection = Vector3.Lerp(_movementDirection, 
                                         (g_orientation.forward * _inputVec.y + g_orientation.right * _inputVec.x).normalized, 
                                         Time.deltaTime * ExtendedDataUtility.Select(_isGrounded, _groundTraction, ExtendedDataUtility.Select(_isRocketJumping, _rocketJumpTraction, _airTraction)));

        Debug.DrawRay(g_orientation.transform.position, _movementDirection * 10f, Color.green);
        Debug.DrawRay(g_orientation.transform.position, g_orientation.transform.forward * 10f, Color.blue);
        Debug.DrawRay(g_orientation.transform.position, new Vector3(c_rb.velocity.x, 0f, c_rb.velocity.z), Color.yellow);

        if (_movementDirection != Vector3.zero)
        {
            
            //Detect Surface We are moving towards
            if (Physics.Raycast(transform.position + transform.up * _stepHeight * 0.5f, _movementDirection, out RaycastHit hitWall, _playerRadius + _surfaceCheckDistance, _wallLayerMask))
            {
                //IF WE ARE MOVING TOWARDS A LADDER, CLIMB IT.
                _currentLadder = hitWall.transform.GetComponent<Ladder>();
                if(_currentLadder != null && 
                    !_isOnLadder && 
                    _canGetOnLadders &&
                    Mathf.Abs(Vector3.Angle(c_rb.velocity, _currentLadder.transform.forward)) > 90f)
                {
                    c_rb.velocity = Vector3.zero;

                    SetLadder(true);
                }

                //WE HAVE DETECTED A WALL IN OUR DESIRED MOVEMENT DIRECTION
                if (!hitWall.collider.isTrigger && (90f - Vector3.Angle(-_movementDirection, hitWall.normal)) >= _maxSlopeAngle)
                {
                    //CHECK FOR SPACE AT STEP HEIGHT
                    if (Physics.Raycast(transform.position + transform.up * _stepHeight, _movementDirection, out RaycastHit hitStep, 2f*_playerRadius + _surfaceCheckDistance, _wallLayerMask, QueryTriggerInteraction.Ignore)
                        && (90f - Vector3.Angle(-_movementDirection, hitStep.normal)) >= _maxSlopeAngle)
                    {
                        //FLAT WALL FOUND, NULLIFY MOVEMENT
                        _isSliding = false;
                        return;
                    }
                    else //FREE SPACE AT STEP HEIGHT, WALK UP STEP
                    {
                        c_rb.position += transform.up * _stepHeight;
                    }
                }
            }

            float desiredSpeed = _isMagnetized? _magentizedSpeed : _isGrounded? _groundSpeed : _airSpeed;          

            if (Mathf.Abs(_inputVec.y) + Mathf.Abs(_inputVec.x) >= 0) c_rb.AddForce(Vector3.ClampMagnitude(_movementDirection, 1f) * desiredSpeed, ForceMode.Acceleration);

            if (_isMagnetized && _currentMagnet != null) c_rb.AddForce(-_currentMagnet.PullStrenght * _currentMagnet.transform.forward, ForceMode.Acceleration);
        }
    }
    private void LadderMovement()
    {
        Vector3 moveVector = _currentLadder.ClimbingSpeed * _inputVec.y * Time.deltaTime * Vector3.up;
        c_rb.velocity = moveVector;
    }
    private void MagnetMovement()
    {
        Vector3 moveVector;
        if (_inputVec.magnitude != 0)
        {
            float moveAngle = Vector3.Angle((g_orientation.forward * _inputVec.y + g_orientation.right * _inputVec.x).normalized, _currentMagnet.transform.right);
            moveVector = _currentMagnet.transform.right * (moveAngle > 90 + _magnetDeadAngle ? -1 : moveAngle < 90 - _magnetDeadAngle ? 1 : 0);
        }
        else moveVector = Vector3.zero;
        

        c_rb.AddForce(moveVector * _magentizedSpeed, ForceMode.Acceleration);

        if (_isMagnetized && _currentMagnet != null) c_rb.AddForce(-_currentMagnet.PullStrenght * _currentMagnet.transform.forward, ForceMode.Acceleration);
    }

    private void BeginJumpCooldown()
    {
        _onJumpCooldown = true;
        //ensure jump Inputs cannot be duplicated
        this.Invoke(() => _onJumpCooldown = false, _jumpCooldown);
    }

    private void JumpOnGround()
    {
        _canJumpFromLadder = false;
        c_rb.velocity = new Vector3(c_rb.velocity.x, 0, c_rb.velocity.z);
        c_rb.AddForce(transform.up * _jumpForce, ForceMode.VelocityChange);
        _jumpSource = AudioManager.TryPlayCueAtPoint(jumpSound, transform.position);
    }

    private void JumpOffMagnet()
    {
        if(_currentMagnet != null)
        {
            _canJumpFromLadder = false;
            _isMagnetized = false;
            c_rb.velocity = new Vector3(c_rb.velocity.x, 0, c_rb.velocity.z);
            c_rb.AddForce((transform.up * _magnetizedJumpDivision.y + _currentMagnet.transform.forward * _magnetizedJumpDivision.x) * _jumpForce, ForceMode.VelocityChange);
            _jumpSource = AudioManager.TryPlayCueAtPoint(jumpSound, transform.position);
        }
    }

    private void JumpOffLadder()
    {
        if (_currentLadder != null)
        {
            c_rb.velocity = Vector3.zero;

            c_rb.AddForce((transform.up * _playerRadius + _currentLadder.transform.forward).normalized * _jumpForce, ForceMode.VelocityChange);

            SetLadder(false);
            _jumpSource = AudioManager.TryPlayCueAtPoint(jumpSound, transform.position);
        }
    }


    private void GroundCheck()
    {
        if (_isNoclip) return;
        //Physics.SphereCast(g_centerOfGravity.position, _groundcheckRadius, Vector3.down, out hitGround, g_centerOfGravity.localPosition.y + _surfaceCheckDistance, _groundLayerMask)
        //Physics.Raycast(g_centerOfGravity.position, Vector3.down, out hitGround, g_centerOfGravity.localPosition.y + _surfaceCheckDistance, _groundLayerMask)

        hitGround = g_groundCheck.CheckGround(g_centerOfGravity.position, g_centerOfGravity.localPosition.y + _surfaceCheckDistance, _groundLayerMask);

        if (hitGround.transform != null)
        {
            //StartCoroutine(SetCapsuleCollider(true));

            if (!_isOnLadder) _canJumpFromLadder = false;
            _walkAngle = Vector3.Angle(Vector3.up, hitGround.normal);

            if (_walkAngle <= _maxSlopeAngle) //IN A VALID SURFACE
            {
                if (!_isGrounded)
                {
                    _landingSource = AudioManager.TryPlayCueAtPoint(landingSound, transform.position);
                    _statJumpsCount = 0;
                    EventManager.OnPlayerLanded?.Invoke(_lastRecordedFallSpeed);
                    _lastRecordedFallSpeed = 0;
                }

                _isGrounded = true;
                if (!_isOnLadder) c_gravity.EnableGravity = _isSliding;

                if (_walkAngle != _previousSurfaceAngle) //ONLY REORIENT PLAYER IF SURFACE ANGLE HAS CHANGED
                {
                    _previousSurfaceAngle = _walkAngle;

                    //reorient 
                    ReorientUpVector(hitGround.normal,false);

                    //Stick To Ground
                    c_rb.velocity = new Vector3(c_rb.velocity.x, 0f, c_rb.velocity.z);
                }
            }
            else if(!_isOnLadder) //SURFACE IS TO STEEP
            {
                SetAirBorne();
            }
        }
        else if (!_isOnLadder) //NO SURFACE DETECTED
        {
            SetAirBorne();
        }

        c_gravity.Scale = _isMagnetized ? _magnetizedGravityReduction : 1f;
        _isRocketJumping = c_rb.velocity.y >= 0 && _isRocketJumping;
        _coyoteTimer = _isGrounded ? 0f : _coyoteTimer += Time.deltaTime;

        if(_isGrounded) _coyoteSSSPosition = hitGround.point;
    }

    private void SetAirBorne()
    {
        ReorientUpVector(Vector3.up, true);

        _previousSurfaceAngle = 0f;
        _isGrounded = false;
        c_gravity.EnableGravity = true;
        if (c_rb.velocity.y != 0)_lastRecordedFallSpeed = c_rb.velocity.y;
        //StartCoroutine(SetCapsuleCollider(false));
    }

    private void ReorientUpVector(Vector3 newUp, bool onAir)
    {
        //Calcular el plano interseccion de la direccion y la rampa
        Vector3 directionNormal = Vector3.Cross(g_orientation.forward, Vector3.up);

        Vector3 newForward = Vector3.Cross(newUp, directionNormal);
        //newForward = Quaternion.AngleAxis(-90, Vector3.Cross(newForward,newUp)) * newUp;

        //if (Physics.Raycast(g_orientation.position + g_orientation.forward * 0.8f + Vector3.up, Vector3.down, out hitRamp, 2, _groundLayerMask))
        //{
        //    newForward = (hitRamp.point - g_orientation.forward * 0.5f) - g_orientation.position;
        //    
        //}
        //(g_orientation.forward - (Vector3.Dot(g_orientation.forward, newUp) / Mathf.Abs(Vector3.Dot(newUp, newUp)) * newUp)).normalized
        //Debug.DrawRay(g_orientation.position, newForward.normalized, Color.magenta, 1f);
        g_orientation.rotation = Quaternion.LookRotation(onAir ? new Vector3(newForward.x, 0, newForward.z).normalized : newForward.normalized, newUp.normalized);
    }

    private void UpdateDrag()
    {
        //set drag: if we are on ground do ground drag, if we are on air check if we are rocket jumping and change drag accordingly

        c_rb.drag = _isMagnetized ?
            _magnetizedDrag : //ON MAGNET
            _isGrounded ?
                _groundFriction : //Not on Magnet and in Ground
                _airDrag;
    }

    public void ExplosionBehaviour(Vector3 origin, Explosion exp, Vector3 normal)
    {
        _isRocketJumping = true;
        //DETERMINE Y FORCE MAGNITUDE
        float velY = c_rb.velocity.y;

        if (velY < -1f) // We are falling
        {
            if (Mathf.Max(Mathf.Abs(c_rb.velocity.x), Mathf.Abs(c_rb.velocity.z)) < _minHorSpeed && _restrictVerticalSpam && !exp.ExplosionRules.IgnoreVerticalAttenuation) //We are not moving horizontally, and we are restricting vertical spam
            {
                _statJumpsCount++;

                if(_statJumpsCount <= _stationaryJumps) velY = 0; 
                else velY *= _verticalReflection; //We will keep some of our momentum to lessen the upwards velocity gain
            }
            else
            {
                velY = 0f; //eliminate vertical momentum to facilitate walljumping
            }
        }
        else if (_onJumpCooldown) //eliminate vertical momentum so that jumping is not that necessary
        {
            velY = 0f;
            _onJumpCooldown = true;
        }

        c_rb.velocity = new Vector3(c_rb.velocity.x, velY ,c_rb.velocity.z);

        //DETERMINE XZ DIRECTION
        Vector3 xzDir = ExtendedMathUtility.HorizontalDirection(origin, c_rb.position);

        //Apply Force

        c_rb.AddForce(exp.BlastForce * 
            exp.ApplyDirectionModifier(
                ((exp.ExplosionRules.SurfaceNormalInfluence * normal) + 
                ((1 - exp.ExplosionRules.SurfaceNormalInfluence) * (xzDir + Vector3.up).normalized)).normalized, 
                exp.ExplosionRules.PlayerDirectionDistribution), ForceMode.VelocityChange);
    }

    #region Slide
    private void TryStartSlide()
    {
        if (!_isSliding && _readyToSlide)
        {
            _isSliding = true;
            _readyToSlide = false;

            //just in case, this code is technically not needed
            if(_currentlyActiveSlideTimer != null) StopCoroutine(_currentlyActiveSlideTimer);

            _currentlyActiveSlideTimer = StartCoroutine(SlideTimer());

            transform.DOScaleY(_slideScale, _slideScalingSpeed).SetEase(Ease.OutSine);
        }
    }

    private void StopSlide()
    {
        _isSliding = false;
        _expiredSlideInput = true;
        this.Invoke(() => _readyToSlide = true, _slideCooldown);

        if (_currentlyActiveSlideTimer != null) StopCoroutine(_currentlyActiveSlideTimer);

        transform.DOScaleY(1f, _slideScalingSpeed).SetEase(Ease.OutSine);
    }

    private IEnumerator SlideTimer()
    {
        yield return new WaitForSeconds(_maxSlideTime);
        StopSlide();
    }

    #endregion

    public void PushPull(Vector3 direction)
    {
        c_rb.AddForce(direction, ForceMode.Acceleration);
    }

    public void BouncePadInteraction(Vector3 dir, float force)
    {
        Vector3 vel = c_rb.velocity;
        _isRocketJumping = true;
        _statJumpsCount = 0;

        //c_rb.velocity = new Vector3(c_rb.velocity.x, 0f, c_rb.velocity.z);
        if(Vector3.Angle(dir, g_orientation.transform.up) > 10f) c_rb.velocity = Vector3.zero;
        else c_rb.velocity = new Vector3(c_rb.velocity.x, 0f, c_rb.velocity.z);

        _movementDirection = new Vector3(dir.x, 0f, dir.z).normalized;

        c_rb.AddForce(dir.normalized * (vel.magnitude * _bouncePlayerSpeedMultiplier + force * _bouncePadForceMultiplier), ForceMode.VelocityChange);
    }

    public IEnumerator IgnoreLadderForABit()
    {
        _canGetOnLadders = false;
        yield return _ladderCooldown;
        _canGetOnLadders = true;
    }

    private IEnumerator StepsCoroutine()
    {
        while (gameObject.activeInHierarchy) 
        { 
            yield return _stepsTimer;
            if (_isGrounded && (Mathf.Abs(c_rb.velocity.x) + Mathf.Abs(c_rb.velocity.z)) >= 5f) AudioManager.TryPlayCueAtPoint(_stepsSfx, transform.position);
        }

    }

    private IEnumerator LadderStepsCoroutine()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return _stepsTimer;
            if (_isOnLadder && (Mathf.Abs(c_rb.velocity.y) >= 2f)) AudioManager.TryPlayCueAtPoint(_ladderStepsSfx, transform.position);
        }

    }

    #endregion

    #region Getters/Setters
    public void SetGraviPad(bool value)
    {
        _onGraviPad = value;
    }
    public bool GetGraviPad()
    {
        return _onGraviPad;
    }
    public void SetLadder(bool isOnLadder)
    {
        _isOnLadder = isOnLadder;
        if (!_isOnLadder)
        {
            _currentLadder.SetPlayer(null);
            _currentLadder = null;
            _canJumpFromLadder = false;
            gameObject.transform.parent = null;
            c_gravity.EnableGravity = true;
            gameObject.GetComponent<PlayerInteract>().SetCanInteract(true);
        }
        else
        {
            _currentLadder.SetPlayer(this);
            gameObject.transform.parent = _currentLadder.transform;
            c_gravity.EnableGravity = false;
            gameObject.GetComponent<PlayerInteract>().SetCanInteract(false);
        }
    }
    public bool GetLadder()
    {
        return _isOnLadder;
    }

    public void SetMagnetization(bool set, MagneticPlate magnet)
    {
        if (set) c_rb.velocity = new Vector3(c_rb.velocity.x, 0f, c_rb.velocity.z);
        _currentMagnet = magnet;
        _isMagnetized = set;
    }

    public Vector3 GetSSSPosition()
    {
        return _coyoteSSSPosition ;
    }

    public bool GetCoyote()
    {
        return _coyoteTimer <= _coyoteTime;
    }

    public IEnumerator SetCapsuleCollider(bool isCapsule)
    {
        _shouldChangeCollider = isCapsule;
        yield return _coyoteCooldown;
        if(_shouldChangeCollider == isCapsule)
        {
            c_capsuleHitbox.enabled = isCapsule;
            c_cilinderHitbox.enabled = !isCapsule;
        }
    }

    #endregion

    #region Debug

    private IEnumerator NotifyUpdateSpeed()
    {
        WaitForSecondsRealtime _speedNotifyDelay = new (_notifyVelocityInterval);

        while (gameObject.activeInHierarchy)
        {
            _playerAbsoluteSpeed = c_rb.velocity.magnitude; //X
            _playerVerticalVelocity = c_rb.velocity.y; //Y
            _playerHorizontalSpeed = ExtendedMathUtility.VectorXZMagnitude(c_rb.velocity); //XZ

            EventManager.OnUpdatePlayerSpeedXYZ?.Invoke(_playerAbsoluteSpeed);
            EventManager.OnUpdatePlayerSpeedXZ?.Invoke(_playerHorizontalSpeed);
            EventManager.OnUpdatePlayerSpeedY?.Invoke(_playerVerticalVelocity);
            EventManager.OnUpdatePlayerVelocity?.Invoke(c_rb.velocity);

            EventManager.OnUpdatePlayerLocalVelocity?.Invoke(Vector3.Cross(c_rb.velocity,g_orientation.transform.right));

            yield return _speedNotifyDelay;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_coyoteSSSPosition, 0.3f);

        Gizmos.color = Color.blue;
        if(hitRamp.transform != null)Gizmos.DrawWireSphere(hitRamp.point - g_orientation.forward * 0.5f, 0.2f);
    }

    #endregion
}

