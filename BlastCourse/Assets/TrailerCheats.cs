using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrailerCheats : MonoBehaviour
{
    [SerializeField] UnityEvent OnEntry;
    [SerializeField] UnityEvent OnExit;

    GameObject _hud;
    GameObject _rpg;
    PlayerMovement _player;
    GravityController _playerGravity;
    GroundCheck _groundCheck;
    bool _fly;
    Collider _playerCol;
    Rigidbody _playerRb;
    PlayerRotation _playerRotation;
    Camera _cam;

    Vector3 _pos;
    Vector2 _rot;

    private void OnTriggerEnter(Collider other)
    {
        OnEntry?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        OnExit?.Invoke();
    }


    private void Start()
    {
        _hud = FindObjectOfType<HUD>().gameObject;
        _rpg = FindObjectOfType<RPGAnimator>().gameObject;
        _player = FindObjectOfType<PlayerMovement>();
        _playerGravity = _player.GetComponent<GravityController>();
        _groundCheck = _player.GetComponent<GroundCheck>();
        _playerCol = _player.GetComponent<Collider>();
        _playerRb = _player.GetComponent<Rigidbody>();
        _playerRotation = FindObjectOfType<PlayerRotation>();
        _cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            _hud.SetActive(!_hud.activeInHierarchy);
            _rpg.SetActive(!_rpg.activeInHierarchy);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            _fly = !_fly;
            _playerCol.isTrigger = _fly;
            _player.enabled = !_fly;
            _playerGravity.Scale = _fly ? 0f : 1f;
            _groundCheck.enabled = !_fly;
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            _playerRotation.ResetRot(0f, 0f);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            _playerRotation.ResetRot(90f, 0f);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            _playerRotation.ResetRot(180f, 0f);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            _playerRotation.ResetRot(270f, 0f);
        }


        if (Input.GetKeyDown(KeyCode.F5) && Physics.Raycast(_cam.transform.position, _cam.transform.forward, out RaycastHit hitInfo, 100f))
        {
            hitInfo.collider.gameObject.SetActive(false);
        }

        if (_fly)
        {
            Vector3 _inputVec = new Vector2(OptionsLoader.TryGetAxisRaw(InputActions.Right, InputActions.Left, "horizontal"), OptionsLoader.TryGetAxisRaw(InputActions.Forward, InputActions.Back, "vertical"));
            _playerRb.drag = 10f;
            _playerRb.AddForce((_cam.transform.forward * _inputVec.y + _cam.transform.right * _inputVec.x).normalized * CustomMethods.ExtendedDataUtility.Select(Input.GetKey(KeyCode.LeftControl), 100f, 200f), ForceMode.Acceleration);
        }
    }
}
