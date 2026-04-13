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
    public int attackDamage = 10;

    [Header("Common Monster Settings")]

    public float attackCooldown = 0.5f;
    private bool isAttackCoolingDown = false;

    [SerializeField] private float portalDetectionRadius = 1.2f;
    [SerializeField] private LayerMask portalLayer;

    private CommonMonsterAnimator monsterAnimator;
    protected override void Awake()
    {
        base.Awake();
        spawnPoint = transform.position;
        monsterAnimator = GetComponentInChildren<CommonMonsterAnimator>();
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!isPortalCooldown && (currentState == EnemyState.Chase || currentState == EnemyState.Patrol))
        {
            CheckForPortals();
        }
    }
    private void CheckForPortals()
    {
        LayerMask debugMask = 1 << LayerMask.NameToLayer("Portal");
        Collider2D hit = Physics2D.OverlapCircle(transform.position, portalDetectionRadius, debugMask);
        if (hit != null && hit.TryGetComponent(out TeleportPortal portal))
        {
            Debug.Log($"포탈 감지 성공: {hit.name}");
            if (currentState == EnemyState.Chase)
            {
                float distToLastSeen = Vector2.Distance(lastKnownPlayerPosition, portal.transform.position);
                if (!CanSeePlayer() && distToLastSeen < 1.0f)
                {
                    Debug.Log("!CanSeePlayer() && distToLastSeen < 1.0f");
                    Debug.Log($"CanSeePlayer : {CanSeePlayer()}");
                    ExecutePortalTeleport(portal);
                }
            }
            else if (currentState == EnemyState.Patrol)
            {
                Debug.Log("currentState == EnemyState.Patrol");
                if (Random.value < 0.5f && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {

                    Debug.Log("currentState == EnemyState.Patrol222222");
                    ExecutePortalTeleport(portal);
                }
            }
        }
    }

    private void ExecutePortalTeleport(TeleportPortal portal)
    {
        StartCoroutine(PortalCooldownRoutine(2.0f));
        portal.TeleportEnemy(this.gameObject);

        if (currentState == EnemyState.Chase) agent.SetDestination(target.position);
    }
    protected override void HandlePatrol()
    {
        if (isWaiting) return;
        if (!agent.pathPending && (agent.remainingDistance <= agent.stoppingDistance || !agent.hasPath))
        {
            StartCoroutine(PatrolRoutine());
        }
    }


    private IEnumerator PatrolRoutine()
    {
        isWaiting = true;
        yield return new WaitForSeconds(patrolWaitTime);
        Vector2 nextPoint = GetRandomPoint(spawnPoint, patrolRadius);
        agent.speed = moveSpeed * 0.5f;
        agent.SetDestination(nextPoint);
        isWaiting = false;
    }
    private Vector2 GetRandomPoint(Vector2 center, float radius)
    {
        for (int i = 0; i < 15; i++)
        {
            Vector2 randomPoint = center + Random.insideUnitCircle * radius;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomPoint, out navHit, 1.0f, NavMesh.AllAreas))
            {
                Vector2 targetPos = navHit.position;
                Vector2 originPos = transform.position;
                float distance = Vector2.Distance(originPos, targetPos);
                Vector2 direction = (targetPos - originPos).normalized;
                RaycastHit2D hit = Physics2D.Raycast(originPos, direction, distance, obstacleMask);
                if (hit.collider == null)
                {
                    return targetPos;
                }
            }
        }
        return (Vector2)transform.position;
    }

    protected override void TryAttack()
    {
        Debug.Log("TryAttack 호출!");
        if (!isAttackCoolingDown)
        {
            Debug.Log("!isAttackCoolingDown  - TryAttack 호출!");
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
                playerHP.TakeDamage(attackDamage, this.transform.position);
                Debug.Log($" 플레이어  현재 체력: {playerHP.CurrentHealth}");
            }
        }
        yield return new WaitForSeconds(attackCooldown);
        isAttackCoolingDown = false;
    }

    protected override void OnLightGaugeFull()
    {
        StartCoroutine(DieRoutine());
    }
    private IEnumerator DieRoutine()
    {
        StopMovement();
        currentState = EnemyState.Idle;
        if (monsterAnimator != null)
        {
            attackDamage = 0;
            monsterAnimator.PlayDie();
        }

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    protected override void ApplyLightEffect()
    {
        StopMovement();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, portalDetectionRadius);
    }
}