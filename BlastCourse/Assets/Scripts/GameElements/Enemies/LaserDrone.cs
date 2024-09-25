using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDrone : MonoBehaviour
{
    #region Fields

    [Space(5), Header("Movement"), Space(3)]
    [SerializeField] float _rotationSpeedX;
    [SerializeField] float _rotationSpeedY;
    [SerializeField] float _amplitudeHeight;
    [SerializeField] float _amplitudeSpeed;

    [Space(5), Header("Detection"), Space(3)]
    [SerializeField] float _detectionDistance;
    [SerializeField] float _detectionAngle;
    [SerializeField] LayerMask _detectionMask;
    [SerializeField] GameObject g_player;

    [Space(5), Header("Laser Variables"), Space(3)]
    [SerializeField] LineRenderer c_lr;
    [SerializeField] Transform _laserOrigin;
    [SerializeField] float _laserWidth;
    [SerializeField] float _laserRange;
    [SerializeField] float _laserDecrease;

    [Space(5), Header("Laser Charge"), Space(3)]
    [SerializeField] float _laserChargeTimer;
    [SerializeField] ParticleSystem c_chargeParticles;

    [Space(5), Header("Laser Attack"), Space(3)]
    [SerializeField] float _laserRecoil;
    [SerializeField] float _laserDelay;
    [SerializeField] float _laserAngle;
    [SerializeField] float _laserDamage;
    [SerializeField] Health.Source _damageType;
    [SerializeField] LayerMask _laserMask;

    [Space(5), Header("Body"), Space(3)]
    [SerializeField] float _playerHeight = 1.5f;
    [SerializeField] GameObject g_bodyMiddle;
    [SerializeField] Rigidbody c_rb;


    [Space(5), Header("Audio"), Space(3)]
    [SerializeField] AudioCue shootSound;

    #endregion

    #region Variables

    private Vector3 _headOrientation;
    private Vector3 _velocity;
    private Vector3 _direction;

    private float _lTimer;
    private float _cTimer;

    private float _distance;
    private float _angle;

    private bool _playerDetected = false;
    private bool _attack = false;
    private bool _charging;

    #endregion

    #region UnityFunctions

    private void Update()
    {
        //Detects Player
        if(g_player != null)
        {
            RaycastHit hit;
            _direction = g_player.transform.position + Vector3.up * _playerHeight - transform.position;
            _distance = _direction.magnitude;
            _angle = Vector3.Angle(transform.forward, new Vector3(_direction.x, 0, _direction.z)) * 2;

            if(_distance <= _detectionDistance
               && _angle <= _detectionAngle
               && Physics.Raycast(transform.position, _direction, out hit, _detectionDistance, _detectionMask))
            {
                if(hit.transform.gameObject == g_player)
                {
                    _playerDetected = true;
                }
                else
                {
                    _playerDetected = false;
                }

                if (_angle <= _laserAngle) _attack = true;
                else _attack = false;
            }
            else
            {
                _playerDetected = false;
            }
        }
        else
        {
            _playerDetected = false;
        }

        //Floats
        _velocity = Mathf.Sin(Time.time * _amplitudeSpeed) * Vector3.up * _amplitudeHeight * Time.deltaTime;
        c_rb.velocity = _velocity;

        //Rotates towards Player
        if(g_player != null && _playerDetected)
        {
            transform.forward = Vector3.RotateTowards(transform.forward, new Vector3(_direction.x, 0, _direction.z).normalized, _rotationSpeedX * Time.deltaTime, Mathf.Infinity);

            g_bodyMiddle.transform.forward = _direction.normalized;
            _headOrientation = g_bodyMiddle.transform.eulerAngles;
            _headOrientation.y = transform.eulerAngles.y;
            g_bodyMiddle.transform.eulerAngles = _headOrientation;
        }

        //Attack
        _lTimer -= Time.deltaTime;
        if (g_player != null && _playerDetected && _attack && _lTimer <= 0)
        {
            _charging = true;
            _lTimer = _laserDelay;
            _cTimer = _laserChargeTimer;
            c_chargeParticles.gameObject.SetActive(true);
        }
        if(c_lr.widthMultiplier > 0)
        {
            c_lr.widthMultiplier = Mathf.Lerp(c_lr.widthMultiplier, 0, _laserDecrease * Time.deltaTime);
            if (c_lr.widthMultiplier < 0.1f) c_lr.widthMultiplier = 0;
        }
        if (_charging)
        {
            _cTimer -= Time.deltaTime;
            _cTimer = Mathf.Clamp(_cTimer, 0.1f, _laserChargeTimer);
            ParticleSystem.EmissionModule emmision = c_chargeParticles.emission;
            emmision.rateOverTime = 4.5f / _cTimer;
            if(_cTimer == 0.1f)
            {
                _charging = false;
                Shoot();
                c_chargeParticles.gameObject.SetActive(false);
            }
        }
    }

    #endregion

    #region Methods

    private void Shoot()
    {
        AudioManager.TryPlayCueAtPoint(shootSound, Vector3.zero);
        c_lr.widthMultiplier = _laserWidth;
        RaycastHit hit;
        Vector3 point;
        if(Physics.Raycast(_laserOrigin.position, _laserOrigin.transform.forward, out hit, _laserRange, _laserMask))
        {
            point = hit.point;
            Health hp = hit.transform.GetComponent<Health>();
            if (hp != null)
            {
                hp.SufferDamage(_laserDamage, _damageType);
            }
            Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddForce(_laserOrigin.transform.forward * _laserRecoil, ForceMode.Impulse);
            }
        }
        else
        {
            point = _laserOrigin.transform.position +_laserOrigin.transform.forward * _laserRange;
        }

        c_lr.SetPosition(0, _laserOrigin.position);
        c_lr.SetPosition(1, point);
    }

    #endregion

    #region Debug

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,_detectionDistance);

    }

    #endregion
}
