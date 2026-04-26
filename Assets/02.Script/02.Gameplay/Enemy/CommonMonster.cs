using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class CommonMonster : EnemyFSMController

{
    [Header("Common Monster - Local Info")]
    private int baseAttackDamage;
    private Vector2 spawnPoint;
    [SerializeField] private bool isDead = false;

    [Header("Effect Settings")]
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private float effectYOffset = 1.0f;

    protected override void Awake()
    {
        base.Awake();
        baseAttackDamage = attackDamage;
        spawnPoint = transform.position;
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
            // Debug.Log($"포탈 감지 성공: {hit.name}");
            if (currentState == EnemyState.Chase)
            {
                float distToLastSeen = Vector2.Distance(lastKnownPlayerPosition, portal.transform.position);
                if (!CanSeePlayer() && distToLastSeen < 2.0f)
                {
                    ExecutePortalTeleport(portal);
                }
            }
            else if (currentState == EnemyState.Patrol)
            {
                // Debug.Log("currentState == EnemyState.Patrol");
                if (Random.value < 0.5f && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {

                    // Debug.Log("currentState == EnemyState.Patrol222222");
                    ExecutePortalTeleport(portal);
                }
            }
        }
    }

    public void ResetMonster(Vector3 spawnPos)
    {
        StopAllCoroutines();
        isDead = false;
        isAttackCoolingDown = false;
        isExposed = false;

        agent.enabled = true;
        agent.Warp(spawnPos);
        
        agent.isStopped = false;
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (SpriteRenderer sr in srs)
        {
            sr.enabled = true; 
            sr.gameObject.SetActive(true); 
        }

        currentState = EnemyState.Patrol;

        
        HandlePatrol();

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
        Vector2 nextPoint = GetRandomPoint(transform.position, patrolRadius);
        // Debug.Log("patrol Radius : " + patrolRadius);
        agent.speed = moveSpeed * 0.5f;
        agent.SetDestination(nextPoint);
        // Debug.Log("enemy pos : " + transform.position);
        // Debug.Log("next Point : " + nextPoint);
        isWaiting = false;
    }
    private Vector2 GetRandomPoint(Vector2 center, float radius)
    {
        for (int i = 0; i < 15; i++)
        {
            Vector2 randomPoint = center + Random.insideUnitCircle * radius;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomPoint, out navHit, 2.0f, NavMesh.AllAreas))
            {
                // Debug.Log("GetRandomPoint Pass - 1");
                Vector2 targetPos = navHit.position;
                return targetPos;
                
            }
        }
        // Debug.Log("GetRandomPoint Failed");
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
        if (isDead) return;
        isDead = true;
        isAttackCoolingDown = true;
        StopMovement();
        currentState = EnemyState.Idle;


        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        spriteRenderer.gameObject.SetActive(false);

        agent.isStopped = true;


        if (deathEffectPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(0, effectYOffset, 0);

            GameObject effect = Instantiate(deathEffectPrefab, spawnPosition, Quaternion.identity);

            Destroy(effect, 0.8f);
        }
        else
        {
            Debug.LogWarning("이펙트 프리팹 할당 X");
        }
        StartCoroutine(PlayDeathSoundRoutine());
    }

    private IEnumerator PlayDeathSoundRoutine()
    {
        PlayDeathSound();
        yield return new WaitForSeconds(0.9f);
        EnemyPool.Instance.Release(this.gameObject);
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

    public void UpdateAttackDamage(float totalPenalty)
    {
        attackDamage = Mathf.Max(baseAttackDamage + (int)totalPenalty, baseAttackDamage);

    }

    public void UpdateChaseRange(float totalPenalty)
    {
        chaseDistance = Mathf.Max(baseChaseDistance + totalPenalty, baseChaseDistance);
        loseRange = Mathf.Max(baseLoseRange + totalPenalty, baseLoseRange);
    }

}