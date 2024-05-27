using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    [SerializeField] AudioCue _sound;

    private void OnTriggerEnter(Collider other)
    {
        AudioManager.TryPlayCueAtPoint(_sound, transform.position);
    }
}
