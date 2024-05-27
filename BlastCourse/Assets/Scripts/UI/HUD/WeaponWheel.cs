using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheel : MonoBehaviour
{
    [Space(5), Header("Modifiers"), Space(3)]
    [SerializeField] private float _timeScale;
    [SerializeField] private float _selectedItemSizeScale;

    [Space(5), Header("Display"), Space(3)]
    [Space(3), SerializeField] private TextMeshProUGUI _weaponTitle;
    [SerializeField] private TextMeshProUGUI _weaponDesc;

    [Space(5), Header("Menu Items"), Space(3)]
    [SerializeField] private int _startingIndex;
    [SerializeField] private MenuItem[] _menuItems;

    private List<MenuItem> _realItems = new List<MenuItem>();

    private int _currentSelection;
    private int _previousSelection;

    private Vector2 _normalizedMousePos;
    private float _currentAngle;
    private float _angleOffset;
    private float _arcSize;

    private float _idleSize;

    private float _idleFixedDeltaTime;

    [System.Serializable]
    public class MenuItem
    {
        public Image menuArc;
        public Image menuIcon;
        public Vector3 iconRotation;
        public FiringMode fireMode;
        public string WeaponName;
        [TextArea] public string weaponDescription;
        public Color idleColor;
        public Color hoverColor;
    }

    private void Start()
    {
        UpdateRealItems();
        UpdateVisuals(true);
    }

    private void OnEnable()
    {
        UpdateRealItems();
        UpdateVisuals(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Time.timeScale = _timeScale;
        _idleFixedDeltaTime = Time.fixedDeltaTime;
        Time.fixedDeltaTime *= 0.25f;
    }

    private void OnDisable()
    {
        UpdateVisuals(false);
        RPGAnimator.Instance.SetRocket(_realItems[_currentSelection].fireMode);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
        Time.fixedDeltaTime = _idleFixedDeltaTime;
    }

    private void Update()
    {
        _normalizedMousePos = new Vector2(Input.mousePosition.x - Screen.width * 0.5f, Input.mousePosition.y - Screen.height * 0.5f);
        _currentAngle = Mathf.Atan2(-_normalizedMousePos.y, _normalizedMousePos.x) * Mathf.Rad2Deg;

        _currentSelection = (int)(-((90 - _currentAngle) / _arcSize + (_realItems.Count / 2)) + _realItems.Count * 5) % _realItems.Count;

        if (_currentSelection != _previousSelection)
        {
            SelectWeapon(_currentSelection);
        }
    }

    private void SelectWeapon(int selection)
    {
        _realItems[selection].menuArc.color = _realItems[selection].hoverColor;
        _realItems[selection].menuArc.rectTransform.sizeDelta = _idleSize * _selectedItemSizeScale * Vector2.one;
        _realItems[selection].menuIcon.gameObject.GetComponent<RectTransform>().localPosition = _realItems[selection].iconRotation * _selectedItemSizeScale;
        _realItems[selection].menuIcon.gameObject.GetComponent<RectTransform>().localScale = Vector2.one * _selectedItemSizeScale;

        _realItems[_previousSelection].menuArc.color = _realItems[_previousSelection].idleColor;
        _realItems[_previousSelection].menuArc.rectTransform.sizeDelta = _idleSize * Vector2.one;
        _realItems[_previousSelection].menuIcon.gameObject.GetComponent<RectTransform>().localPosition = _realItems[_previousSelection].iconRotation;
        _realItems[_previousSelection].menuIcon.gameObject.GetComponent<RectTransform>().localScale = Vector2.one;

        _weaponTitle.text = _realItems[selection].WeaponName;
        _weaponDesc.text = _realItems[selection].weaponDescription;

        EventManager.OnSelectNewRpg?.Invoke(_realItems[selection].fireMode);

        _previousSelection = selection;
    }

    private void UpdateRealItems()
    {
        _realItems = new List<MenuItem>();

        for (int i = 0; i < SaveLoader.Instance.UnlockedRpgs.Length; i++)
        {
            if (SaveLoader.Instance.UnlockedRpgs[i])
            {
                foreach (MenuItem item in _menuItems)
                {
                    if (item.fireMode == (FiringMode)i) _realItems.Add(item);
                }
            }
        }

        _arcSize = 360 / _realItems.Count;
        _angleOffset = _arcSize - 180;

        _idleSize = _realItems[0].menuArc.rectTransform.sizeDelta.x;
    }

    private void UpdateVisuals(bool isActive)
    {
        for(int i = 0; i < _realItems.Count; i++)
        {
            //Arc
            _realItems[i].menuArc.gameObject.SetActive(isActive);
            _realItems[i].menuArc.fillAmount = 1 / (float)_realItems.Count;
            _realItems[i].menuArc.gameObject.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, (-_arcSize * (i + _realItems.Count/2)) % 360);

            //Icon
            _realItems[i].menuIcon.gameObject.SetActive(isActive);
            _realItems[i].iconRotation = Vector3.up;
            _realItems[i].iconRotation = Quaternion.Euler(0, 0, -_arcSize * ((float)i + (((float)_realItems.Count + 1) % 2 / 2))) * _realItems[i].iconRotation;
            _realItems[i].iconRotation = _realItems[i].iconRotation.normalized * 300;
            _realItems[i].menuIcon.gameObject.GetComponent<RectTransform>().localPosition = _realItems[i].iconRotation;

            //Reset Size
            _realItems[i].menuArc.color = _realItems[i].idleColor; ;
            _realItems[i].menuArc.rectTransform.sizeDelta = _idleSize * Vector2.one;
            _realItems[i].menuIcon.gameObject.GetComponent<RectTransform>().localScale = Vector2.one;
        }
    }
}
