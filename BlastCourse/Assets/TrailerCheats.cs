using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class TrailerCheats : MonoBehaviour
{
    public GameObject _hud;
    public GameObject _rpg;
    public PlayerMovement _player;
    public GravityController _playerGravity;
    public GroundCheck _groundCheck;
    bool _fly;
    public Collider _playerCol;
    public Rigidbody _playerRb;
    public PlayerRotation _playerRotation;
    public Camera _cam;

    public CityDestruction _cityDestruction;
    public FinalFan _finalFan;

    Vector3 _pos;
    Vector2 _rot;
    private bool _is;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);


        _hud = FindObjectOfType<HUD>().gameObject;
        _rpg = FindObjectOfType<RPGAnimator>().gameObject;
        _player = FindObjectOfType<PlayerMovement>();
        _playerGravity = _player.GetComponent<GravityController>();
        _groundCheck = _player.GetComponent<GroundCheck>();
        _playerCol = _player.GetComponent<Collider>();
        _playerRb = _player.GetComponent<Rigidbody>();
        _playerRotation = FindObjectOfType<PlayerRotation>();
        _cam = Camera.main;
        _cityDestruction = FindObjectOfType<CityDestruction>();

        _is = true;
    }

    private void Update()
    {
        if (!_is) return;

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

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (_cityDestruction != null) _cityDestruction.Deliver(true);

            if (_finalFan != null) _finalFan.FeedFan();
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

        if (SteamIntegrator.Instance != null)
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                SteamIntegrator.Instance.ClearAll();
            }
        }

        if (_fly)
        {
            Vector3 _inputVec = new Vector2(OptionsLoader.TryGetAxisRaw(InputActions.Right, InputActions.Left, "horizontal"), OptionsLoader.TryGetAxisRaw(InputActions.Forward, InputActions.Back, "vertical"));
            _playerRb.drag = 10f;
            _playerRb.AddForce((_cam.transform.forward * _inputVec.y + _cam.transform.right * _inputVec.x).normalized * CustomMethods.ExtendedDataUtility.Select(Input.GetKey(KeyCode.LeftControl), 100f, 200f), ForceMode.Acceleration);
        }
    }
}
