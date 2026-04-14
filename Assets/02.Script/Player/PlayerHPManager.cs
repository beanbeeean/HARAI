using System;
using System.Collections;
using UnityEngine;

public class PlayerHPManager : MonoBehaviour
{
    [Header("Health Settings (체력 10 기준)")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int baseMaxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private float invincibleTime = 2f;
    [SerializeField] private HealthPopup healthPopup;
    
    public bool setFullHealthOnStart = true;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public float HealthPercentage => (float)currentHealth / maxHealth;
    public bool IsDead => currentHealth <= 0;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    public bool isHit = false;

    [SerializeField] private GameObject visualContainer; 
    private SpriteRenderer[] childSprites;

    private void Awake()
    {
        if (setFullHealthOnStart) FullHealth();
    }

    private void Start()
    {
        if (visualContainer != null)
        {
            childSprites = visualContainer.GetComponentsInChildren<SpriteRenderer>();
        }

    }

    public int FullHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        return currentHealth;
    }




    public void TakeDamage(int amount, Vector2 attackerPosition)
    {
        if (IsDead || amount <= 0 || isHit) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (healthPopup != null)
        {
            healthPopup.Show(amount, true);
        }

        // 넉백 및 무적 로직...
        if (TryGetComponent(out PlayerMove2D playerMove))
        {
            playerMove.ApplyKnockback(attackerPosition);
        }
        StartCoroutine(InvincibilityRoutine());
        if (IsDead) Die();
    }

    private IEnumerator InvincibilityRoutine()
    {
        isHit = true;

        if (childSprites != null && childSprites.Length > 0)
        {
            float elapsed = 0f;
            float blinkInterval = 0.15f;

            while (elapsed < invincibleTime)
            {
                foreach (var s in childSprites)
                {
                    if (s != null) s.enabled = !s.enabled;
                }

                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }

            foreach (var s in childSprites)
            {
                if (s != null) s.enabled = true;
            }
        }
        else
        {
            yield return new WaitForSeconds(invincibleTime);
        }

        isHit = false;
    }


    public void Heal(int amount)
    {
        if (IsDead || amount <= 0) return;


        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (healthPopup != null)
        {
            healthPopup.Show(amount, false);
        }
    }

    private void Die()
    {
        StopAllCoroutines();
        OnDied?.Invoke();

        //// 플레이어 캐릭터라면 결과 화면(Clear/Result)으로 이동
        //if (gameObject.CompareTag("Player") && GameSceneManager.Instance != null)
        //{
        //    GameSceneManager.Instance.LoadSceneByName("Result");
        //}
    }


    public void UpdateMaxHp(float totalPenalty)
    {
        maxHealth = Mathf.Max(baseMaxHealth - (int)totalPenalty, 70);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

}