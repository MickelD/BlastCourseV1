using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MusicStart : MonoBehaviour
{
    #region Fields

    public AudioCue music;
    public AudioSource Source;

    #endregion

    #region Vars



    #endregion

    #region UnityFunctions

    public void Start()
    {
        TryPlayCueAtPoint(music, transform.position);
        PauseMenu.Instance.OnPause += Pause;
    }
    public void OnDestroy()
    {
        PauseMenu.Instance.OnPause -= Pause;
    }

    #endregion

    #region Methods

    public void TryPlayCueAtPoint(AudioCue audioCue, Vector3 location)
    {
        //the given cue has no clip
        if (audioCue.SfxClip == null || audioCue.SfxClip.Length < 1) return;


        //set source values
        Source.clip = audioCue.SfxClip[Random.Range(0, audioCue.SfxClip.Length)];
        Source.loop = audioCue.Loop;

        Source.volume = audioCue.Volume;
        Source.pitch = audioCue.Pitch;

        Source.spatialBlend = audioCue.SpatialBlend;

        Source.outputAudioMixerGroup = audioCue.Group;

        //Play source at location
        Source.transform.position = location;

        Source.gameObject.SetActive(true);
        Source.enabled = true;
        Source.Play();
    }

    private void Pause(bool pause)
    {
        if (pause) Source.Pause();
        else Source.Play();
    }

    #endregion
}


