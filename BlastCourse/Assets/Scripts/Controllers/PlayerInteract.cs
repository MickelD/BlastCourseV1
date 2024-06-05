using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    #region Fields

    [Space(5), Header("Values"), Space(3)]
    [Tooltip("Is the interaction stopped when the button is lifted, or when it is pressed again"), SerializeField] bool _continuousInteraction;
    [SerializeField] float _interactRange;
    [SerializeField] string _interactButtonName;

    [Space(5), Header("Grabbing"), Space(3)]
    public float GrabDistance;
    public float GrabForce;
    public Transform GrabAnchor;
    public LayerMask IgnoreActivablesLayerMask;

    [Space(5), Header("Components"), Space(3)]
    public Transform PointerTransform;
    [SerializeField] private RpgHolder g_rpg;

    #endregion

    #region Variables

    private RaycastHit hit;

    private IInteractable _selectedInteractable;
    private IInteractable _currentInteractable;

    private bool _canInteract = true;

    #endregion

    #region Unity Functions

    private void Update()
    {
        //Debug.Log(_currentInteractable);

        CheckForInteractable();

        InteractableInput();
    }

    #endregion

    #region Methods

    private void CheckForInteractable()
    {
        if (_currentInteractable == null && _canInteract)
        {
            if (Physics.Raycast(PointerTransform.position, PointerTransform.forward, out hit, _interactRange, IgnoreActivablesLayerMask, QueryTriggerInteraction.Collide))
            {
                IInteractable hitInter = hit.collider.GetComponent<IInteractable>();

                if(hitInter != null && !hitInter.Locked) SetSelectedInteractable(hitInter);
                else SetSelectedInteractable(null);
            }
            else
            {
                SetSelectedInteractable(null);
            }
        }
        else if(_currentInteractable != null)
        {
            //RaycastHit obj;
            //if(Physics.Raycast(transform.position + Vector3.up * 1.75f,_currentInteractable.gameObject.transform.position - (transform.position + Vector3.up * 1.75f), out obj, BlockSightFromObject))
            //{
            //    Debug.Log("Oops " + _currentInteractable.gameObject.layer);
            //    if (obj.transform.gameObject != _currentInteractable.gameObject) CancelCurrentInteraction();
            //}
        }
    }
    private void SetSelectedInteractable(IInteractable candidate)
    {
        if (candidate != _selectedInteractable)
        {
            _selectedInteractable = candidate;

            EventManager.OnSelectNewInteractable?.Invoke(_selectedInteractable);
        }
    }

    private void InteractableInput()
    {
        if (OptionsLoader.TryGetKeyDown(InputActions.Interact,_interactButtonName))
        {
            if (_currentInteractable != null) //We are already interacting with sm
            {
                if (!OptionsLoader.TryGetValueFromInstance(nameof(OptionsLoader.HoldGrab), _continuousInteraction)) SetInteractWith(_currentInteractable, false);
            }
            else if (_selectedInteractable != null && (transform.parent == null || !transform.parent.Equals(_selectedInteractable.gameObject.transform))) //We are looking at a valid interactable
            {
                SetInteractWith(_selectedInteractable, true);
            }
        }
        else if (OptionsLoader.TryGetKeyUp(InputActions.Interact, _interactButtonName) && _currentInteractable != null)
        {
            if (OptionsLoader.TryGetValueFromInstance(nameof(OptionsLoader.HoldGrab), _continuousInteraction)) SetInteractWith(_currentInteractable, false);
            else if (_currentInteractable is PushButton) ((PushButton)_currentInteractable).OnInteractButtonUp();
        }
    }

    public void SetInteractWith(IInteractable interactable, bool interact)
    {
        EventManager.OnIsInteracting?.Invoke(interact);

        _currentInteractable = interact ? interactable : null;

        if (interactable is PhysicsObject)
        {
            if (g_rpg != null) g_rpg.EnableShooting(!interact);
            RPGAnimator.Instance.SetGrabbing(interact);
        }
        else if ((interactable is PickUpBox || interactable is InteractablePickUpRPG) && interact)
        {
            RPGAnimator.Instance.TakeBox();
        }
        else if (interact)
        {
            RPGAnimator.Instance.Interact();
        }

        if (interactable != null)
        {
            if (interact)
            {
                interactable.SetInteraction(true, this);
            }
            else
            {
                interactable.SetInteraction(false, null);
            }
        }
    }

    public void CancelCurrentInteraction()
    {
        SetInteractWith(_currentInteractable, false);
    }

    public void SetCanInteract(bool canInteract)
    {
        _canInteract = canInteract;
        _selectedInteractable = null;
        if (!canInteract && _currentInteractable != null) CancelCurrentInteraction();
        EventManager.OnSelectNewInteractable?.Invoke(null);
    }

    #endregion
} 
