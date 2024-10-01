using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class OptionsData
{
    /*
     * Variables
     * - Inputs
     * - Game Speed
     * - Sensibility
     * - SFX Volume
     * - Music Volume
     * - Fullscreen
     */
    public KeyCode[] _inputKeys;

    public float _sensitivity;
    public float _sfxVolume;
    public float _musicVolume;
    public float _dialogueVolume;
    public float _masterVolume;

    public bool _fullscreen;
    public bool _holdGrab;
    public bool _extraHUD;
    public float _camShake;
    public float _fieldOfView;

    public OptionsData(float sense, float mstr, float sfx, float music, float dial, bool fs, KeyCode[] iK, bool hG, float cS, float fieldOfView, bool extraHUD)
    {
        _sensitivity = sense;
        _masterVolume = mstr;
        _sfxVolume = sfx;
        _musicVolume = music;
        _fullscreen = fs;
        _holdGrab = hG;
        _dialogueVolume = dial;
        _camShake = cS;

        _inputKeys = new KeyCode[Enum.GetValues(typeof(InputActions)).Length];
        if (iK != null)
            for (int i = 0; i < iK.Length; i++)
            {
                _inputKeys[i] = iK[i];
            }

        _fieldOfView = fieldOfView;
        _extraHUD = extraHUD;
    }
}


