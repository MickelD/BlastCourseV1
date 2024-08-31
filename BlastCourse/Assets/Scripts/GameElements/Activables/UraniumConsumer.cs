using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//Consume me papa, for I have sinned
public class UraniumConsumer : ActivableBase
{
    #region Fields

    [Space(5), Header("Variables"), Space(3)]
    public FinalFan _FinalFan;
    public Transform _BoxPos;
    public Animator _Animator;
    public Transform _SpawnPos;

    [Space(5), Header("Audio"), Space(3)]
    public AudioCue _consumeSfx;

    
    bool _fed = false;

    #endregion

    private void OnEnable()
    {
        if (_fed) _Animator.Play("anFeeder", 0, 1f);
    }

    protected override void Start()
    {
        base.Start();

        _Animator.speed = 0f;
    }

    #region Collisions && Triggers

    private void OnTriggerEnter(Collider other)
    {
        if (!_fed && other.TryGetComponent(out UraniumBox box))
        {
            _fed = true;

            AudioManager.TryPlayCueAtPoint(_consumeSfx, transform.position);
            box.Consume(_BoxPos);
            _Animator.speed = 1f;
            SaveLoader.Instance.SetSpawn(_SpawnPos.transform.position);
            _FinalFan.FeedFan();
            SendAllActivations(true);
        }
    }

    public void MarkAsFed()
    {
        _fed = true;
        _FinalFan.FeedFan(true);
        if (gameObject.activeInHierarchy) _Animator.Play("anFeeder", 0, 1f);
    }

    #endregion
}


