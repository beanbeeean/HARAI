using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPUIController : MonoBehaviour
{
    [SerializeField] PlayerHPManager playerHPManager;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;

    [SerializeField] private float changeTimer = 3.0f;
    
    [SerializeField] private Image heartImg;
    [SerializeField] private float blinkerSpeed = 2.0f;

    [SerializeField] private Sprite FullHeart;
    [SerializeField] private Sprite BrokenHeart1;
    [SerializeField] private Sprite BrokenHeart2;

    private float prevRatio;

    void Start()
    {
        prevRatio = (float)playerHPManager.CurrentHealth/(float)playerHPManager.MaxHealth;
    }

    private void OnEnable()
    {
        playerHPManager.OnHealthChanged += UpdateHPUI;
        playerHPManager.OnDamaged += BlinkerHPUI;

    }

    private void OnDisable()
    {
        playerHPManager.OnHealthChanged -= UpdateHPUI;
        playerHPManager.OnDamaged -= BlinkerHPUI;
    }

    void BlinkerHPUI()
    {
        StartCoroutine(BlinkerHpUIRoutine());
    }

    IEnumerator BlinkerHpUIRoutine()
    {
        float timer = 0f;
        while (timer < changeTimer)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.PingPong(timer * blinkerSpeed, 1f);
            heartImg.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        heartImg.color = new Color(1f, 1f, 1f, 1f);;
    }

    void UpdateHPUI(int currentHealth, int maxHealth)
    {
        hpText.text = $"HP: {currentHealth}/{maxHealth}";
        float ratio = (float)currentHealth / (float)maxHealth;

        if(currentHealth >= 70)
        {
            heartImg.sprite = FullHeart;
        }else if(currentHealth >= 40)
        {
            heartImg.sprite = BrokenHeart1;
        }
        else
        {
            heartImg.sprite = BrokenHeart2;
        }

        if (currentHealth >= maxHealth)
        {
            hpSlider.value = ratio;
            Debug.Log("ratio : " + ratio);
        }
        else
        {
            StartCoroutine(ChangeHpRoutine(currentHealth, maxHealth));
        }
        

    }

    IEnumerator ChangeHpRoutine(int currentHealth, int maxHealth)
    {
        float timer = 0f;
        float ratio = (float)currentHealth / (float)maxHealth;

        while (timer < changeTimer)
        {
            timer += Time.deltaTime;    
            hpSlider.value = Mathf.Lerp(prevRatio, ratio, timer);
            yield return null;
        }

        prevRatio = ratio;
    }

}
