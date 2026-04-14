using UnityEngine;

public class MainMonsterAnimator : MonoBehaviour
{
    private Animator animator;
    private MainMonster controller;

    [Header("Animator Parameters")]
    [SerializeField] private string dirXParameterName = "DirX";
    [SerializeField] private string dirYParameterName = "DirY";
    [SerializeField] private string isMovingParameterName = "IsMoving";
    private int dirXHash;
    private int dirYHash;
    private int isMovingHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponentInParent<MainMonster>();

        dirXHash = Animator.StringToHash(dirXParameterName);
        dirYHash = Animator.StringToHash(dirYParameterName);
        isMovingHash = Animator.StringToHash(isMovingParameterName);
    }



    private void Update()
    {
        if (animator == null || controller == null) return;

        bool isMoving = (controller.currentState != EnemyState.Idle &&
                         controller.currentState != EnemyState.Attack &&
                         controller.currentState != EnemyState.Stun);

        animator.SetBool(isMovingHash, isMoving);
        animator.speed = 1f;

        Vector2 velocity = controller.CurrentVelocity;
        if (velocity.sqrMagnitude > 0.01f)
        {
            float inputX = velocity.x > 0.1f ? 1f : (velocity.x < -0.1f ? -1f : 0f);
            float inputY = velocity.y > 0.1f ? 1f : (velocity.y < -0.1f ? -1f : 0f);

            animator.SetFloat(dirXHash, inputX);
            animator.SetFloat(dirYHash, inputY);
        }
    }

}
