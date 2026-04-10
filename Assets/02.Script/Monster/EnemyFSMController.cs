using UnityEngine;
using System.Collections;

public abstract class EnemyFSMController : MonoBehaviour
{
    [Header("Common References")]
    protected Rigidbody2D rb;
    public Transform target;
    protected PlayerHPManager playerHP;

    [Header("State Settings")]
    public EnemyState currentState = EnemyState.Idle;
    public float moveSpeed = 2.0f;
    public float chaseDistance = 7f;
    public float attackDistance = 1.2f; 
    public float loseRange = 10f;
    public LayerMask obstacleMask;

    [Header("Light Settings")]
    public float maxExposure = 3.0f;
    [SerializeField] protected float currentExposure = 0f;
    [SerializeField] protected bool isExposed = false;
    protected readonly int lightLayerMask = 1 << 13;

    protected Vector2 currentVelocity;
    public Vector2 CurrentVelocity => currentVelocity;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        TryFindTarget();
    }

    private void TryFindTarget()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
            playerHP = playerObject.GetComponent<PlayerHPManager>();
        }
    }

    protected virtual void Update()
    {
        if (currentState == EnemyState.Dead) return;

        //HandleLightGauge();
        EvaluateStateTransition();
    }

    protected virtual void FixedUpdate()
    {
        // 1. 사망 또는 스턴 상태 체크
        if (currentState == EnemyState.Dead || currentState == EnemyState.Stun)
        {
            StopMovement();
            return;
        }

        // 2. 빛 감지 및 게이지 처리 (HandleLightGauge의 역할을 여기서 수행)
        if (isExposed)
        {
            // 빛을 맞고 있는 중
            currentExposure += Time.fixedDeltaTime;
            if (currentExposure >= maxExposure) OnLightGaugeFull();

            ApplyLightEffect(); // 여기서 속도를 0으로 만듦

            // 중요: 물리 프레임이 끝날 때 false로 초기화 (OnTriggerStay가 다시 켜줄 것임)
            isExposed = false;

            // 일반 몬스터는 빛을 맞으면 이후 로직(추격 등)을 실행하지 않음
            if (this is CommonMonster) return;
        }
        else
        {
            // 빛을 안 맞으면 게이지 즉시 초기화
            currentExposure = 0f;
        }

        // 3. 공격 중이 아닐 때만 이동/행동 로직 실행
        // IsAttacking은 아까 만든 코루틴의 bool 변수나 프로퍼티
        if (currentState != EnemyState.Attack)
        {
            HandleStateBehavior();
        }
    }

    protected void StopMovement()
    {
        currentVelocity = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
    }

    protected virtual void EvaluateStateTransition()
    {
        if (target == null) return;
        float distance = Vector2.Distance(transform.position, target.position);

        switch (currentState)
        {
            case EnemyState.Idle:
                if (CanSeePlayer()) currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                if (distance <= attackDistance) currentState = EnemyState.Attack;
                else if (distance > loseRange) currentState = EnemyState.Idle;
                break;

            case EnemyState.Attack:
                if (distance > attackDistance) currentState = EnemyState.Chase;
                break;
        }
    }

    protected virtual void HandleStateBehavior()
    {
        switch (currentState)
        {
            case EnemyState.Chase:
                MoveTowardsTarget(1f);
                break;
            case EnemyState.Attack:
                currentVelocity = Vector2.zero;
                rb.linearVelocity = Vector2.zero;
                TryAttack();
                break;
            case EnemyState.Idle:
                currentVelocity = Vector2.zero;
                break;
        }
    }

    protected void MoveTowardsTarget(float speedMultiplier)
    {
        if (target == null) return;
        Vector2 direction = (target.position - transform.position).normalized;
        currentVelocity = direction;
        rb.MovePosition(rb.position + direction * (moveSpeed * speedMultiplier) * Time.fixedDeltaTime);
    }

    public bool CanSeePlayer()
    {
        if (target == null) return false;
        Vector2 dir = (target.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, chaseDistance, obstacleMask);
        return hit.collider == null || hit.collider.CompareTag("Player");
    }

    //private void HandleLightGauge()
    //{
    //    if (isExposed)
    //    {
    //        currentExposure += Time.deltaTime;
    //        if (currentExposure >= maxExposure) OnLightGaugeFull();
    //        isExposed = false;
    //    }
    //    else
    //    {
    //        currentExposure = 0f;
    //    }
    //}

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & lightLayerMask) != 0) isExposed = true;
    }

    protected abstract void TryAttack();
    protected abstract void OnLightGaugeFull();
    protected abstract void ApplyLightEffect();
}