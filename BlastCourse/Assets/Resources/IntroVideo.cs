using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.Audio;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class IntroVideo : MonoBehaviour
{
    [SerializeField] VideoPlayer _videoPlayer;
    [SerializeField] MainMenu _mainMenu;
    [SerializeField] float _skipSpeed;
    [SerializeField] Image _skipIndicator;
    [SerializeField] GameObject _video;
    [SerializeField] GameObject _music;

    private float _skip;

    private void Start()
    {
        _skip = 0;
        _mainMenu.enabled = false;
        _music.gameObject.SetActive(false);
        _videoPlayer.loopPointReached += EndVideo;

        if (OptionsLoader.Instance != null)
        {
            _videoPlayer.SetDirectAudioVolume(0, OptionsLoader.Instance.MasterVolume);
        }

        if (Time.time > 5f) //only play intro animation on first load (yes, I know this is lazy, but in my country we call that smart, sue me)
        {
            EndVideo(_videoPlayer);
        }
    }


    private void Update()
    {
        _skip = Input.anyKey ? _skip + _skipSpeed * Time.deltaTime : 0f;

        _skipIndicator.fillAmount = _skip;
        _skipIndicator.rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, _skip);

        if (_skip >= 1f)
        {
            EndVideo(_videoPlayer);
        }
    }

    private void EndVideo(VideoPlayer vp)
    {
        _videoPlayer.loopPointReached -= EndVideo;
        _videoPlayer.gameObject.SetActive(false);
        _mainMenu.enabled = true;
        _video.SetActive(false);
        _music.gameObject.SetActive(true);
    }
}


