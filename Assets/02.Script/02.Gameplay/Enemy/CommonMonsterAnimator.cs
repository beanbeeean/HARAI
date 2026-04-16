using UnityEngine;

public class CommonMonsterAnimator : MonoBehaviour
{
    private Animator animator;
    private CommonMonster controller;

    [Header("Animator Parameters")]
    [SerializeField] private string dirXParameterName = "DirX";
    [SerializeField] private string dirYParameterName = "DirY";
    [SerializeField] private string dieParameterName = "DieTrigger";
    private int dirXHash;
    private int dirYHash;
    private int dieHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponentInParent<CommonMonster>();

        dirXHash = Animator.StringToHash(dirXParameterName);
        dirYHash = Animator.StringToHash(dirYParameterName);
        dieHash = Animator.StringToHash(dieParameterName);
    }

    public void PlayDie()
    {
        if (animator != null)
        {
            animator.SetTrigger(dieHash);
        }
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
            float inputX = direction.x > 0.1f ? 1f : (direction.x < -0.1f ? -1f : 0f);
            float inputY = direction.y > 0.1f ? 1f : (direction.y < -0.1f ? -1f : 0f);

            animator.SetFloat(dirXHash, inputX);
            animator.SetFloat(dirYHash, inputY);

            //Debug.Log($"몬스터 방향 - X: {inputX}, Y: {inputY}");
        }
    }

}
