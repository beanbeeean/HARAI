using UnityEngine;
using System.Collections;

public class CommonMonster : EnemyFSMController
{
    [Header("Common Monster Settings")]
    public float attackCooldown = 2.0f;
    private bool isAttackCoolingDown = false;


    protected override void OnTriggerStay2D(Collider2D collision)
    {
        base.OnTriggerStay2D(collision);

        if (collision.CompareTag("Player") && currentState != EnemyState.Stun)
        {
            TryAttack();
        }
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

    protected override void ApplyLightEffect()
    {
        StopMovement();
    }

    protected override void OnLightGaugeFull()
    {
        Debug.Log("소멸");
        Destroy(gameObject);
    }
}