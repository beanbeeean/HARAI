using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class PlayerInputReader : MonoBehaviour
{
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction interactAction;

    public Vector2 MoveVector { get; private set; }
    public bool InteractPressedThisFrame { get; private set; }

    public bool isMoving;

    [Header("Action Names")]
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string interacionActionName = "Interact";

    private void Awake()
    {
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        ResolveActions();
    }


    private void Update()
    {
        MoveVector = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        InteractPressedThisFrame = interactAction != null && interactAction.WasPerformedThisFrame();

        if (MoveVector == Vector2.zero)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }


    }

    private void ResolveActions()
    {
        if (playerInput == null || playerInput.actions == null)
        {
            Debug.Log("playerInput == null || playerInput.actions == null");
            return;
        }

        moveAction = FindAction(moveActionName);
        interactAction = FindAction(interacionActionName);
    }

    private InputAction FindAction(string actionName)
    {
        if (string.IsNullOrWhiteSpace(actionName))
        {
            return null;
        }

        InputAction action = playerInput.actions.FindAction(actionName, false);
        if (action == null)
        {
            Debug.LogWarning($"[PlayerInputReader] Action 못 찾음:{actionName} ");
        }
        return action;
    }
}

