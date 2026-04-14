using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UI;

public abstract class EnemyFSMController : MonoBehaviour
{
    [Header("Common References")]
    protected Rigidbody2D rb;
    protected NavMeshAgent agent;
    public Transform target;
    protected PlayerHPManager playerHP;

    [Header("Base Stat Settings")]
    public EnemyState currentState = EnemyState.Patrol;
    public float moveSpeed = 2.0f;
    public int attackDamage = 10;
    public float attackCooldown = 1.0f;
    protected bool isAttackCoolingDown = false;

    [Header("Detection Settings")]
    public float chaseDistance = 5f;
    public float attackDistance = 1.0f;
    public float loseRange = 7f;
    public float baseChaseDistance = 5f;
    public float baseLoseRange = 7f;
    public LayerMask obstacleMask;
    public bool canAttackWhileExposed = false;

    [Header("Patrol Settings")]
    public float patrolRadius = 5f;
    public float patrolWaitTime = 2f;
    protected bool isWaiting = false;

    [Header("Light(UV) Settings")]
    public float maxExposure = 3.0f;
    [SerializeField] protected float currentExposure = 0f;
    [SerializeField] protected bool isExposed = false;
    protected readonly int lightLayerMask = 1 << 13;

    [Header("Light Timing Settings")]
    [SerializeField] private float exposureResetDelay = 0.1f;
    private float lastExposedTime;

    [Header("Portal Settings")]
    public float portalDetectionRadius = 1.2f;
    public LayerMask portalLayer;
    protected Vector2 lastKnownPlayerPosition;
    protected bool isPortalCooldown = false;

    [Header("UI Settings")]
    [SerializeField] protected Slider exposureSlider;
    [SerializeField] protected GameObject canvasObj;

    protected Vector2 currentVelocity;
    public Vector2 CurrentVelocity => currentVelocity;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

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

        if (CanSeePlayer() && target != null)
        {
            lastKnownPlayerPosition = target.position;
        }

        EvaluateStateTransition();

        if (agent.hasPath)
            currentVelocity = agent.velocity.normalized;
        else
            currentVelocity = Vector2.zero;

        UpdateExposureUI();
    }

    protected virtual void UpdateExposureUI()
    {
        if (exposureSlider == null || canvasObj == null) return;

        if (currentExposure > 0)
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

        if (agent != null && agent.isOnNavMesh && agent.hasPath)
        {
            if (agent.velocity.sqrMagnitude < 0.1f && agent.remainingDistance > agent.stoppingDistance)
            {
                agent.SetDestination(agent.destination);
            }
        }

        if (isExposed)
        {
            lastExposedTime = Time.fixedTime;
            isExposed = false;
        }

        float timeSinceLastExposed = Time.fixedTime - lastExposedTime;
        bool isStillExposed = timeSinceLastExposed < exposureResetDelay;

        if (isStillExposed)
        {
            currentExposure += Time.fixedDeltaTime;
            ApplyLightEffect();


            if (currentExposure >= maxExposure)
            {
                OnLightGaugeFull();
                return;
            }
            if (!canAttackWhileExposed)
            {
                return;
            }
        }
        else
        {
            if (currentExposure > 0)
            {
                Debug.Log($"[리셋됨] 마지막 노출로부터 {timeSinceLastExposed}초 경과");
                currentExposure = 0f;
            }
        }

        HandleStateBehavior();
    }

    protected void StopMovement()
    {
        if (agent != null && agent.gameObject.activeInHierarchy && agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        rb.linearVelocity = Vector2.zero;
        currentVelocity = Vector2.zero;
    }

    protected void ResumeMovement()
    {
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
                    currentState = EnemyState.Patrol;
                break;

            case EnemyState.Attack:
                if (distance > attackDistance)
                    currentState = EnemyState.Chase;
                break;
        }
    }

    protected virtual void HandleStateBehavior()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        ResumeMovement();

        switch (currentState)
        {
            case EnemyState.Chase:
                if (currentExposure <= 0)
                {
                    agent.speed = moveSpeed;
                }
                agent.SetDestination(target.position);
                break;
            case EnemyState.Patrol:
                HandlePatrol();
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
        if (((1 << collision.gameObject.layer) & lightLayerMask) != 0 && collision.GetComponentInParent<FlashlightManager>().isUVMode) isExposed = true;
    }

    protected abstract void HandlePatrol();
    protected abstract void TryAttack();
    protected abstract void OnLightGaugeFull();
    protected abstract void ApplyLightEffect();
}