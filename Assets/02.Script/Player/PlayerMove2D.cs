

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
    [SerializeField] private float jumpHeight = 1.5f; // 포물선 높이
    [SerializeField] private Transform spriteTransform;

    [SerializeField]
    Direction currentDirection;
    public Direction CurrentDirection => currentDirection;

    Vector2 inputDirection;
    public float moveSpeed = 5.0f;


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
    }

    //private void FixedUpdate() 
    //{
    //    //Vector2 newPosition = rb.position + inputDirection.normalized * moveSpeed * Time.fixedDeltaTime;
    //    //rb.MovePosition(newPosition);

    //    Vector2 moveStep = inputDirection.normalized * moveSpeed;

    //    // 넉백 중일 때는 입력 방향 대신 넉백 속도를 사용하거나, 두 값을 합칩니다.
    //    // 여기서는 넉백의 손맛을 위해 입력을 잠시 무시하고 넉백 속도만 적용해볼게요.
    //    Vector2 finalVelocity = isKnockback ? knockbackVelocity : moveStep;

    //    Vector2 newPosition = rb.position + finalVelocity * Time.fixedDeltaTime;
    //    rb.MovePosition(newPosition);
    //}

    //public void ApplyKnockback(Vector2 attackerPos)
    //{
    //    if (isKnockback) return;
    //    StartCoroutine(KnockbackRoutine(attackerPos));
    //}

    private void FixedUpdate()
    {
        if (isKnockback) return; // 넉백 중엔 입력 무시

        // 입력 방향으로 속도를 직접 꽂아줍니다.
        rb.linearVelocity = inputDirection.normalized * moveSpeed;
    }

    public void ApplyKnockback(Vector2 attackerPos)
    {
        if (isKnockback) return;
        StartCoroutine(KnockbackArcRoutine(attackerPos));
    }

    private IEnumerator KnockbackArcRoutine(Vector2 attackerPos)
    {
        isKnockback = true;

        // 1. 초기 넉백 속도 부여 (Linear Damping이 이걸 서서히 줄여줄 겁니다)
        Vector2 dir = ((Vector2)transform.position - attackerPos).normalized;
        rb.linearVelocity = dir * knockbackForce;

        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / knockbackDuration;

            // 2. 포물선 계산 (스프라이트만 위로 띄우기)
            // y = -4h(x^2 - x) 공식
            float height = 4 * jumpHeight * (percent - percent * percent);

            if (spriteTransform != null)
            {
                spriteTransform.localPosition = new Vector3(0, height, 0);
            }

            yield return null; // 매 프레임 실행
        }

        // 3. 착지 및 제어권 반환
        if (spriteTransform != null) spriteTransform.localPosition = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
        isKnockback = false;
    }

    //private IEnumerator KnockbackRoutine(Vector2 attackerPos)
    //{
    //    isKnockback = true;

    //    // 1. 방향 계산 (공격자 -> 나)
    //    Vector2 dir = ((Vector2)transform.position - attackerPos).normalized;

    //    // 2. 넉백 속도 설정
    //    knockbackVelocity = dir * knockbackForce;

    //    // 3. 넉백 지속 시간
    //    yield return new WaitForSeconds(knockbackDuration);

    //    // 4. 초기화
    //    knockbackVelocity = Vector2.zero;
    //    isKnockback = false;
    //}

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
}
