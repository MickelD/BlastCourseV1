using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    #region Fields

    public AudioSource introSource;
    public AudioSource[] loopSource;

    public float loopStart;
    public float loopEnd;

    public float NormalVolume;
    public float DecreasedVolume;

    #endregion

    #region Vars

    bool _evenLoop = false;
    bool _isTalking = false;
    WaitForSecondsRealtime startL;
    WaitForSecondsRealtime endL;

    #endregion

    #region UnityFunctions

    public void Start()
    {
        PauseMenu.Instance.OnPause += LowerVolume;
        DialogueManager.Instance.ActivateSpeakers += Talking;

        LowerVolume(false);
        if (introSource != null) introSource.Play();
        _evenLoop = false;
        
        startL = new WaitForSecondsRealtime(loopStart);
        if (loopEnd <= 0) endL = new WaitForSecondsRealtime(loopSource[0].clip.length);
        else endL = new WaitForSecondsRealtime(loopEnd);

        StartCoroutine(StartLoop());
    }

    public void OnDestroy()
    {
        PauseMenu.Instance.OnPause -= LowerVolume;
        DialogueManager.Instance.ActivateSpeakers -= Talking;
    }

    IEnumerator Loop()
    {
        yield return endL;

        if (_evenLoop)
        {
            loopSource[1].Play();
        }
        else
        {
            loopSource[0].Play();
        }

        _evenLoop = !_evenLoop;
        StartCoroutine(Loop());
    }

    IEnumerator StartLoop()
    {
        yield return startL;

        loopSource[0].Play();
        _evenLoop=true;

        StartCoroutine(Loop());
    }


    public void LowerVolume(bool down)
    {
        if (down)
        {
            if (introSource != null) introSource.volume = DecreasedVolume;
            foreach(AudioSource s in loopSource) s.volume = DecreasedVolume;
        }
        else if (!_isTalking)
        {
            if (introSource != null) introSource.volume = NormalVolume;
            foreach (AudioSource s in loopSource) s.volume = NormalVolume;
        }
    }

    public void Talking(bool talk)
    {
        _isTalking = talk;
        LowerVolume(talk);
    }

    #endregion
}


