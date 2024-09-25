using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FinalCredits : MonoBehaviour
{
    [SerializeField] UnityEvent _onApproach;
    [SerializeField] UnityEvent _onEntry;
    [SerializeField] PlayerRotation _playerRot;
    [SerializeField] Animator _animator;
    [SerializeField] Transform _endScenePos;
    [SerializeField] Vector3 _offSet;
    [SerializeField] float _speedFactor;
    private int _entries;
    private bool _creditsEnded;

    private void OnTriggerEnter(Collider other)
    {
        _entries++;

        if (_entries == 1)
        {
            _onApproach.Invoke();
        }
        else if (_entries == 2) 
        { 
            _onEntry.Invoke();
        }
    }

    private void Update()
    {
        _animator.speed = !_creditsEnded && Input.anyKey ? _speedFactor : 1f;
    }

    private void OnTriggerExit(Collider other)
    {
        _entries--;
    }

    public void TeleportPlayer()
    {
        _playerRot.transform.position = _endScenePos.transform.position + _offSet;
        _playerRot.ResetRot(180f, 0f);
        _playerRot.transform.eulerAngles = Vector3.zero;
        _creditsEnded = true;
    }

    public void AllowCamMovement()
    {
        _playerRot.LockRot(false);
    }

    public void EndGame()
    {
        if (LoadingScreenManager.instance != null) LoadingScreenManager.instance.LoadScene(1);
        else SceneManager.LoadScene(1);
    }
}


