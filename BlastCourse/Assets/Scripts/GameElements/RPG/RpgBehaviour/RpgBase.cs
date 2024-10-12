using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using CustomMethods;

public abstract class RpgBase : ScriptableObject 
{
    #region Fields
    [Header("Special Interactions"), Space(3)]
    [Tooltip("Should rockets fired from this RPG automatically explode upon contact with the player after being reflected via BouncePad?")] public bool ExplodeOnPlayerUponReflection;

    #endregion

    #region Variables
    protected bool _allowSecondaryFire = true;
    protected bool _allowPrimaryFire = true;
    protected bool _recharging = false;

    [HideInInspector] public RpgStats _stats;
    protected LayerMask _aimLayerMask;
    protected float _maxAimDistance;
    protected Camera _camera;
    protected Transform _fireOrigin;
    public RpgHolder _rpgHolder;

    protected float _energy;
    [HideInInspector] public float Cost;
    protected float _timeSinceLastRocketFired;

    public List<RocketBase> rockets;
    

    #endregion

    #region Methods

    public virtual void ReceivePrimaryInput()
    {
        if (!_allowPrimaryFire) return;

        if (_energy < Cost)
        {
            _rpgHolder.FailShoot();
            return;
        }
        PrimaryFire();
    }

    public virtual void ReceiveSecondaryInput()
    {
        if (_allowSecondaryFire) SecondaryFire();
    }

    public virtual async void ResetPrimaryFire()
    {
        await Task.Delay((int)(_rpgHolder._fireSpeed * 1000));
        _allowPrimaryFire = true;
    }

    public virtual void InitializeValues(RpgStats stats, RpgHolder rpgHolder, LayerMask aimLayerMask, float maxAimDistance,Camera camera, Transform fireOrigin)
    {
        _stats = stats;
        _rpgHolder = rpgHolder;
        _aimLayerMask = aimLayerMask;
        _maxAimDistance = maxAimDistance;
        _camera = camera;
        _fireOrigin = fireOrigin;

        Cost = 1f / stats.ClipSize;
        _energy = 1f;

        rockets = new();
    }

    /// <summary>
    /// Called every frame. Used to update the rpg even when it is not being used
    /// </summary>
    public virtual void TickUnselected()
    {
        _timeSinceLastRocketFired = Mathf.Clamp(_timeSinceLastRocketFired += Time.deltaTime, 0f, _rpgHolder._energyRecoveryRate.keys[^1].time);
        _energy = Mathf.Clamp01(_energy + _rpgHolder._energyRecoveryRate.Evaluate(_timeSinceLastRocketFired) * Time.deltaTime * _stats.EnergyRecoveryMultiplier);
        if(_recharging && _energy == 1f)
        {
            _recharging = false;
            _rpgHolder.EnergyRecharge();
        }
    }

    /// <summary>
    /// Called every frame where this rpg is selected
    /// </summary>
    public virtual void TickSelected()
    {
        //Energy += Mathf.Lerp(_rpgHolder._energyRecoveryRate.keys[0].value, _rpgHolder._energyRecoveryRate.keys[1].value, something with time ) * Time.deltaTime;
        EventManager.OnUpdateEnergy?.Invoke(_energy);
    }

    public virtual void PrimaryFire()
    {
        _rpgHolder.PrimaryFireAnimation();
        _allowPrimaryFire = false;
        ResetPrimaryFire();
        if (SteamIntegrator.Instance != null) SteamIntegrator.Instance.FireRocket(_stats.FireMode);
        EventManager.OnFireRocketNotifyFireSpeed?.Invoke(_rpgHolder._fireSpeed);

        _energy -= Cost;
        _recharging = true;
        _timeSinceLastRocketFired = 0f;

        Vector3 targetPoint;

        if (_rpgHolder.IsAimingDown())
        {
            targetPoint = _rpgHolder.GetTargetPoint();
        }
        else
        {
            //fire rocket towards the point being aimed at, or towards the center of the screen
            if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hitInfo, _maxAimDistance, _aimLayerMask, QueryTriggerInteraction.Ignore))
            {
                targetPoint = hitInfo.point;
            }
            else
            {
                targetPoint = _camera.transform.position + _camera.transform.forward * _maxAimDistance;
            }
        }

        FireRocketAtPosition(targetPoint);
    }

    public virtual void SecondaryFire()
    {

    }

    public virtual void FireRocketAtPosition(Vector3 target)
    {
        Vector3 _targetDirection = (target - _fireOrigin.position).normalized;

        RocketBase instantiatedRocket = Instantiate(_stats.RocketPrefab, _fireOrigin.transform.position, _rpgHolder.transform.rotation, AudioManager.Instance.transform).GetComponent<RocketBase>();
        instantiatedRocket.rpg = this;
        instantiatedRocket.SetVelocity(_targetDirection * _stats.RocketSpeed);
    }

    #endregion

    #region Debug


    #endregion
}
