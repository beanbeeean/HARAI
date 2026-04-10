using UnityEngine;

public class CommonMonsterAnimator : MonoBehaviour
{
    private Animator animator;
    private EnemyFSMController controller;

    [Header("Animator Parameters")]
    [SerializeField] private string dirXParameterName = "DirX";
    [SerializeField] private string dirYParameterName = "DirY";

    private int dirXHash;
    private int dirYHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponentInParent<EnemyFSMController>();

        dirXHash = Animator.StringToHash(dirXParameterName);
        dirYHash = Animator.StringToHash(dirYParameterName);
    }

    private void Update()
    {
        if(animator ==null || controller == null) return;

        if (controller.currentState == EnemyState.Attack)
        {
            animator.speed = 0f;
            return;
        }

        Vector2 direction = controller.CurrentVelocity;

        if (direction.sqrMagnitude > 0.001f)
        {
            animator.SetFloat(dirXHash, direction.x);
            animator.SetFloat(dirYHash, direction.y);
        }
    }

}
