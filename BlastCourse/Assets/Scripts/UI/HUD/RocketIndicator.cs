using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMethods;
using UnityEngine.UI;
using DG.Tweening;

/*
 *  This script HANDLES BOTH the INDICATOR FOR ROCKETS travelling towards the 
 *  player from behind, and the MARKER FOR THE REMOTE bomb selected for detonation.
 *  
 *  This is done to sync up the movements of them both. Note that we will always
 *  reffer to the FIRST (ROCKET BEHIND) AS INDICATOR and the SECOND (SELECTED REMOTE) AS MARKER
 */


public class RocketIndicator : MonoBehaviour
{
    [Space(5), Header("References"), Space(2)]
    [SerializeField] private float _canvasPlaneDistance;
    [SerializeField, Range(0, 1)] private float _distance;
    [SerializeField] private float _lerpSpeed;
    [SerializeField, Range(0, 1)] private float _baseAlpha;
    [SerializeField] private float _fadeDuration;
    [Space(3), SerializeField] private Camera _cam;

    [Space(5), Header("Pointer Values")]
    [Space(3), SerializeField] private RocketIndicatorComponents _indicatorComponents;
    [Space(3), SerializeField] private RemoteMarkerComponents _markerComponents;

    private RocketRemoteExplosion _selectedRemote;
    private RocketBase _closestRocket;
    private RectTransform _rectTransform;

    Vector3 _incomingFlatPos;
    Vector3 _remoteFlatPos;
    Vector3 _screenCenter;

    private bool _remoteAppearedInView;

    #region UnityFunctions

    private void OnValidate()
    {
        ValidateData();
    }

    private void Start()
    {
        ValidateData();

        UpdateSelectedRemote(null);
    }

    private void OnEnable()
    {
        EventManager.OnUpdateClosestIncomingRocket += UpdateIncomingRocket;
        EventManager.OnUpdateSelectedRemoteRocket += UpdateSelectedRemote;
    }

    private void OnDisable()
    {
        EventManager.OnUpdateClosestIncomingRocket -= UpdateIncomingRocket;
        EventManager.OnUpdateSelectedRemoteRocket -= UpdateSelectedRemote;
    }

    private void Update()
    {
        _screenCenter = _cam.transform.position + _cam.transform.forward * _canvasPlaneDistance;

        if (_closestRocket != null)
        {
            UpdateIndicatorPositionAndDirection();

            UpdateDangerLevel();
        }

        if (_selectedRemote != null)
        {
            UpdateMarkerPositionAndDirection();
        }
    }

    #endregion

    #region Methods
    private void UpdateSelectedRemote(RocketRemoteExplosion rocket)
    {
        _selectedRemote = rocket;

        if (_selectedRemote != null)
        {
            _remoteAppearedInView = ExtendedDataUtility.IsPointOnCamera(_selectedRemote.transform.position, _cam);

            FadeInFadeOut(true, ExtendedDataUtility.Select(_remoteAppearedInView, _markerComponents.OnScreenGroup, _markerComponents.OffScreenGroup));
            FadeInFadeOut(false, ExtendedDataUtility.Select(!_remoteAppearedInView, _markerComponents.OnScreenGroup, _markerComponents.OffScreenGroup));
        }
        else
        {
            FadeInFadeOut(_selectedRemote != null, _markerComponents.FadeGroup);
        }
    }

    private void UpdateIncomingRocket(RocketBase rocket)
    {
        _closestRocket = rocket;

        if (_closestRocket != null)
        {
            UpdateIndicatorVisuals();
        }

        FadeInFadeOut(_closestRocket != null, _indicatorComponents.FadeGroup);
    }


    private void ValidateData()
    {
        _rectTransform = (RectTransform)transform;

        _indicatorComponents.FlashColor   
        = new Color(_indicatorComponents.FlashColor.r, _indicatorComponents.FlashColor.g, _indicatorComponents.FlashColor.b, _baseAlpha);

        _indicatorComponents.TransformGroup.RotatedTransform.anchoredPosition 
        = _markerComponents.TransformGroup.RotatedTransform.anchoredPosition
        = _distance * Screen.height * Vector2.up;

        foreach (RectTransform trns in ExtendedDataUtility.CombineArrays(_indicatorComponents.TransformGroup.FollowerTransforms, _markerComponents.TransformGroup.FollowerTransforms))
            trns.anchoredPosition = _indicatorComponents.TransformGroup.RotatedTransform.anchoredPosition;
    }

    /// <summary>
    /// Update the Indicator for the CLOSEST INCOMING ROCKET
    /// </summary>
    private void UpdateIndicatorPositionAndDirection()
    {
        AimTowardsOffScreenPoint(_closestRocket.transform.position, _indicatorComponents.TransformGroup.SmoothlyRotate, _indicatorComponents.TransformGroup.InstantlyRotate, _indicatorComponents.TransformGroup.RotatedTransform, _indicatorComponents.TransformGroup.FollowerTransforms);
    }

    /// <summary>
    /// Update the marker for the SELECTED REMOTE DETONATOR
    /// </summary>
    private void UpdateMarkerPositionAndDirection()
    {
        if (ExtendedDataUtility.IsPointOnCamera(_selectedRemote.transform.position, _cam)) //In view
        {
            if (!_remoteAppearedInView)
            {
                FadeInFadeOut(true, _markerComponents.OnScreenGroup);
                FadeInFadeOut(false, _markerComponents.OffScreenGroup);
            }

            _remoteAppearedInView = true;

            
        }
        else //Outside View
        {
            if (_remoteAppearedInView)
            {
                FadeInFadeOut(false, _markerComponents.OnScreenGroup);
                FadeInFadeOut(true, _markerComponents.OffScreenGroup);
            }

            _remoteAppearedInView = false;

            
        }
        _markerComponents.ChaseOnScreenRocket.anchoredPosition = Vector3.Scale(_cam.WorldToViewportPoint(_selectedRemote.transform.position), new Vector3(1920f, 1080f, 0f));
        //_markerComponents.ChaseOnScreenRocket.anchoredPosition = _cam.WorldToScreenPoint(_selectedRemote.transform.position) - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        AimTowardsOffScreenPoint(_selectedRemote.transform.position, _markerComponents.TransformGroup.SmoothlyRotate, _markerComponents.TransformGroup.InstantlyRotate, _markerComponents.TransformGroup.RotatedTransform, _markerComponents.TransformGroup.FollowerTransforms);
    }

    /// <summary>
    /// Updates given transforms to point offscreen
    /// </summary>
    /// <param name="point"> The world space point projected onto the screen plane</param>
    /// <param name="lerpedRotation"> Transforms that are smoothly rotated to point towards projected point</param>
    /// <param name="instantRotation"> Transforms that are instantly rotated to point towards projected point</param>
    /// <param name="rotatedTransform"> A transform that is moved as a result of the rotation</param>
    /// <param name="followers"> Any other transform that must follow the rotated transform</param>
    private void AimTowardsOffScreenPoint(Vector3 point, Transform[] lerpedRotation, Transform[] instantRotation, Transform rotatedTransform, Transform[] followers)
    {
        Vector3 proj = ExtendedMathUtility.ProjectOnPlane(point, _cam.transform.forward, _screenCenter);

        // Apply Smooth Rotation
        foreach(Transform trns in lerpedRotation) 
            trns.rotation = Quaternion.Lerp(trns.rotation, Quaternion.LookRotation(trns.forward, (proj - _screenCenter).normalized), Time.deltaTime * _lerpSpeed);

        //Apply Instant Rotation
        foreach (Transform trns in instantRotation) 
            trns.rotation = Quaternion.LookRotation(trns.forward, (proj - _screenCenter).normalized);

        //Update follower transforms position
        foreach (Transform follower in followers)
            follower.position = rotatedTransform.position;
    }

    private void UpdateIndicatorVisuals()
    {
        foreach(Image sprite in _indicatorComponents.LightGroup) sprite.color = _closestRocket.rpg._stats.AssociatedVisuals.LightColor;
        foreach (Image sprite in _indicatorComponents.DarkGroup) sprite.color = _closestRocket.rpg._stats.AssociatedVisuals.DarkColor;
        _indicatorComponents.Icon.sprite = _closestRocket.rpg._stats.AssociatedVisuals.RocketIcon;
    }

    private void UpdateDangerLevel()
    {
        foreach (Image sprite in _indicatorComponents.FlashGroup)
        {
            sprite.color = Vector3.Distance(_closestRocket.transform.position, transform.position) <= _closestRocket.rpg._stats.Explosion.BlastRadius ? _indicatorComponents.FlashColor : _closestRocket.rpg._stats.AssociatedVisuals.LightColor;
        }
    }

    private void FadeInFadeOut(bool fadeIn, Image[] sprites)
    {
        foreach (Image img in sprites)
        {
            img.DOFade(ExtendedDataUtility.Select(fadeIn, _baseAlpha, 0f), _fadeDuration);
        }
    }

    #endregion

    #region Subclasses

    [System.Serializable]
    public struct RocketIndicatorComponents
    {
        [Space(5)]
        [SerializeField] public Color FlashColor;

        [Space(5), Header("Image Groups"), Space(3)]
        [Tooltip("Change this sprite to be the rocket's icon")] public Image Icon;
        [Tooltip("Images to fade in/out")] public Image[] FadeGroup;
        [Tooltip("Images to flash on rocket proximity")] public Image[] FlashGroup;
        [Tooltip("Images to color as the rocket's designated light color")] public Image[] LightGroup;
        [Tooltip("Images to color as the rocket's designated dark color")] public Image[] DarkGroup;

        [Space(5)]
        public PointerTransformGroup TransformGroup;
    }

    [System.Serializable]
    public struct RemoteMarkerComponents
    {
        [Space(5)]
        [SerializeField] public Image[] FadeGroup;
        [SerializeField] public Image[] OnScreenGroup;
        [SerializeField] public Image[] OffScreenGroup;

        [Space(5)]
        public RectTransform ChaseOnScreenRocket; 
        public PointerTransformGroup TransformGroup;
    }

    [System.Serializable]
    public struct PointerTransformGroup
    {
        [Tooltip("A transform that is moved due to a rotation")] public RectTransform RotatedTransform;
        [Tooltip("Transforms to apply lerped rotation to")] public RectTransform[] SmoothlyRotate;
        [Tooltip("Transforms to apply an instant rotation to")] public RectTransform[] InstantlyRotate;
        [Tooltip("Transforms that will follow the rotated transform's position (but not rotation)")] public RectTransform[] FollowerTransforms;
    }

    #endregion

    #region Debug

    private bool hide;

    [ContextMenu("Toggle Pointer Visibility")]
    public void HidePointers()
    {
        foreach (Image sprite in ExtendedDataUtility.CombineArrays(_indicatorComponents.FadeGroup, _markerComponents.FadeGroup))
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, ExtendedDataUtility.Select(hide, 0f, _baseAlpha));
        }

        hide = !hide;
    }

    #endregion
}

/*      MAYBE SOME DAY, WE WILL WALK FREE FROM THE CHAINS OF SHAME, BUT FOR NOW, NONE SHALL BE FORGIVEN
 
             //project world position of rocket onto the HUD plane
            _screenCenter = _cam.transform.position + _cam.transform.forward * _canvasPlaneDistance;
            _flatPos = ExtendedMathUtility.ProjectOnPlane(_closestRocket.transform.position, _cam.transform.forward, _cam.transform.position + _cam.transform.forward * _canvasPlaneDistance);

            //rotation
            _rectTransform.localRotation = Quaternion.Euler(0f, 0f, Vector3.Angle(_cam.transform.up, (_flatPos - _screenCenter).normalized));

            //DELETE
            Debug.DrawLine(_closestRocket.transform.position, _flatPos, Color.cyan);
            Debug.DrawRay(_screenCenter, _flatPos - _screenCenter, Color.green);

            //Turn Projection into a screen space anchored point 
            _flatPos = (Vector2)_cam.WorldToScreenPoint(_flatPos) - _screenBounds - Vector2.one * _borderOffset;

            //normalize position to turn it into a constant circle that takes it outside the screen
            _flatPos = _flatPos.normalized * (200f);

            //smoothing
            _lerpedPosition.x = Mathf.Lerp(_lerpedPosition.x, _flatPos.x, Time.deltaTime * _lerpSpeed.x);
            _lerpedPosition.y = Mathf.Lerp(_lerpedPosition.y, _flatPos.y, Time.deltaTime * _lerpSpeed.y);

            //clamp its values to the margins so that it sticks to the borders
            _flatPos = new Vector2(Mathf.Clamp(_flatPos.x, -_screenBounds.x + _borderOffset, _screenBounds.x - _borderOffset),
                                   Mathf.Clamp(_flatPos.y, -_screenBounds.y + _borderOffset, _screenBounds.y - _borderOffset));

            //update screen space position
            _rectTransform.anchoredPosition = _lerpedPosition; 



        _incomingFlatPos = ExtendedMathUtility.ProjectOnPlane(_closestRocket.transform.position, _cam.transform.forward, _screenCenter);

        //Marker position is damped
        _indicatorComponents._indicatorPivot.rotation = Quaternion.Lerp(_indicatorComponents._indicatorPivot.rotation, Quaternion.LookRotation(_indicatorComponents._indicatorPivot.forward, (_incomingFlatPos - _screenCenter).normalized), Time.deltaTime * _lerpSpeed);

        //Marker Direction is instant    
        _indicatorComponents._indicatorArrow.transform.SetPositionAndRotation( _indicatorComponents._indicatorBackground.transform.position, Quaternion.LookRotation(_indicatorComponents._indicatorPivot.forward, (_incomingFlatPos - _screenCenter).normalized));
        _indicatorComponents._indicatorIcon.transform.position = _indicatorComponents._indicatorBackground.transform.position;
 */
