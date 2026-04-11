using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CommonMonster : EnemyFSMController
{
    [Header("Patrol Settings")]
    public float patrolRadius = 5f;
    public float patrolWaitTime = 2f;
    private Vector2 spawnPoint;
    private bool isWaiting = false;

    [Header("Common Monster Settings")]
    public float attackCooldown = 2.0f;
    private bool isAttackCoolingDown = false;

    [SerializeField] private float portalDetectionRadius = 1.2f;
    [SerializeField] private LayerMask portalLayer;
    protected override void Awake()
    {
        base.Awake();
        spawnPoint = transform.position; // 처음 생성된 위치를 순찰 중심점으로
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate(); // 기존 노출 게이지 및 이동 로직 실행

        // 쿨타임이 아니고, 추격이나 순찰 상태일 때만 포탈 체크
        if (!isPortalCooldown && (currentState == EnemyState.Chase || currentState == EnemyState.Patrol))
        {
            CheckForPortals();
        }
    }


    private void CheckForPortals()
    {
        // 1. 인스펙터에서 설정한 portalLayer(LayerMask)를 그대로 사용합니다.
        LayerMask debugMask = 1 << LayerMask.NameToLayer("Portal");

        Collider2D hit = Physics2D.OverlapCircle(transform.position, portalDetectionRadius, debugMask);

        if (hit != null && hit.TryGetComponent(out TeleportPortal portal))
        {
            Debug.Log($"포탈 감지 성공: {hit.name}");

            if (currentState == EnemyState.Chase)
            {
                // 플레이어가 안 보이고 마지막 목격 지점 근처일 때
                float distToLastSeen = Vector2.Distance(transform.position, lastKnownPlayerPosition);
                if (!CanSeePlayer() && distToLastSeen < 2.5f) // 테스트를 위해 거리를 살짝 늘림
                {
                    ExecutePortalTeleport(portal);
                }
            }
            else if (currentState == EnemyState.Patrol)
            {
                // 순찰 중 확률적 이동
                if (Random.value < 0.5f && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    ExecutePortalTeleport(portal);
                }
            }
        }
    }

    private void ExecutePortalTeleport(TeleportPortal portal)
    {
        StartCoroutine(PortalCooldownRoutine(2.0f)); // 2초 쿨타임
        portal.TeleportEnemy(this.gameObject);

        // 이동 후 AI가 즉시 길을 재계산하도록 타겟 재설정
        if (currentState == EnemyState.Chase)
            agent.SetDestination(target.position);
    }

    protected override void HandlePatrol()
    {
        // 이미 대기 중이거나 이동 중(경로가 있음)이면 리턴
        if (isWaiting) return;

        // 목적지에 거의 도착했는지 확인
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(PatrolRoutine());
        }
    }

    private IEnumerator PatrolRoutine()
    {
        isWaiting = true;

        // 지점에 도착하면 잠시 대기
        yield return new WaitForSeconds(patrolWaitTime);

        // 새로운 랜덤 지점 찾기
        Vector2 nextPoint = GetRandomPoint(spawnPoint, patrolRadius);
        agent.speed = moveSpeed * 0.5f; // 순찰은 조금 천천히
        agent.SetDestination(nextPoint);

        isWaiting = false;
    }

    private Vector2 GetRandomPoint(Vector2 center, float radius)
    {
        // 최대 15번 시도하여 유효한 지점을 찾습니다.
        for (int i = 0; i < 15; i++)
        {
            // 1. 반경 내 랜덤 좌표 생성
            Vector2 randomPoint = center + Random.insideUnitCircle * radius;

            // 2. 해당 지점이 NavMesh(바닥) 위인지 확인
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomPoint, out navHit, 1.0f, NavMesh.AllAreas))
            {
                Vector2 targetPos = navHit.position;
                Vector2 originPos = transform.position;

                // 3. [핵심] 현재 위치에서 목적지까지 벽이 있는지 레이캐스트로 검사
                float distance = Vector2.Distance(originPos, targetPos);
                Vector2 direction = (targetPos - originPos).normalized;

                // obstacleMask(Wall 레이어)에 걸리는게 있는지 확인
                RaycastHit2D hit = Physics2D.Raycast(originPos, direction, distance, obstacleMask);

                if (hit.collider == null)
                {
                    // 벽에 가로막히지 않는 깨끗한 시야라면 이 지점을 목적지로 확정!
                    return targetPos;
                }
            }
        }

        // 15번 시도해도 못 찾았다면, 무리하게 벽으로 가지 않고 현재 위치 근처나 제자리를 반환합니다.
        return (Vector2)transform.position;
    }

    protected override void TryAttack()
    {
        if (!isAttackCoolingDown)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttackCoolingDown = true;
        currentState = EnemyState.Attack;

        if (target != null)
        {
            if (playerHP != null)
            {
                playerHP.TakeDamage(1);
                Debug.Log($" 플레이어  현재 체력: {playerHP.CurrentHealth}");
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttackCoolingDown = false;
    }


    protected override void OnLightGaugeFull()
    {
        // 빛 게이지가 다 찼을 때 소멸 로직
        Destroy(gameObject);
    }

    protected override void ApplyLightEffect()
    {
        // 빛을 맞는 동안 멈춤
        StopMovement();
    }

    private void OnDrawGizmosSelected()
    {
        // 포탈 감지 범위를 노란색 원으로 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, portalDetectionRadius);
    }
}



