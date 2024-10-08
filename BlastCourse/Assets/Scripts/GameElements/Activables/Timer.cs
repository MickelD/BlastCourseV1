using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Timer : ActivableBase
{
    #region Fields

    [Space(5), Header("Timer"), Space(3)]
    public float Delay;
    public TextMeshPro DisplayText;

    [Tooltip("Send Events when on the last seconds")] public bool FinalCountdown;
    [ExcludeFromActivableEditor(nameof(FinalCountdown))] public float CountdownStartAt;
    [ExcludeFromActivableEditor(nameof(FinalCountdown))] public float CountdownInterval;
    [ExcludeFromActivableEditor(nameof(FinalCountdown))] public AudioCue CountdownSound;

    #endregion

    #region Vars
    private float _countDown;
    public float CountDown
    {
        get { return _countDown; }
        set
        {
            _countDown = Mathf.Max(0f, value);

            if (DisplayText != null)
            {
                int mm = Mathf.FloorToInt(_countDown / 60);
                int ss = Mathf.FloorToInt(_countDown % 60);
                DisplayText.text = string.Format("{0:00}:{1:00}", mm, ss);
            }

            //foreach (Timer timer in MimicTimers) timer.CountDown = value;
        }
    }

    private bool _paused = true;
    private float _nextCountDownFlag;
    private bool _activeTimerFunc;

    #endregion

    #region UnityFunctions

    private void OnValidate()
    {
        CountDown = Delay;
    }

    protected override void Start()
    {
        if (IsMimic())
        {
            Delay = transform.parent.GetComponent<Timer>().Delay;
        }

        base.Start();
    }

    #endregion

    #region Methods

    #region ActivableActions

    [ActivableAction]
    public void Play(bool play)
    {
        if (play && !_activeTimerFunc) TimerFunction();
        _paused = !play;
    }

    [ActivableAction]
    public void Reset(bool playAfter)
    {
        CountDown = Delay;
        Play(playAfter);
    }

    #endregion

    private void CountDownStep()
    {
        AudioManager.TryPlayCueAtPoint(CountdownSound, transform.position);
    }

    private async void TimerFunction()
    {
        _activeTimerFunc = true;
        _nextCountDownFlag = CountdownStartAt + CountdownInterval;

        while (CountDown > 0f)
        {
            CountDown -= Time.deltaTime * (!_paused).GetHashCode();

            if(FinalCountdown && CountDown <= _nextCountDownFlag)
            {
                CountDownStep();
                _nextCountDownFlag -= CountdownInterval;
            }

            if (this == null || gameObject == null) return;

            await Task.Yield();
        }

        CountDown = Delay;
        _activeTimerFunc = false;
        SendAllActivations(true);
    }

    #endregion
}
