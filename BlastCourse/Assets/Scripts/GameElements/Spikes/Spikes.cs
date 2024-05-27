using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Health player = other.GetComponent<Health>();

        if(player != null)
        {
            player.Die();
        }
    }
}
