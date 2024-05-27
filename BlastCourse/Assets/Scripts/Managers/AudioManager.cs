using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    #region Singleton Framework
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    #region Variables

    private List<AudioSource> _usableAudioSources;
    [HideInInspector] public AudioSource activeAudioSource;

    #endregion

    #region Methods

    private void Start()
    {
        _usableAudioSources = new List<AudioSource>();

        foreach (Transform child in transform)
        {
            _usableAudioSources.Add(child.GetComponent<AudioSource>());

            child.GetComponent<AudioSource>().volume = 1;
            child.gameObject.SetActive(false);
        }
    }


    public static AudioSource TryPlayCueAtPoint(AudioCue audioCue, Vector3 location)
    {

        //no Instance
        if (Instance == null) return null;

        //the given cue has no clip
        if (audioCue.SfxClip == null || audioCue.SfxClip.Length < 1) return null;

        AudioSource selectedAudioSource;
        if (Instance._usableAudioSources.Count > 0) //we have available audiosources
        {
            selectedAudioSource = Instance._usableAudioSources[0];
            Instance._usableAudioSources.Remove(selectedAudioSource);
        }
        else //we ran out of audio sources, so we should instantiate a new one
        {
            AudioSource newSource = new GameObject("Audio Source (" + (Instance._usableAudioSources.Count - 1) + ")", typeof(AudioSource)).GetComponent<AudioSource>();

            newSource.transform.parent = Instance.transform;
            selectedAudioSource = newSource;
        }
        Instance.activeAudioSource = selectedAudioSource;


        //set source values
        selectedAudioSource.clip = audioCue.SfxClip[Random.Range(0, audioCue.SfxClip.Length)];
        selectedAudioSource.loop = audioCue.Loop;

        selectedAudioSource.volume = audioCue.Volume;
        selectedAudioSource.pitch = audioCue.Pitch;

        selectedAudioSource.spatialBlend = audioCue.SpatialBlend;

        selectedAudioSource.outputAudioMixerGroup = audioCue.Group;

        //Play source at location
        selectedAudioSource.transform.position = location;

        selectedAudioSource.gameObject.SetActive(true);
        selectedAudioSource.enabled = true;
        selectedAudioSource.Play();

        if (!audioCue.Loop && Instance != null)
        {
            Instance.StartCoroutine(Instance.PlayingAudio(selectedAudioSource, selectedAudioSource.clip.length));
        }

        return selectedAudioSource;

    }


    private IEnumerator PlayingAudio(AudioSource audioSource, float duration)
    {
        yield return new WaitForSeconds(duration);

        //Return to original location
        audioSource.transform.position = new Vector3(0, 0, 0);
        _usableAudioSources.Add(audioSource);
        audioSource.gameObject.SetActive(false);
    }

    public bool AudioPlaying(AudioSource source)
    {
        if (source == null) return false;
        return !_usableAudioSources.Contains(source);
    }

    #endregion



}
