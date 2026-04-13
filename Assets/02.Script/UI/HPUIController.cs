using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPUIController : MonoBehaviour
{
    [SerializeField] PlayerHPManager playerHPManager;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TextMeshProUGUI hpText;

    private void OnEnable()
    {
        playerHPManager.OnHealthChanged += UpdateHPUI; 
    }

    private void OnDisable()
    {
        playerHPManager.OnHealthChanged -= UpdateHPUI;
    }


    void UpdateHPUI(int currentHealth, int maxHealth)
    {
        float ratio = (float)currentHealth / (float)maxHealth;
        hpSlider.value = ratio;
        Debug.Log("ratio : " + ratio);
        hpText.text = $"HP: {currentHealth}/100";
    }

}
