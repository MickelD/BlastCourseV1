using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Health : MonoBehaviour
{
    #region Fields
    [Space(5), Header("Damage Multipliers"), Space(3)]
    [SerializeField] protected float _playerDamageMultiplier;
    [SerializeField] protected float _enemyDamageMultiplier;
    [SerializeField] protected float _environmentDamageMultiplier;

    [Space(5), Header("Health"), Space(3)]
    [SerializeField] protected float _maxHealth;
    [SerializeField] protected float _health;

    [Space(5), Header("Values"), Space(3)]
    [SerializeField] protected float _invulnerability = 0.5f;
    [SerializeField] protected float _respawnTime = 0.3f;

    [Space(5), Header("Audio"), Space(3)]
    [SerializeField] AudioCue damageSound;
    [SerializeField] AudioCue sfxDeathSound;

    [Space(5), Header("PlayerVariables"), Space(3)]
    [SerializeField] CinemachineVirtualCamera _cam;
    [SerializeField] float _shakeForce;
    [SerializeField] float _shakeDuration;
    [SerializeField] float _timeUntilHeal;
    [SerializeField] float _healRate;

    #endregion

    #region Variables
    public enum Source
    {
        PLAYER,
        ENEMY,
        ENVIRONMENT,
        HEAL
    }

    protected float _invincibleTimer;

    protected bool _alive = true;

    float _shakeTimer;
    float _healTimer;
    
    #endregion

    #region UnityFunctions
    public void Awake()
    {
        _health = _maxHealth;
    }

    public void Start()
    {
        EventManager.OnUpdateHealth.Invoke(_health / _maxHealth);
    }

    //FOR TESTING PURPOSES

    protected void Update()
    {
        if(_invincibleTimer > 0)
        {
            _invincibleTimer -= Time.deltaTime;
        }

        if(_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;
            if (_shakeTimer <= 0) Shake(false);
        }

        if(_health < _maxHealth && _healTimer > 0)
        {
            _healTimer -= Time.deltaTime;
        }
        else if(_health < _maxHealth)
        {
            Heal(_healRate*Time.deltaTime);
        }

        if(Input.GetKeyDown(KeyCode.L) && GetComponent<PlayerMovement>() != null)
        {
            SufferDamage(10,Source.ENEMY);
        }
    }
    #endregion

    #region Methods
    public virtual void SufferDamage(float amount, Source source)
    {
        float damageRecieved = amount;
        if(_invincibleTimer <= 0)
        {
            switch (source)
            {
                case Source.PLAYER:
                    damageRecieved *= _playerDamageMultiplier;
                    _health -= damageRecieved;
                    HurtEffect();
                    if(damageRecieved > 0) _invincibleTimer = _invulnerability;
                    break;
                case Source.ENEMY:
                    damageRecieved *= _enemyDamageMultiplier;
                    AudioManager.TryPlayCueAtPoint(damageSound, transform.position);
                    _health -= damageRecieved;
                    HurtEffect();
                    if (damageRecieved > 0) _invincibleTimer = _invulnerability;
                    
                    break;
                case Source.ENVIRONMENT:
                    damageRecieved *= _environmentDamageMultiplier;
                    _health -= damageRecieved;
                    HurtEffect();
                    if (damageRecieved > 0) _invincibleTimer = _invulnerability;
                    break;
                case Source.HEAL:
                    _health -= amount;
                    HealEffect();
                    break;
                default:
                    _health -= amount;
                    break;
            }
            _health = Mathf.Clamp(_health, 0, _maxHealth);


            if (_health <= 0 && _alive)
            {
                _alive = false;
                Die();
            }
        }
        
    }
    public void Heal(float amount)
    {
        _invincibleTimer = 0;
        SufferDamage(-amount, Source.HEAL);
    }
    public virtual void Die()
    {
        AudioManager.TryPlayCueAtPoint(sfxDeathSound, transform.position);
        StartCoroutine(Respawn());
    }

    public void SetRespawn(Vector3 position)
    {
        SaveLoader.Instance?.SetSpawn(position);
    }
    public void HurtEffect()
    {
        Shake(true);
        _healTimer = _timeUntilHeal;
        EventManager.OnUpdateHealth.Invoke(_health/_maxHealth);
    }
    public void HealEffect()
    {
        EventManager.OnUpdateHealth.Invoke(_health / _maxHealth);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(_respawnTime);
        SaveLoader.Instance?.Load();
        //_health = _maxHealth;
        //transform.position = _respawnPosition;
        //GetComponent<Rigidbody>().velocity = Vector3.zero;
        //CreditManager.Instance.RespawnCredits();
        //_alive = true;
    }

    private void Shake(bool shake)
    {
        if(_cam != null)
        {
            _shakeTimer = shake ? _shakeDuration : 0;

            _cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = shake ? _shakeForce : 0;
        }
    }

    #endregion

    #region Getter/Setter Methods
    public void SetHealth(float value)
    {
        _health = value;
    }
    public float GetHealth()
    {
        return _health;
    }
    public void SetMaxHealth(float value)
    {
        _maxHealth = value;
    }
    public float GetMaxHealth()
    {
        return _maxHealth;
    }


    #endregion

}
