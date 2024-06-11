using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : BoxVisualizer
{
    #region Fields

    [SerializeField] bool _killPlayer = true;
    [SerializeField] bool _killObjects = true;

    #endregion

    protected override Color GetColor()
    {
        return new Color(1,0,0,0.5f);
    }

    #region UnityFunctions

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Health player) && _killPlayer)
        {
            player.Die();
        }
        else if (other.TryGetComponent(out PhysicsObject physics) && _killObjects)
        {
            physics.DestroyObject();
        }
    }

    #endregion
}
