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

    [Space(5), Header("Death"), Space(3)]
    [SerializeField] protected float _deathYScale;
    [SerializeField] protected float _respawnTime;
    [SerializeField] protected Vector3 _deathImpulse;
    [SerializeField] protected float _deathRot;
    [SerializeField] protected float _deathDrag;
    [SerializeField] protected float _deathAngularDrag;

    [Space(5), Header("Audio"), Space(3)]
    [SerializeField] AudioCue damageSound;
    [SerializeField] AudioCue sfxDeathSound;

    [Space(5), Header("PlayerVariables"), Space(3)]
    [SerializeField] CinemachineImpulseSource _camShake;
    [SerializeField] float _shakeForce;
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

    protected bool _alive = true;

    float _healTimer;

    private WaitForSeconds _respawnTimer;
    
    #endregion

    #region UnityFunctions
    public void Awake()
    {
        _health = _maxHealth;
    }

    public void Start()
    {
        EventManager.OnUpdateHealth.Invoke(_health / _maxHealth);
        _respawnTimer = new WaitForSeconds(_respawnTime);
    }

    //FOR TESTING PURPOSES

    protected void Update()
    {

        if(_health < _maxHealth && _healTimer > 0)
        {
            _healTimer -= Time.deltaTime;
        }
        else if(_health < _maxHealth)
        {
            Heal(_healRate*Time.deltaTime);
        }
    }
    #endregion

    #region Methods
    public virtual void SufferDamage(float amount, Source source)
    {
        float damageRecieved = amount;

        switch (source)
        {
            case Source.PLAYER:
                damageRecieved *= _playerDamageMultiplier;
                _health -= damageRecieved;
                HurtEffect();
                break;
            case Source.ENEMY:
                damageRecieved *= _enemyDamageMultiplier;
                AudioManager.TryPlayCueAtPoint(damageSound, transform.position);
                _health -= damageRecieved;
                HurtEffect();
                    
                break;
            case Source.ENVIRONMENT:
                damageRecieved *= _environmentDamageMultiplier;
                _health -= damageRecieved;
                HurtEffect();
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
    public void Heal(float amount)
    {
        SufferDamage(-amount, Source.HEAL);
    }
    public virtual void Die()
    {
        AudioManager.TryPlayCueAtPoint(sfxDeathSound, transform.position);

        gameObject.transform.localScale = new Vector3(1f, _deathYScale, 1f);
        EventManager.OnPlayerDeath?.Invoke();

        foreach (MonoBehaviour mono in gameObject.GetComponents<MonoBehaviour>())
        {
            if (mono is Health) continue;

            if (mono is PlayerInteract) (mono as PlayerInteract).SetCanInteract(false);

            mono.enabled = false;
        }

        if (gameObject.TryGetComponent(out Rigidbody rb))
        {
            rb.drag = _deathDrag;
            rb.angularDrag = _deathAngularDrag;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(_deathImpulse, ForceMode.VelocityChange);
            rb.AddTorque(Vector3.up * Random.Range(-_deathRot, _deathRot));
        }

        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return _respawnTimer;
        SaveLoader.Instance?.Load();
    }

    public void SetRespawn(Vector3 position)
    {
        SaveLoader.Instance?.SetSpawn(position);
    }
    public void HurtEffect()
    {
        _camShake.GenerateImpulse((OptionsLoader.Instance ? OptionsLoader.Instance.CameraShake : 1f ) * _shakeForce);
        _healTimer = _timeUntilHeal;
        EventManager.OnUpdateHealth.Invoke(_health/_maxHealth);
    }
    public void HealEffect()
    {
        EventManager.OnUpdateHealth.Invoke(_health / _maxHealth);
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
