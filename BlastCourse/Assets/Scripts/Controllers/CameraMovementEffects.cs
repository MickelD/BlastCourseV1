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
    [HideInInspector] public CinemachineBasicMultiChannelPerlin _noise;

    [Space(5), Header("Landing Offset"), Space(3)]
    [SerializeField] private bool _displaceCameraOnLanding;
    [SerializeField] private AnimationCurve _YSpeedAndOffsetCurve;
    [SerializeField] private float _cameraAnimationDuration;
    [SerializeField, Range(0, 1)] private float _easeInEaseOutDivision;

    private void OnEnable()
    {
        EventManager.OnFovChanged += UpdateDesiredFOV;

        if (_displaceCameraOnLanding)   EventManager.OnPlayerLanded += CameraLandingAnimation;

        if (_noise == null) _noise = _camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnDisable()
    {
        EventManager.OnFovChanged -= UpdateDesiredFOV;
        EventManager.OnUpdatePlayerSpeedXZ -= UpdateDesiredFOV;
        EventManager.OnPlayerLanded -= CameraLandingAnimation;
    }

    private void UpdateDesiredFOV(float fov)
    {
        _camera.m_Lens.FieldOfView = fov;
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
