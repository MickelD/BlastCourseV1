using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Header("Stats"), Space(3)]
    [SerializeField] float _horSensitivity;
    [SerializeField] float _verVensitivity;
    [SerializeField] float _verRotationLimit;

    [Space(5), Header("Components"), Space(3)]
    [SerializeField] Transform _playerOrientation;
    [SerializeField] Transform _targetCamera;

    float _xRot;
    float _yRot;

    private bool _enableRot;

    private IEnumerator Start()
    {
        _enableRot = false;

        _playerOrientation.rotation = Quaternion.identity;
        _targetCamera.eulerAngles = Vector3.zero;

        yield return new WaitForSecondsRealtime(0.25f);

        _enableRot = true;
    }

    private void Update()
    {
        CalculateDesiredRotation();
        ApplyRotation();
    }

    private void CalculateDesiredRotation()
    {
        float configSens = OptionsLoader.TryGetValueFromInstance(nameof(OptionsLoader.Sensitivity), 1f);
        float mouseX = Input.GetAxisRaw("Mouse X") * _horSensitivity * Time.deltaTime * configSens * _enableRot.GetHashCode();
        float mouseY = Input.GetAxisRaw("Mouse Y") * _verVensitivity * Time.deltaTime * configSens * _enableRot.GetHashCode();

        _yRot = Mathf.Repeat(mouseX, 360f);

        _xRot = Mathf.Clamp(_xRot + mouseY, -90f, 90f);
    }

    private void ApplyRotation()
    {
        _playerOrientation.Rotate(0f, _yRot, 0f, Space.Self);
        _targetCamera.eulerAngles = (Vector3.right * -_xRot) + (Vector3.up * _playerOrientation.localEulerAngles.y);
    }

    public float GetxRot()
    {
        return _xRot;
    }
}
