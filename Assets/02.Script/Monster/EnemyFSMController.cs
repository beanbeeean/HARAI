using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public abstract class EnemyFSMController : MonoBehaviour
{
    [Header("Common References")]
    protected Rigidbody2D rb;
    protected NavMeshAgent agent;
    public Transform target;
    protected PlayerHPManager playerHP;

    [Header("State Settings")]
    public EnemyState currentState = EnemyState.Patrol;
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

    [Header("Portal Settings")]
    protected Vector2 lastKnownPlayerPosition; // 플레이어를 마지막으로 본 위치
    protected bool isPortalCooldown = false;    // 핑퐁 방지 쿨타임

    protected Vector2 currentVelocity;
    public Vector2 CurrentVelocity => currentVelocity;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        // 2D 설정을 코드로 강제
        agent.updateRotation = false;
        agent.updateUpAxis = false;

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

        // 플레이어를 보고 있다면 마지막 위치를 계속 갱신
        if (CanSeePlayer() && target != null)
        {
            lastKnownPlayerPosition = target.position;
        }

        EvaluateStateTransition();

        if (agent.hasPath)
            currentVelocity = agent.velocity.normalized;
        else
            currentVelocity = Vector2.zero;
    }

    // 몬스터가 포탈을 이용한 후 호출할 코루틴
    protected IEnumerator PortalCooldownRoutine(float duration)
    {
        isPortalCooldown = true;
        yield return new WaitForSeconds(duration);
        isPortalCooldown = false;
    }

    protected virtual void FixedUpdate()
    {
        if (currentState == EnemyState.Dead || currentState == EnemyState.Stun)
        {
            StopMovement();
            return;
        }

        if (isExposed)
        {
            currentExposure += Time.fixedDeltaTime;
            if (currentExposure >= maxExposure) OnLightGaugeFull();
            ApplyLightEffect();
            isExposed = false;

            if (this is CommonMonster) return;
        }
        else
        {
            currentExposure = 0f;
        }

        if (currentState != EnemyState.Attack)
        {
            HandleStateBehavior();
        }
    }

    protected void StopMovement()
    {
        // 정지 시에도 마찬가지로 체크해주는 것이 안전합니다.
        if (agent != null && agent.gameObject.activeInHierarchy && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        rb.linearVelocity = Vector2.zero;
        currentVelocity = Vector2.zero;
    }

    protected void ResumeMovement()
    {
        // 에이전트가 활성화되어 있고, NavMesh 위에 올라와 있는지(isOnNavMesh) 체크
        if (agent != null && agent.gameObject.activeInHierarchy && agent.isOnNavMesh)
        {
            agent.isStopped = false;
        }
    }

    protected virtual void EvaluateStateTransition()
    {
        if (target == null) return;
        float distance = Vector2.Distance(transform.position, target.position);

        switch (currentState)
        {
            case EnemyState.Idle:
            case EnemyState.Patrol:
                if (distance <= chaseDistance && CanSeePlayer())
                    currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                if (distance <= attackDistance)
                    currentState = EnemyState.Attack;
                else if (distance > loseRange)
                    currentState = EnemyState.Patrol; // 잃어버리면 다시 순찰로
                break;

            case EnemyState.Attack:
                if (distance > attackDistance)
                    currentState = EnemyState.Chase;
                break;
        }
    }

    protected virtual void HandleStateBehavior()
    {
        // 에이전트가 아직 NavMesh에 배치되지 않았다면 로직을 실행하지 않음
        if (agent == null || !agent.isOnNavMesh) return;

        ResumeMovement();

        switch (currentState)
        {
            case EnemyState.Chase:
                agent.speed = moveSpeed;
                agent.SetDestination(target.position);
                break;

            case EnemyState.Patrol:
                HandlePatrol(); // 자식 클래스에서 구현하거나 공통 로직으로 구현
                break;

            case EnemyState.Attack:
                StopMovement();
                TryAttack();
                break;

            case EnemyState.Idle:
                StopMovement();
                break;
        }
    }

    public bool CanSeePlayer()
    {
        if (target == null) return false;
        Vector2 dir = (target.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, chaseDistance, obstacleMask);
        return hit.collider == null || hit.collider.CompareTag("Player");
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & lightLayerMask) != 0) isExposed = true;
    }

    protected abstract void HandlePatrol(); // 순찰 방식은 몬스터마다 다를 수 있어 추상화
    protected abstract void TryAttack();
    protected abstract void OnLightGaugeFull();
    protected abstract void ApplyLightEffect();
}