using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "RemoteBehaviour", menuName = "RPG Behaviour/Remote Detonation")]
public class RpgRemote : RpgBase
{
    #region Fields

    [Space(5), Header("Audio"), Space(3)]
    [field: SerializeField] public AudioCue ActivateSound;

    #endregion

    #region Variables

    private RocketRemoteExplosion _selectedRocket;
    private RocketRemoteExplosion _previousSelectedRocket;

    bool lastRocket = true;

    #endregion

    #region UnityFunctions

    #endregion

    #region Methods

    public override void TickUnselected()
    {
        base.TickUnselected();

        if (rockets.Count != 0)
        {
            float currentClosestAngle = 360;

            lastRocket = true;

            foreach (RocketRemoteExplosion rocket in rockets)
            {
                float angle = Vector3.Angle(_camera.transform.forward, rocket.transform.position - _camera.transform.position);

                if (angle < currentClosestAngle)
                {
                    currentClosestAngle = angle;

                    _selectedRocket = rocket;
                }
            }

            //only notify if value has changed
            if (_previousSelectedRocket != _selectedRocket) //this check ignores when you detonate the last rocket, wich moves you from missing (null)to none (also null)
            {
                _previousSelectedRocket = _selectedRocket;
                EventManager.OnUpdateSelectedRemoteRocket?.Invoke(_selectedRocket);
            }
        }
        else if (lastRocket)
        {
            lastRocket = false;
            EventManager.OnUpdateSelectedRemoteRocket?.Invoke(null);
        }
        
    }

    public override void SecondaryFire()
    {
        base.SecondaryFire();

        if (_selectedRocket != null)
        {
            _selectedRocket.Explode(_selectedRocket.transform.position, Vector3.up);
            AudioManager.TryPlayCueAtPoint(ActivateSound, _selectedRocket.transform.position);
        }
    }

    public override void FireRocketAtPosition(Vector3 target)
    {
        if(Input.GetButton(_rpgHolder._secondaryFireButtonName) && _rpgHolder._canDetonate)
        {
            _stats.Explosion.Explode(_rpgHolder._player.transform.position, Vector3.up);
        }
        else base.FireRocketAtPosition(target);
    }

    #endregion
}
