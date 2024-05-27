using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RPG Stats", menuName = "Game Rules/RPG Stats"), System.Serializable]
public class RpgStats : ScriptableObject
{
    [field: Header("Launcher Stats")]
    [field: SerializeField] public FiringMode FireMode { get; private set; }

    [field: SerializeField] public bool Unlocked;

    [field: SerializeField] public int ClipSize { get; private set; }
    [field: SerializeField] public float EnergyRecoveryMultiplier { get; private set; }

    [field: Header("Rocket Stats")]
    [field: SerializeField] public int ActiveRocketCap { get; private set; }
    [SerializeField] public GameObject RocketPrefab;
    [field: SerializeField] public float RocketSpeed { get; private set; }

    [Header("Explosion Stats")]
    [SerializeField] public Explosion Explosion;

    [Header("Visuals")]
    [SerializeField] public RpgVisuals AssociatedVisuals;

    [Header("Audio")]
    [SerializeField] public AudioCue ShootingSound;
}

[System.Serializable]
public class RpgVisuals
{
    [SerializeField] public Color LightColor;
    [SerializeField] public Color DarkColor;
    [SerializeField, ColorUsage(true, true)] public Color EmissiveColor;

    [SerializeField] public Sprite RpgIcon;
    [SerializeField] public Sprite RocketIcon;
}
