using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    #endregion

    #region UnityFunctions
    public void Awake()
    {
        _health = _maxHealth;
    }

    //FOR TESTING PURPOSES

    protected void Update()
    {
        if(_invincibleTimer > 0)
        {
            _invincibleTimer -= Time.deltaTime;
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
                    if(damageRecieved > 0) _invincibleTimer = _invulnerability;
                    break;
                case Source.ENEMY:
                    damageRecieved *= _enemyDamageMultiplier;
                    AudioManager.TryPlayCueAtPoint(damageSound, transform.position);
                    _health -= damageRecieved;
                    if (damageRecieved > 0) _invincibleTimer = _invulnerability;
                    break;
                case Source.ENVIRONMENT:
                    damageRecieved *= _environmentDamageMultiplier;
                    _health -= damageRecieved;
                    if (damageRecieved > 0) _invincibleTimer = _invulnerability;
                    break;
                case Source.HEAL:
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
