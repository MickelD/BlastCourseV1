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
    int _mult = 1;

    private bool _enableRot;

    private IEnumerator Start()
    {
        _enableRot = false;
        Debug.Log("a");
        if (SaveLoader.Instance != null && SaveLoader.Instance.SpawnPos?.Length >= 4)
        {
            ResetRot(SaveLoader.Instance.SpawnPos[3], 0f);
            Debug.Log("z");
        }
        else
        {
            Debug.Log("c");
            ResetRot(0f, 0f);
        }
        Debug.Log("a2");
        yield return new WaitForSecondsRealtime(0.5f);

        Debug.Log("b");
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

        _yRot = Mathf.Repeat(mouseX, 360f) * _mult;

        _xRot = Mathf.Clamp(_xRot + mouseY, -90f, 90f) * _mult;
    }

    private void ApplyRotation()
    {
        _playerOrientation.Rotate(0f, _yRot, 0f, Space.Self);
        _targetCamera.eulerAngles = (Vector3.right * -_xRot) + (Vector3.up * _playerOrientation.localEulerAngles.y);
    }

    public void ResetRot(float x, float y)
    {
        _yRot = _xRot = 0f;
        _playerOrientation.localEulerAngles = new Vector3(0f, x, 0f);
        _targetCamera.eulerAngles = new Vector3(y, 0f, 0f);
    }

    public float GetxRot()
    {
        return _xRot;
    }

    public void LockRot(bool lck)
    {
        _mult = lck ? 0 : 1;
    }
}
