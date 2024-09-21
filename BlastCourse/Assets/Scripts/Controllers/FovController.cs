using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FovController : MonoBehaviour
{
    public static FovController instance;
    private void Awake()
    {
        if(instance == null)instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SetFov(OptionsLoader.Instance.FieldOfView);
    }

    [SerializeField] private float _fov;
    [SerializeField] private CinemachineVirtualCamera _mainCam;

    public void SetFov(float value)
    {
        _mainCam.m_Lens.FieldOfView = value;
        _fov = value;
    }


}


