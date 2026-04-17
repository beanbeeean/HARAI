using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerHPManager playerHPManager;
    [SerializeField] private GameObject circleLight;

    [SerializeField] private List<GameObject> closeObjects;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] Animator playerAnimator;
    [SerializeField] Image fadeoutImg;
    [SerializeField] CanvasGroup fadeinText;

    void OnEnable()
    {
        playerHPManager.OnDied += PlayerDie;
    }

    void OnDisable()
    {
        playerHPManager.OnDied -= PlayerDie;
    }

    void SetInactiveObjects()
    {
        foreach (GameObject obj in closeObjects)
        {
            obj.SetActive(false);
        }

        CommonMonster[] commonMonsters = FindObjectsByType<CommonMonster>(FindObjectsSortMode.None);
        ItemObject[] items = FindObjectsByType<ItemObject>(FindObjectsSortMode.None);
        TeleportPortal[] portals = FindObjectsByType<TeleportPortal>(FindObjectsSortMode.None);
        foreach (CommonMonster monster in commonMonsters)
        {
            Destroy(monster.gameObject);
        }

        foreach (ItemObject item in items)
        {
            Destroy(item.gameObject);
        }

        foreach(TeleportPortal portal in portals)
        {
            portal.SetInactiveCanvas();
        }

    }

    void PlayerDie()
    {
        Debug.Log("PlayerDie()");
        // UI 다 끄는 함수.  키입력 전체 비활성화 함수 필요

        // Enemy, Item Spawner 비활성화
        // Common Monster 전체 비활성화 + Item 오브젝트전체 비활성화
        // Mainmonster 비활성화 
        SetInactiveObjects();
        playerInput.enabled = false;


        playerSprite.enabled = false;
        

        StartCoroutine(DeathScenario());
    }

    IEnumerator DeathScenario()
    {
        SoundManager.Instance.PlaySFX("PlayerDied");
        yield return new WaitForSeconds(3f);
        circleLight.SetActive(true);
        playerSprite.enabled = true;
        // 애니메이션 - 메인몬스터 모습으로 웃는 Sprite 8프레임 정도..
        //  + 사운드 포함
        playerAnimator.SetBool("IsDead", true);
        SoundManager.Instance.PlaySFX("EnemyLaugh_1");


        yield return new WaitForSeconds(2.5f);
        SoundManager.Instance.PlaySFX("EnemyLaugh_2");
        float timer = 0f;
        bool isSoundPlayed = false;

        while (timer < 5.0f)
        {
            timer += Time.deltaTime;
            if (timer >= 3.0f && !isSoundPlayed) 
            {
                SoundManager.Instance.PlaySFX("EnemyLaugh_2");
                isSoundPlayed = true;
            }
            float alpha = Mathf.Lerp(0f, 1f, timer / 5.0f);
            fadeoutImg.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        timer = 0f;
        

        yield return new WaitForSeconds(1f);
        
        while (timer < 3.0f)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / 3.0f);
            fadeinText.alpha = alpha;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        timer = 0f;
        while (timer < 3.0f)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / 3.0f);
            fadeinText.alpha = alpha;
            yield return null;
        }

        GameSceneManager.Instance.LoadSceneByName("Ending");
    }


}
