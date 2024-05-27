using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public struct AudioCue
{
    [Space(5), Header("Audio"), Space(3)]

    public AudioClip[] SfxClip;
    public float Volume;
    public float Pitch;
    public bool Loop;
    public AudioMixerGroup Group;
    [Range(0f, 1f)] public float SpatialBlend;
}
