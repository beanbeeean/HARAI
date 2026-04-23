using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Light2D globalLight;
    [SerializeField] private CinemachineCamera cineCamera;


    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerHPManager playerHPManager;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] Animator playerAnimator;
    [SerializeField] private GameObject playerCircleLight;
    [SerializeField] private GameObject mainMonster;
    [SerializeField] Animator mainMonsterAnimator;
    [SerializeField] private GameObject mainMonsterCircleLight;



    [SerializeField] private List<GameObject> closeObjects;

    [SerializeField] private Image fadeoutImg;

    [SerializeField] private CanvasGroup fadeinText;

    public Action StartPlayEvent;

    [SerializeField] private GameObject fadeOutObj;
    

    private bool isCleared = false;

    private void Awake() => Instance = this;


    void Start()
    {
        StartPlay();
    }
    
    void OnEnable()
    {
        PurificationManager.Instance.gameClearEvent += GameClear;
        playerHPManager.OnDied += GameOver;
    }

    void OnDisable()
    {
        PurificationManager.Instance.gameClearEvent -= GameClear;
        playerHPManager.OnDied -= GameOver;
    }

    void SetInactiveObjects()
    {
        if (!isCleared)
        {
            mainMonster.SetActive(false);
        }

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

        foreach (TeleportPortal portal in portals)
        {
            portal.SetInactiveCanvas();
        }

    }

    public void StartPlay()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartCoroutine(StartPlayRoutine());
    }
    
    IEnumerator StartPlayRoutine()
    {
        fadeOutObj.SetActive(true);
        foreach (GameObject obj in closeObjects)
        {
            obj.SetActive(false);
        }
        playerInput.enabled = false;
        yield return FadeInView();

        foreach (GameObject obj in closeObjects)
        {
            obj.SetActive(true);
        }
        fadeOutObj.SetActive(false);
        playerInput.enabled = true;
        yield return new WaitForSeconds(1f);

        AlertManager.Instance.ShowAlert(AlertKey.StartMsg_1);
        AlertManager.Instance.ShowAlert(AlertKey.StartMsg_2);
        AlertManager.Instance.ShowAlert(AlertKey.StartMsg_3);
        AlertManager.Instance.ShowAlert(AlertKey.StartMsg_4);
        
    }

    IEnumerator FadeOutView()
    {
        float timer = 0f;
        while (timer < 5.0f)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / 5.0f);
            fadeoutImg.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    IEnumerator FadeInView()
    {
        float timer = 0f;
        while (timer < 5.0f)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / 5.0f);
            fadeoutImg.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    IEnumerator FadeInOutText()
    {
        
        if (isCleared)
        {
            TextMeshProUGUI tmp = fadeinText.gameObject.GetComponent<TextMeshProUGUI>();
            tmp.text = "학교에 깃든 긴 어둠이 끝났습니다.";
            tmp.color = new Color(1, 1, 1, 1);
        }
        else
        {
            GlitchEffect.Instance.Play(7f);
        }

        float timer = 0f;
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
    }
    
    void GameOver()
    {
        isCleared = false;
        // UI 다 끄는 함수.  키입력 전체 비활성화 함수 필요

        // Enemy, Item Spawner 비활성화
        // Common Monster 전체 비활성화 + Item 오브젝트전체 비활성화
        // Mainmonster 비활성화 
        fadeOutObj.SetActive(true);
        SetInactiveObjects();
        playerInput.enabled = false;

        StartCoroutine(DeathScenario());
    }

    IEnumerator DeathScenario()
    {
        // float timer = 0f;
        playerCircleLight.SetActive(true);
        // yield return new WaitForSeconds(1f);

        playerAnimator.SetBool("IsDead", true);

        yield return new WaitForSeconds(3f);


        yield return StartCoroutine(FadeOutView());

        playerAnimator.SetBool("IsDead", false); 
        playerSprite.enabled = false;
        SoundManager.Instance.PlaySFX(SoundType.PlayerDied);

        yield return new WaitForSeconds(3f);
        fadeoutImg.color = new Color(0, 0, 0, 0);
        playerCircleLight.SetActive(true);
        playerSprite.enabled = true;
        // 애니메이션 - 메인몬스터 모습으로 웃는 Sprite 8프레임 정도..
        //  + 사운드 포함
        playerAnimator.SetBool("IsChanged", true);
        SoundManager.Instance.PlaySFX(SoundType.EnemyLaugh_1);


        yield return new WaitForSeconds(2.5f);
        SoundManager.Instance.PlaySFX(SoundType.EnemyLaugh_2);
        float timer = 0f;
        bool isSoundPlayed = false;

        while (timer < 5.0f)
        {
            timer += Time.deltaTime;
            if (timer >= 3.0f && !isSoundPlayed) 
            {
                SoundManager.Instance.PlaySFX(SoundType.EnemyLaugh_2);
                isSoundPlayed = true;
            }
            float alpha = Mathf.Lerp(0f, 1f, timer / 5.0f);
            fadeoutImg.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        timer = 0f;
        

        yield return new WaitForSeconds(1f);
        
        yield return StartCoroutine(FadeInOutText());

        GameSceneManager.Instance.LoadSceneByName("Ending");
    }


    void GameClear()
    {
        isCleared = true;
        // HUD UI, 입력값, 아이템, 커먼몬스터, 스포너 다 비활성화.
        fadeOutObj.SetActive(true);
        SetInactiveObjects();
        playerInput.enabled = false;

        StartCoroutine(ClearScenario());
    }

    IEnumerator ClearScenario()
    {
        // 암전효과
        yield return StartCoroutine(FadeOutView());

        // yield return new WaitForSeconds(5f);

        MainMonster mainMonsterCon = mainMonster.GetComponent<MainMonster>();
        mainMonsterCon.currentState = EnemyState.Idle;

        //플레이어를 비추던 카메라가 MainMonster를 비춰준다.
        cineCamera.Follow = mainMonster.transform;

        //MainMonster의 주위에 Sprite Light 2D 비춰줌
        fadeoutImg.color = new Color(0, 0, 0, 0);
        mainMonsterCircleLight.SetActive(true);
        mainMonsterAnimator.speed = 0.1f;
        mainMonsterAnimator.SetBool("IsDead", true);
        SoundManager.Instance.PlaySFXEnding(SoundType.EnemyScream, 0.5f);
        yield return new WaitForSeconds(1f);

        // MainMonster가 소멸되는 애니메이션 + 사운드

        // 암전 이후 다시 주인공 시점 카메라
        yield return StartCoroutine(FadeOutView());

        // yield return new WaitForSeconds(1f);
        // playerCircleLight.SetActive(true);
        cineCamera.Follow = player.transform;
        fadeoutImg.color = new Color(0, 0, 0, 0);


        float timer = 0f;

        // 플레이어가 위를 쳐다보는 애니메이션
        playerAnimator.SetBool("IsCleared", true);

        // Global Light가 점점 밝아진다. (기본 0, 목표값 1)
        while (timer < 5.0f)
        {
            timer += Time.deltaTime;
            float intensity = Mathf.Lerp(0f, 1f, timer / 5.0f);
            globalLight.intensity = intensity;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        // 화면 전체를 덮는 검은 패널로 Scene View를 Fade Out처리

        yield return StartCoroutine(FadeOutView());

        yield return new WaitForSeconds(2f);

        // 중앙에 텍스트 Fade In, FadeOut
        yield return StartCoroutine(FadeInOutText());

        // 그리고 엔딩씬으로 넘기기
        GameSceneManager.Instance.LoadSceneByName("Ending");
    }
}
