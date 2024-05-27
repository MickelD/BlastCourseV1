using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPad : MonoBehaviour
{
    #region Fields
    [Space(5) ,Header("Variables"), Space(3)]
    [SerializeField] private float _pullStrength = 25;

    #endregion

    #region Trigger/Collision Methods
    private void OnTriggerStay(Collider other)
    {
        PlayerMovement playerCheck = other.GetComponent<PlayerMovement>();
        if(playerCheck != null)
        {
            if(!playerCheck.GetGraviPad()) playerCheck.SetGraviPad(true);
            playerCheck.PushPull(-transform.up * _pullStrength);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerMovement playerCheck = other.GetComponent<PlayerMovement>();
        if (playerCheck != null)
        {
            playerCheck.SetGraviPad(false);
        }
    }
    #endregion
}
