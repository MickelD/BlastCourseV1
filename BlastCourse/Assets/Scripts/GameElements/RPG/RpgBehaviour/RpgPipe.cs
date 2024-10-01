using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PipeBehaviour", menuName = "RPG Behaviour/Pipe Grenade")]
public class RpgPipe : RpgBase
{
    #region Fields
    [Tooltip("Multiplier of pipe Xspeed added vertically")] public float VerticalForceMult;
    [Tooltip("Multiplier of player Xspeed added horizontally")] public float PlayerInertiaMult;
    public int BounceCount;
    public float FuseTime;

    private Vector2 _playerInertiaVector;

    #endregion

    #region UnityFunctions

    private void OnEnable()
    {
        EventManager.OnUpdatePlayerVelocity += UpdatePlayerVelocity;
    }

    private void OnDisable()
    {
        EventManager.OnUpdatePlayerVelocity -= UpdatePlayerVelocity;
    }

    #endregion

    #region Methods

    public override void FireRocketAtPosition(Vector3 target)
    {
        Vector3 _targetDirection = (target - _fireOrigin.position).normalized;

        RocketBase instantiatedRocket = Instantiate(_stats.RocketPrefab, _fireOrigin.transform.position, _rpgHolder.transform.rotation).GetComponent<RocketBase>();

        instantiatedRocket.rpg = this;
        instantiatedRocket.SetVelocity(_targetDirection * (_stats.RocketSpeed + _playerInertiaVector.x) 
                                    + Vector3.up * (_stats.RocketSpeed * VerticalForceMult + _playerInertiaVector.y));
    }

    private void UpdatePlayerVelocity(Vector3 vel) 
    {
        _playerInertiaVector.x = Mathf.Max(0f, Vector3.Dot(vel, _rpgHolder.transform.forward));
        _playerInertiaVector.y = Mathf.Max(0f, Vector3.Dot(vel, _rpgHolder.transform.up));

        _playerInertiaVector *= PlayerInertiaMult;
    }

    #endregion
}
