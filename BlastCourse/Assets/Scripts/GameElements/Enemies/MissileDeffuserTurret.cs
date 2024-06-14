using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileDeffuserTurret : GenericTurret
{
    #region Fields

    [Space(5), Header("Laser"), Space(3)]
    [SerializeField] public LineRenderer c_lr;
    [SerializeField] public float _laserDuration;
    [SerializeField] public float _laserSteps;
    [SerializeField] public float _laserWiggliness;
    [SerializeField] public float _pushForce;
    [SerializeField] public AudioCue destroyRockectSound;

    #endregion

    #region Variables

    private bool _targetingRocket;

    private float _laserTimer;

    #endregion

    #region UnityFunctions

    public override void Update()
    {
        base.Update();

        Laser();
    }

    #endregion

    #region Methods
    private void SetLaser()
    {
        if (_target != null)
        {
            c_lr.enabled = true;
            Vector3 distance = _target.transform.position - g_gunPoint.transform.position;
            bool createRope = true;

            int count = 0;
            c_lr.positionCount = (int)(distance.magnitude / _laserSteps) + 1;
            if (distance.magnitude % _laserSteps > 0)
            {
                c_lr.positionCount++;
            }
            Vector3 previousPos = g_gunPoint.transform.position;
            c_lr.SetPosition(count, g_gunPoint.transform.position);
            count++;

            while (createRope)
            {
                if (((distance.normalized * _laserSteps + previousPos) - g_gunPoint.transform.position).magnitude > distance.magnitude)
                {
                    createRope = false;
                    c_lr.SetPosition(count, _target.transform.position);
                }
                else
                {
                    previousPos = distance.normalized * _laserSteps + previousPos;
                    c_lr.SetPosition(count, previousPos);
                    count++;
                }
            }
        }

    }

    protected override void Shoot()
    {
        if (_targetingRocket && _posibleTargets.Count > 0)
        {
            base.Shoot();

            SetLaser();
            _posibleTargets.Remove(_target);
            if(_target != null && _target.GetComponent<RocketBase>() != null) _target.GetComponent<RocketBase>().Defuse();
            _target = null;
        }
    }

    private void WiggleLaser()
    {
        for (int i = 1; i < c_lr.positionCount - 1; i++)
        {
            c_lr.SetPosition(i, c_lr.GetPosition(i) + Random.insideUnitSphere * _laserWiggliness / 100);
        }
    }

    protected virtual void Laser()
    {
        if (c_lr.enabled)
        {
            _laserTimer -= Time.deltaTime;
            WiggleLaser();
            if (_laserTimer <= 0)
            {
                _laserTimer = _laserDuration;
                c_lr.enabled = false;
            }
        }
    }

    protected override bool RaycastCheck(Transform obj)
    {
        bool check = false;

        if (obj.GetComponent<RocketBase>() || obj.GetComponent<PhysicsObject>()) check = true;

        return check;
    }

    #endregion

    #region Trigger&Collision

    protected override void OnTriggerStay(Collider other)
    {
        
        var collidingRocket = other.GetComponent<RocketBase>();
        if (!_posibleTargets.Contains(other.transform) && collidingRocket != null && collidingRocket is not RocketPipe)
        {
            //Checks for Rockets
            _posibleTargets.Add(other.transform);
            _targetingRocket = true;
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<RocketBase>() && _posibleTargets.Contains(other.transform))
        {
            _posibleTargets.Remove(other.transform);
        }
        
    }

    #endregion
}
