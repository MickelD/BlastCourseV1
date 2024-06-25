using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DamageTurret : GenericTurret
{
    #region Fields

    [Space(5), Header("Bullet"), Space(3)]
    [SerializeField] public ParticleSystem c_bullet;
    [SerializeField] public float _damage;
    [SerializeField] public float _knockBack;

    #endregion

    #region Variables

    private Health _hp;

    #endregion

    #region UnityFunctions



    #endregion

    #region Methods

    protected override void Shoot()
    {
        base.Shoot();

        if(c_bullet != null) c_bullet.Play();

        _hp.SufferDamage(_damage, Health.Source.ENEMY);

        if (_hp.TryGetComponent(out Rigidbody _rb)) _rb.AddForce(CustomMethods.ExtendedMathUtility.HorizontalDirection(transform.position, _hp.transform.position).normalized * _knockBack, ForceMode.VelocityChange);

        if(_hp.GetHealth() <= 0)
        {
            _posibleTargets.RemoveAt(_posibleTargets.IndexOf(_target));
            _target = null;
            _hp = null;
        }
    }

    protected override bool RaycastCheck(Transform obj)
    {
        bool check = false;

        if (obj.GetComponent<PlayerMovement>() != null) check = true;

        return check;
    }

    #endregion

    #region Trigger&Collision

    protected override void OnTriggerStay(Collider other)
    {

        var collidingPlayer = other.GetComponent<PlayerMovement>();
        if (!_posibleTargets.Contains(other.transform)
            && collidingPlayer != null)
        {
            _posibleTargets.Add(other.transform);
            _hp = collidingPlayer.GetComponent<Health>();
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        var collidingPlayer = other.GetComponent<PlayerMovement>();
        if ((collidingPlayer != null
            && _posibleTargets.Contains(other.transform)))
        {
            _posibleTargets.Remove(other.transform);
            _hp = null;
        }

    }

    #endregion
}


