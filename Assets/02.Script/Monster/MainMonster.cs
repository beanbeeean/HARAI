using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MainMonster : EnemyFSMController
{
    [Header("Stalker - Timer Settings")]
    public float maxTeleportTime = 60f;
    public float maxEncounterTime = 120f;
    [SerializeField] private float teleportTimer = 0f;
    [SerializeField] private float encounterTimer = 0f;

    [Header("Stalker - Floor Info")]
    public int currentFloor;
    private int playerFloor => (target != null) ? target.GetComponent<PlayerMove2D>().currentFloor : -1;

    [Header("Stalker - Unique Movement")]
    public float slowedSpeed = 0.3f;
    [Header("Stalker - Status")]
    private Coroutine currentImmuneRoutine;
    private float stunTime = 5f;
    private float immuneTime = 30f;
    public bool isImmune = false;
    [SerializeField] GameObject auraObject;

    protected override void Awake()
    {
        base.Awake();
        canAttackWhileExposed = true;
        if (auraObject != null) auraObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
        if (currentState == EnemyState.Dead) return;

        HandleTeleportTimers();


    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (!isPortalCooldown && (currentState == EnemyState.Chase || currentState == EnemyState.Patrol))
        {
            CheckForPortals();
        }

        if (agent != null && agent.isOnNavMesh && agent.hasPath)
        {
            if (agent.velocity.sqrMagnitude < 0.1f && agent.remainingDistance > agent.stoppingDistance)
            {
                agent.SetDestination(agent.destination);
            }
        }

    }

    private void CheckForPortals()
    {
        LayerMask debugMask = 1 << LayerMask.NameToLayer("Portal");
        Collider2D hit = Physics2D.OverlapCircle(transform.position, portalDetectionRadius, debugMask);

        if (hit != null && hit.TryGetComponent(out TeleportPortal portal))
        {
            Debug.Log("[MainMonster] hit != null && hit.TryGetComponent(out TeleportPortal portal)");
            Debug.Log("현재 상태: " + currentState);
            if (currentState == EnemyState.Chase)
            {
                float distToLastSeen = Vector2.Distance(lastKnownPlayerPosition, portal.transform.position);
                if (!CanSeePlayer() && distToLastSeen < 1.0f)
                {
                    Debug.Log("!CanSeePlayer() && distToLastSeen < 1.0f");
                    ExecutePortalTeleport(portal);
                }
            }
            else if (currentState == EnemyState.Patrol)
            {
                if (Random.value < 0.5f && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    ExecutePortalTeleport(portal);
                }
            }
        }
    }


    private void ExecutePortalTeleport(TeleportPortal portal)
    {
        StartCoroutine(PortalCooldownRoutine(2.0f));

        portal.TeleportEnemy(this.gameObject);

        if (currentState == EnemyState.Chase && target != null)
        {
            agent.SetDestination(target.position);
        }

        Debug.Log($"[MainMonster]포탈 이동: {portal.name}");
    }

    private void HandleTeleportTimers()
    {
        if (playerFloor != currentFloor && playerFloor != -1)
            teleportTimer += Time.deltaTime;

        if (currentState != EnemyState.Chase)
            encounterTimer += Time.deltaTime;
        else
            encounterTimer = 0f;

        if (teleportTimer >= maxTeleportTime || encounterTimer >= maxEncounterTime)
            TeleportToPlayerFloor();
    }

    private void TeleportToPlayerFloor()
    {
        teleportTimer = 0f;
        encounterTimer = 0f;

        Vector3 spawnPos = GetRandomPositionOnPlayerFloor();
        agent.Warp(spawnPos);

        agent.isStopped = false;
        currentState = EnemyState.Patrol;
        Debug.Log("메인 몬스터가 플레이어 층으로 재배치");
    }

    public void UpdateCurrentFloor(int floor) => currentFloor = floor;


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
        agent.speed = moveSpeed * 0.7f;
        agent.SetDestination(nextPoint);

        isWaiting = false;
    }

    protected override void TryAttack()
    {
        if (!isAttackCoolingDown)
            StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttackCoolingDown = true;
        if (playerHP != null)
        {
            playerHP.TakeDamage(attackDamage, transform.position);
        } 


        if (isImmune)
        {
            isImmune = false;
            if (auraObject != null) auraObject.SetActive(false);

            if (currentImmuneRoutine != null)
            {
                StopCoroutine(currentImmuneRoutine);
                currentImmuneRoutine = null;
            }
            AlertManager.Instance.ShowAlert(AlertKey.NoImmune);
            
        }
        yield return new WaitForSeconds(attackCooldown);
        isAttackCoolingDown = false;
    }

    protected override void ApplyLightEffect()
    {
        if (isImmune) return;

        agent.speed = slowedSpeed;
    }

    protected override void HandleStateBehavior()
    {
        if (currentExposure <= 0)
        {
            agent.speed = (currentState == EnemyState.Chase) ? moveSpeed : moveSpeed * 0.7f;
        }
        base.HandleStateBehavior();
    }

    protected override void OnLightGaugeFull()
    {
        if (isImmune) return;

        if (currentImmuneRoutine != null) StopCoroutine(currentImmuneRoutine);
        currentImmuneRoutine = StartCoroutine(StunAndImmuneRoutine());
    }

    private IEnumerator StunAndImmuneRoutine()
    {
        currentState = EnemyState.Stun;
        StopMovement();
        currentExposure = 0;
        AlertManager.Instance.ShowAlert(AlertKey.Stun);

        auraObject.SetActive(false);
        yield return new WaitForSeconds(stunTime);

        isImmune = true;
        auraObject.SetActive(true);
        AlertManager.Instance.ShowAlert(AlertKey.Immune);

        ResumeMovement();

        yield return new WaitForSeconds(immuneTime);
        if (isImmune)
        {
            AlertManager.Instance.ShowAlert(AlertKey.NoImmune);
            isImmune = false;
            auraObject.SetActive(false);
        }

        currentImmuneRoutine = null;
        
    }


    private Vector2 GetRandomPoint(Vector2 center, float radius)
    {
        for (int i = 0; i < 15; i++)
        {
            Vector2 randomPoint = center + Random.insideUnitCircle * radius;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomPoint, out navHit, 1.0f, NavMesh.AllAreas))
            {
                return navHit.position;
            }
        }
        return (Vector2)transform.position;
    }

    private Vector3 GetRandomPositionOnPlayerFloor()
    {
        Vector3 finalPos = Vector3.zero;
        bool targetFound = false;
        int maxAttempts = 50;
        int currentAttempt = 0;

        while (!targetFound && currentAttempt < maxAttempts)
        {
            currentAttempt++;

            float randomDist = Random.Range(10f, 15f);
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector3 candidatePos = target.position + (Vector3)(randomDir * randomDist);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidatePos, out hit, 5.0f, NavMesh.AllAreas))
            {
                finalPos = hit.position;
                targetFound = true;
            }
        }

        if (!targetFound)
        {
            finalPos = target.position - (target.right * 15f);
            if (NavMesh.SamplePosition(finalPos, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                finalPos = hit.position;
            }
        }

        return finalPos;
    }

    protected override void UpdateExposureUI()
    {
        if (exposureSlider == null || canvasObj == null) return;

        if (currentExposure > 0 && !isImmune)
        {
            canvasObj.SetActive(true);
            exposureSlider.value = currentExposure / maxExposure;
            canvasObj.transform.rotation = Quaternion.identity;
        }
        else
        {
            canvasObj.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, portalDetectionRadius);
    }
}