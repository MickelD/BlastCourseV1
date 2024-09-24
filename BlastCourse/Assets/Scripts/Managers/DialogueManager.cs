using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueManager : MonoBehaviour
{
    #region Singleton Framework
    public static DialogueManager Instance { get; private set; }

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

    #region Fields

    public List<AudioCue> Interruptions;
    public AudioSource Source;

    #endregion

    #region Vars

    bool _dialoguePlaying;

    public delegate void SpeakerDelegate(bool isSpeaking);
    public SpeakerDelegate ActivateSpeakers;

    #endregion

    #region UnityFunctions

    public void Start()
    {
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
        if (!_dialoguePlaying || Interruptions.Count <= 0)
        {
            if(_dialoguePlaying) StopAudio();

            _dialoguePlaying = true;

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

            if (!audioCue.Loop && Instance != null)
            {
                Instance.StartCoroutine(Instance.PlayingAudio(Source.clip.length));
            }
        }
        else
        {
            //Stop Current Dialogue
            StopAudio();

            //Select Random Interruption
            AudioCue inter = Interruptions[Random.Range(0, Interruptions.Count)];

            //Play fisrt an interruption
            _dialoguePlaying = true;

            //the given cue has no clip
            if (inter.SfxClip == null || inter.SfxClip.Length < 1) return;


            //set source values
            Source.clip = inter.SfxClip[Random.Range(0, inter.SfxClip.Length)];
            Source.loop = inter.Loop;

            Source.volume = inter.Volume;
            Source.pitch = inter.Pitch;

            Source.spatialBlend = inter.SpatialBlend;

            Source.outputAudioMixerGroup = inter.Group;

            //Play source at location
            Source.transform.position = location;

            Source.gameObject.SetActive(true);
            Source.enabled = true;
            Source.Play();


            //Play audio
            if (!inter.Loop && Instance != null)
            {
                Instance.StartCoroutine(Instance.PlayingInterruption(Source.clip.length,audioCue));
            }
        }
    }

    private IEnumerator PlayingAudio(float duration)
    {
        ActivateSpeakers.Invoke(true);

        yield return new WaitForSeconds(duration);

        //Return to original location
        Source.transform.position = new Vector3(0, 0, 0);
        Source.gameObject.SetActive(false);
        _dialoguePlaying = false;
        ActivateSpeakers.Invoke(false);
    }

    private IEnumerator PlayingInterruption(float duration, AudioCue savedDialogue)
    {
        yield return new WaitForSeconds(duration);

        //Return to original location
        Source.transform.position = new Vector3(0, 0, 0);

        //the given cue has no clip
        if (savedDialogue.SfxClip != null && savedDialogue.SfxClip.Length >= 1)
        {
            //set source values
            Source.clip = savedDialogue.SfxClip[Random.Range(0, savedDialogue.SfxClip.Length)];
            Source.loop = savedDialogue.Loop;

            Source.volume = savedDialogue.Volume;
            Source.pitch = savedDialogue.Pitch;

            Source.spatialBlend = savedDialogue.SpatialBlend;

            Source.outputAudioMixerGroup = savedDialogue.Group;

            Source.Play();

            if (!savedDialogue.Loop && Instance != null)
            {
                Instance.StartCoroutine(Instance.PlayingAudio(Source.clip.length));
            }
        }
    }

    private void StopAudio()
    {
        StopAllCoroutines();
        Source.transform.position = new Vector3(0, 0, 0);
        Source.gameObject.SetActive(false);
        _dialoguePlaying = false;
        ActivateSpeakers.Invoke(false);
    }

    private void Pause(bool pause)
    {
        if (pause) Source.Pause();
        else Source.Play();
    }

    [ContextMenu("Give IDs")]
    private void SearchForDialogues()
    {
        int a = 0;

        ActivableDIalogue[] aDial = GameObject.FindObjectsOfType<ActivableDIalogue>();
        if(aDial != null && aDial.Length > 0)
            for(int i = 0; i < aDial.Length; i++)
            {
                a= i;
                aDial[i].Id = "s" + SceneManager.GetActiveScene().buildIndex + "d" + a;
            }
        DialogueTrigger[] tDial = GameObject.FindObjectsOfType<DialogueTrigger>();
        if (tDial != null && tDial.Length > 0)
            for (int i = 0; i < tDial.Length; i++)
            {
                a = i;
                tDial[i].Id = "s" + SceneManager.GetActiveScene().buildIndex + "d" + a;
            }
        DialogueStart[] sDial = GameObject.FindObjectsOfType<DialogueStart>();
        if (sDial != null && sDial.Length > 0)
            for (int i = 0; i < sDial.Length; i++)
            {
                a = i;
                sDial[i].Id = "s" + SceneManager.GetActiveScene().buildIndex + "d" + a;
            }
    }

    #endregion
}


