using System;
using System.Collections;
using UnityEngine;

public class PlayerHPManager : MonoBehaviour
{
    [Header("Health Settings (체력 10 기준)")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int currentHealth;
    [SerializeField] private float invincibleTime = 2f;
    public bool setFullHealthOnStart = true;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public float HealthPercentage => (float)currentHealth / maxHealth;
    public bool IsDead => currentHealth <= 0;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    public bool isHit = false;

    [SerializeField] private SpriteRenderer sprite;

    private void Awake()
    {
        if (setFullHealthOnStart) FullHealth();
    }


    public int FullHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        return currentHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead || amount <= 0 || isHit) return;

        currentHealth = Mathf.Max(0, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        StartCoroutine(InvincibilityRoutine());

        if (IsDead) Die();
    }

    private IEnumerator InvincibilityRoutine()
    {
        isHit = true;

        if (sprite != null)
        {
            Color originalColor = sprite.color;
            Color fadedColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.4f);

            float elapsed = 0f;
            float blinkInterval = 0.15f; 

            while (elapsed < invincibleTime)
            {
                sprite.color = (sprite.color == originalColor) ? fadedColor : originalColor;

                yield return new WaitForSeconds(blinkInterval);
                elapsed += blinkInterval;
            }

            sprite.color = originalColor;
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


}