using UnityEngine;

public class MainMonsterAnimator : MonoBehaviour
{
    private Animator animator;
    private MainMonster controller;

    [Header("Animator Parameters")]
    [SerializeField] private string dirXParameterName = "DirX";
    [SerializeField] private string dirYParameterName = "DirY";
    private int dirXHash;
    private int dirYHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponentInParent<MainMonster>();

        dirXHash = Animator.StringToHash(dirXParameterName);
        dirYHash = Animator.StringToHash(dirYParameterName);
    }



    private void Update()
    {
        if(animator == null || controller == null) return;

        if (controller.currentState == EnemyState.Attack || controller.currentState == EnemyState.Stun)
        {
            animator.speed = 0f;
            return;
        }
        else
        {
            animator.speed = 1f;
        }

            Vector2 direction = controller.CurrentVelocity;

        if (direction.sqrMagnitude > 0.001f)
        {
            float inputX = direction.x > 0.1f ? 1f : (direction.x < -0.1f ? -1f : 0f);
            float inputY = direction.y > 0.1f ? 1f : (direction.y < -0.1f ? -1f : 0f);

            animator.SetFloat(dirXHash, inputX);
            animator.SetFloat(dirYHash, inputY);

        }
    }

}
