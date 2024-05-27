using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedFilter : BarrierBase
{

    #region Fields
    [Space(5), Header("Extra Properties"), Space(3)]
    [SerializeField] private int _speedLimit;

    [Space(5), Header("Extra Components"), Space(3)]
    [SerializeField] private GameObject _speedLimitDisplay;
    #endregion

    protected override void OnValidate()
    {
        base.OnValidate();

        foreach (TextMeshPro text in _speedLimitDisplay.GetComponentsInChildren<TextMeshPro>())
        {
            text.transform.localPosition = (Vector3.up * _size.y / 2) + text.transform.forward * -0.2f;
            text.transform.localScale = Vector3.one * Mathf.Min(_size.x, _size.y);
            text.text = _speedLimit.ToString();
        }
    }


    private void OnEnable() => EventManager.OnUpdatePlayerSpeedXYZ += CheckForPlayerSpeed;

    private void OnDisable() => EventManager.OnUpdatePlayerSpeedXYZ -= CheckForPlayerSpeed;

    private void OnTriggerEnter(Collider other) => EventManager.OnUpdatePlayerSpeedXYZ -= CheckForPlayerSpeed;

    private void OnTriggerExit(Collider other) => EventManager.OnUpdatePlayerSpeedXYZ += CheckForPlayerSpeed;


    private void CheckForPlayerSpeed(float spd)
    {
        bool canPass = Mathf.RoundToInt(spd) >= _speedLimit;

        Open(canPass);
    }

}
