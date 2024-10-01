using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuBG : MonoBehaviour
{
    #region Fields

    [SerializeField] private float _jumpSize;
    [SerializeField] private float _maxMovement;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private RectTransform _rectTransform;

    #endregion

    #region Vars

    private Vector2 _originalPos; 

    #endregion

    #region UnityFunctions

    private void Start()
    {
        _originalPos = _rectTransform.anchoredPosition;
    }

    private void Update()
    {
        _rectTransform.anchoredPosition += new Vector2((_moveSpeed * Time.deltaTime * _rectTransform.up).x, (_moveSpeed * Time.deltaTime * _rectTransform.up).y);

        float _yDistance = (_rectTransform.anchoredPosition - _originalPos).magnitude;
        if(_yDistance >= _maxMovement)
        {
            _rectTransform.anchoredPosition -= new Vector2((_jumpSize * _rectTransform.up).x, (_jumpSize * _rectTransform.up).y);
        }
    }

    #endregion

    #region Methods



    #endregion

#if UNITY_EDITOR



#endif
}


