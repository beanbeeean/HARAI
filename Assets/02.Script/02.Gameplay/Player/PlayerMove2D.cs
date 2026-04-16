

using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove2D : MonoBehaviour
{
    [Header("Ref")]
    private Rigidbody2D rb;
    private PlayerInputReader playerInputReader;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.5f;
    private bool isKnockback = false;
    private Vector2 knockbackVelocity;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private Transform spriteTransform;

    [SerializeField]
    Direction currentDirection;
    public Direction CurrentDirection => currentDirection;

    Vector2 inputDirection;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float baseMoveSpeed = 5.0f;
    private float tempSpeedMultiplier = 1.0f;
    public float MoveSpeed => moveSpeed;

    public int currentFloor;

    private float footstepTimer;
    [SerializeField] private float stepInterval = 0.3f;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (playerInputReader == null) playerInputReader = GetComponent<PlayerInputReader>();
    }

    private void Start()
    {
        currentDirection = Direction.Down;
    }

    private void Update()
    {
        
        inputDirection = playerInputReader != null ? playerInputReader.MoveVector : Vector2.zero;
        UpdateDirection();

        if(inputDirection.sqrMagnitude > 0.01f && !isKnockback)
        {
            footstepTimer += Time.deltaTime;
            if(footstepTimer >= stepInterval)
            {
                SoundManager.Instance.PlayWalkSound();
                footstepTimer = 0;
            }
        }
        else
        {
            SoundManager.Instance.StopWalkSound();
            footstepTimer = stepInterval;
        }
    }

    private void FixedUpdate()
    {
        if (isKnockback) return;

        rb.linearVelocity = inputDirection.normalized * (moveSpeed * tempSpeedMultiplier);
    }

    public void ApplyKnockback(Vector2 attackerPos)
    {
        if (isKnockback) return;
        StartCoroutine(KnockbackArcRoutine(attackerPos));
    }

    private IEnumerator KnockbackArcRoutine(Vector2 attackerPos)
    {
        isKnockback = true;

        Vector2 dir = ((Vector2)transform.position - attackerPos).normalized;
        rb.linearVelocity = dir * knockbackForce;

        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / knockbackDuration;

            float height = 4 * jumpHeight * (percent - percent * percent);

            if (spriteTransform != null)
            {
                spriteTransform.localPosition = new Vector3(0, height, 0);
            }

            yield return null;
        }

        if (spriteTransform != null) spriteTransform.localPosition = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }


    private void UpdateDirection()
    {
        const float deadzoneValue = 0.1f;

        if (inputDirection.sqrMagnitude < 0.01f) return;

        float x = inputDirection.x;
        float y = inputDirection.y;

        if (x > deadzoneValue) 
        {
            if (y > deadzoneValue) currentDirection = Direction.RightUp;
            else if (y < -1 * deadzoneValue) currentDirection = Direction.RightDown;
            else currentDirection = Direction.Right;
        }
        else if (x < -1 * deadzoneValue) 
        {
          
            if (y > deadzoneValue) currentDirection = Direction.LeftUp;
           
            else if (y < -1 * deadzoneValue) currentDirection = Direction.LeftDown;
            else currentDirection = Direction.Left;
        }
        else 
        {
            if (y > deadzoneValue) currentDirection = Direction.Up;
            else if (y < -1 * deadzoneValue) currentDirection = Direction.Down;
        }

    }

    public void SetTempSpeedMultiplier(float multiplier)
    {
        tempSpeedMultiplier = multiplier;
    }

    public void UpdateMoveSpeed(float totalPenalty)
    {
        moveSpeed = Mathf.Max(baseMoveSpeed - totalPenalty, 3.5f);
    }
}
