using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RPGAnimator : MonoBehaviour
{
    #region Fields

    public Animator AnimController;
    [SerializeField] ParticleSystem _plop;

    public SkinnedMeshRenderer RpgMesh;

    public float SssToHandTransitionSpeed = 7;
    public float DefaultToSssTransitionSpeed = 3;

    public static RPGAnimator Instance;

    #endregion

    #region Variables

    private bool _grabbing;
    private bool _hidden;
    private bool _aimingDown;
    private bool _handForward;

    private int _rocketType;
    private float _angle;

    private float _sthTimer;
    private float _desiredAngle;

    #endregion

    #region UnityFunctions

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Update()
    {
        if (_handForward && _sthTimer < 1)
        {
            _sthTimer = Mathf.Clamp01(_sthTimer + Time.deltaTime * SssToHandTransitionSpeed);
            UpdateHandForward();
        }
        else if (!_handForward && _sthTimer > 0)
        {
            _sthTimer = Mathf.Clamp01(_sthTimer - Time.deltaTime * SssToHandTransitionSpeed);
            UpdateHandForward();
        }

        if(_angle > _desiredAngle)
        {
            _angle -= DefaultToSssTransitionSpeed * Time.deltaTime;
            _angle = Mathf.Clamp(_angle, _desiredAngle, 1);
            UpdateSSSAngle();
        }
        else if (_angle < _desiredAngle)
        {
            _angle += DefaultToSssTransitionSpeed * Time.deltaTime;
            _angle = Mathf.Clamp(_angle, 0, _desiredAngle);
            UpdateSSSAngle();
        }


    }

    #endregion

    #region Methods
    

    //RPG Animations
    public void SetHidden(bool isHidden)
    {
        _hidden = isHidden;

        AnimController.SetBool("Hidden", _hidden);
    }
    public void Shoot()
    {
        AnimController.SetTrigger("Shoot");
        _plop.Emit(1);
    }
    public void SetRocket(FiringMode firingMode)
    {
        _rocketType = (int)firingMode;

        AnimController.SetInteger("RocketType", _rocketType);
    }
    public void SetSSSAngle(bool aiming, float angle)
    {
        _aimingDown = aiming;
        if (aiming)
        {
            if (_grabbing) _handForward = true;
            _desiredAngle = Mathf.Clamp01((angle + 90) / 180);
        }
        else
        {
            _desiredAngle = 0;
        }
        
    }
    public void UpdateSSSAngle()
    {
        AnimController.SetFloat("AimDownRotation",_angle);
    }

    //Hand Animations
    public void SetGrabbing(bool isGrabbing)
    {
        if (_aimingDown) _handForward = isGrabbing;
        _grabbing = isGrabbing;

        AnimController.SetBool("Grabbing", _grabbing);
    }
    public void Interact()
    {
        if (_aimingDown) _handForward = true;

        AnimController.SetTrigger("Interact");
        StartCoroutine(EndInteract());
    }
    private IEnumerator EndInteract()
    {
        yield return new WaitForSeconds(0.8f);
        _handForward = false;
    }
    public void TakeBox()
    {
        if (_aimingDown) _handForward = true;

        AnimController.SetTrigger("Take");
        StartCoroutine(EndTakeBox());
    }
    private IEnumerator EndTakeBox()
    {
        yield return new WaitForSeconds(0.4f);
        _handForward = false;
    }
    private void UpdateHandForward()
    {
        AnimController.SetLayerWeight(4, _sthTimer);
    }

    #endregion
}


