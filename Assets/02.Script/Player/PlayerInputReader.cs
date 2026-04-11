using UnityEngine;
using UnityEngine.InputSystem;
using System; // Action 이벤트를 위해 필요합니다.

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputReader : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction interactAction;

    // 외부에서 읽어갈 데이터들
    public Vector2 MoveVector { get; private set; }
    public bool InteractPressed { get; private set; } // 현재 버튼이 눌려있는 상태인지 (채널링용)
    public bool isMoving { get; private set; }

    // 다른 스크립트에서 구독할 이벤트들
    public event Action InteractStartedEvent;  // 버튼을 누른 순간 (문 이동 등)
    public event Action InteractCanceledEvent; // 버튼에서 손을 뗀 순간 (정화 중단 등)

    [Header("Action Names")]
    [SerializeField] private string moveActionName = "Move";
    [SerializeField] private string interacionActionName = "Interact";

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        ResolveActions();
    }

    private void OnEnable()
    {
        // New Input System의 콜백(started, canceled)을 연결합니다.
        if (interactAction != null)
        {
            interactAction.started += OnInteractStarted;
            interactAction.canceled += OnInteractCanceled;
        }
    }

    private void OnDisable()
    {
        // 메모리 누수 방지를 위해 연결을 해제합니다.
        if (interactAction != null)
        {
            interactAction.started -= OnInteractStarted;
            interactAction.canceled -= OnInteractCanceled;
        }
    }

    private void Update()
    {
        // 매 프레임 이동 값과 버튼 유지 상태를 업데이트합니다.
        MoveVector = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        isMoving = MoveVector != Vector2.zero;

        // 버튼이 '유지'되고 있는지 체크 (채널링 게이지 상승 등에 사용)
        InteractPressed = interactAction != null && interactAction.IsPressed();
    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        // 이벤트를 구독한 델리게이트가 있다면 실행합니다.
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