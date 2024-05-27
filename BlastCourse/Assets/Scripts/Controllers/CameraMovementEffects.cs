using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraMovementEffects : MonoBehaviour
{
    [Space(5), Header("Components"), Space(3)]
    [SerializeField] private Rigidbody g_playerRB;
    [SerializeField] private CinemachineVirtualCamera _camera;
    [SerializeField] private CinemachineCameraOffset _camOffset;

    [Space(5), Header("FOV change on speed"), Space(3)]
    [SerializeField] private bool _changeFovWithSpeed;
    [SerializeField] private AnimationCurve _speedAndFovCurve;
    [SerializeField] private float _lerpFovDuration;
    private float _desiredFov;

    [Space(5), Header("Landing Offset"), Space(3)]
    [SerializeField] private bool _displaceCameraOnLanding;
    [SerializeField] private AnimationCurve _YSpeedAndOffsetCurve;
    [SerializeField] private float _cameraAnimationDuration;
    [SerializeField, Range(0, 1)] private float _easeInEaseOutDivision;

    private void OnEnable()
    {
        if (_changeFovWithSpeed)
        {
            EventManager.OnUpdatePlayerSpeedXZ += UpdateDesiredFOV;
        }

        if (_displaceCameraOnLanding)
        {
            EventManager.OnPlayerLanded += CameraLandingAnimation;
        }
    }

    private void OnDisable()
    {
        EventManager.OnUpdatePlayerSpeedXZ -= UpdateDesiredFOV;
        EventManager.OnPlayerLanded -= CameraLandingAnimation;
    }

    private void UpdateDesiredFOV(float spd)
    {
        float newFOV = _speedAndFovCurve.Evaluate(Mathf.Round(spd));

        if (newFOV != _desiredFov)
        {
            //If we are going at a speed that means we should have a new FOV, Tween it to the new value;

            _desiredFov = newFOV;

            //float currentFOV = _camera.m_Lens.FieldOfView;

            //DOVirtual.Float(currentFOV, _desiredFov, _lerpFovDuration, value =>
            //{
            //    _camera.m_Lens.FieldOfView = value;
            //});

            //_camera.m_Lens.FieldOfView =  _desiredFov;
        }
    }

    private void CameraLandingAnimation(float ySpeed)
    {
        //pull the camera down
        DOVirtual.Vector3(_camOffset.m_Offset, Vector3.down * _YSpeedAndOffsetCurve.Evaluate(Mathf.Abs(ySpeed)), _cameraAnimationDuration * _easeInEaseOutDivision, descendingOffset =>
        {
            _camOffset.m_Offset = descendingOffset;
        }
        ).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            //pull the camera back up
            DOVirtual.Vector3(_camOffset.m_Offset, Vector3.zero, _cameraAnimationDuration * (1f - _easeInEaseOutDivision), ascendingOffset =>
            {
                _camOffset.m_Offset = ascendingOffset;
            }).SetEase(Ease.InOutSine);
        });
    }
}
