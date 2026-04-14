using UnityEngine;
using UnityEngine.InputSystem;
using System; 
[RequireComponent(typeof(PlayerInput))]
public class PlayerInputReader : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction interactAction;
    private InputAction pickUpAction;
    private InputAction inv1Action;
    private InputAction inv2Action;
    private InputAction inv3Action;
    private InputAction flashlightPowerAction;
    private InputAction flashlightModeAction;
    private InputAction curseListOpenAction;
    private InputAction curseListCloseAction;

    public Vector2 MoveVector { get; private set; }
    public bool InteractPressed { get; private set; }
    public bool isMoving { get; private set; }

    public event Action InteractStartedEvent;
    public event Action InteractCanceledEvent;
    public event Action PickUpStartedEvent;
    public event Action<int> OnInventoryUsed;
    public event Action<int> OnInventoryDropped;
    public event Action FlashlightPowerStartedEvent;
    public event Action FlashlightModeStartedEvent;
    public event Action CurseListOpenEvent;
    public event Action CurseListCloseEvent;

    

    [Header("Action Names")]
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string interacionActionName = "Interact";
    [SerializeField] private string pickUpActionName = "PickUp";
    [SerializeField] private string inventory1ActionName = "Inventory1";
    [SerializeField] private string inventory2ActionName = "Inventory2";
    [SerializeField] private string inventory3ActionName = "Inventory3";
    [SerializeField] private string flashlightPowerActionName = "FlashlightPower";
    [SerializeField] private string flashlightModeActionName = "FlashlightMode";
    [SerializeField] private string curseListOpenActionName = "CurseListOpen";
    [SerializeField] private string curseListCloseActionName = "CurseListClose";


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        ResolveActions();
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.started += OnInteractStarted;
            interactAction.canceled += OnInteractCanceled;

        }

        if (inv1Action != null) inv1Action.started += ctx => HandleInventoryInput(0);
        if (inv2Action != null) inv2Action.started += ctx => HandleInventoryInput(1);
        if (inv3Action != null) inv3Action.started += ctx => HandleInventoryInput(2);

        if (pickUpAction != null)
        {
            pickUpAction.started += OnPickUpStarted;
        }

        if (flashlightPowerAction != null)
            flashlightPowerAction.started += OnFlashlightPowerStarted;

        if (flashlightModeAction != null)
            flashlightModeAction.started += OnFlashlightModeStarted;

        if(curseListOpenAction != null)
        {
            curseListOpenAction.started += OnCurseListOpenStarted;
        }

        if (curseListCloseAction != null)
        {
            curseListCloseAction.started += OnCurseListCloseStarted;
        }

    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.started -= OnInteractStarted;
            interactAction.canceled -= OnInteractCanceled;
        }

        if (pickUpAction != null)
        {
            pickUpAction.started -= OnPickUpStarted;
        }

        if (flashlightPowerAction != null)
        {
            flashlightPowerAction.started -= OnFlashlightPowerStarted;
        }

        if (flashlightModeAction != null)
        {
            flashlightModeAction.started -= OnFlashlightModeStarted;
        }

        if (curseListOpenAction != null)
        {
            curseListOpenAction.started -= OnCurseListOpenStarted;
        }

        if (curseListCloseAction != null)
        {
            curseListCloseAction.started -= OnCurseListCloseStarted;
        }
    }

    public void SetUIInput(bool isUI)
    {
        if (isUI)
        {
            playerInput.SwitchCurrentActionMap("UI");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("Player");
        }
    }

    private void OnCurseListCloseStarted(InputAction.CallbackContext context)
    {
        Debug.Log("OnCurseListCloseStarted 이벤트 호출");
        CurseListCloseEvent?.Invoke();
    }

    private void OnCurseListOpenStarted(InputAction.CallbackContext context)
    {
        Debug.Log("OnCurseListOpenStarted 이벤트 호출");
        CurseListOpenEvent?.Invoke();
    }

    private void OnFlashlightPowerStarted(InputAction.CallbackContext context)
    {
        FlashlightPowerStartedEvent?.Invoke();
    }

    private void OnFlashlightModeStarted(InputAction.CallbackContext context)
    {
        FlashlightModeStartedEvent?.Invoke();
    }

    private void OnPickUpStarted(InputAction.CallbackContext context)
    {
        PickUpStartedEvent?.Invoke();
    }

    private void HandleInventoryInput(int index)
    {
        bool isAltPressed = Keyboard.current.leftAltKey.isPressed || Keyboard.current.rightAltKey.isPressed;

        if (isAltPressed)
            OnInventoryDropped?.Invoke(index);
        else
            OnInventoryUsed?.Invoke(index);
    }

    private void Update()
    {
        MoveVector = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        isMoving = MoveVector != Vector2.zero;

        InteractPressed = interactAction != null && interactAction.IsPressed();
    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        InteractStartedEvent?.Invoke();
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        InteractCanceledEvent?.Invoke();
    }

    private void ResolveActions()
    {
        if (playerInput == null || playerInput.actions == null) return;

        moveAction = FindAction(moveActionName);
        interactAction = FindAction(interacionActionName);
        pickUpAction = FindAction(pickUpActionName);
        inv1Action = FindAction(inventory1ActionName); 
        inv2Action = FindAction(inventory2ActionName);
        inv3Action = FindAction(inventory3ActionName);
        flashlightPowerAction = FindAction(flashlightPowerActionName);
        flashlightModeAction = FindAction(flashlightModeActionName);
        curseListOpenAction = FindAction(curseListOpenActionName);
        curseListCloseAction = FindAction(curseListCloseActionName);
    }

    private InputAction FindAction(string actionName)
    {
        if (string.IsNullOrWhiteSpace(actionName)) return null;

        InputAction action = playerInput.actions.FindAction(actionName, false);
        if (action == null)
        {
            Debug.LogWarning($"[PlayerInputReader] Action을 찾을 수 없습니다: {actionName}");
        }
        return action;
    }
}