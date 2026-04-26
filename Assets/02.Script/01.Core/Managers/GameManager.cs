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
        // 모드 변경 관련 수정 필요.
        // GameSceneManager에서 받아서 이벤트 발행
        // 세팅에서 키보드/마우스 모드 선택 시 GameSceneManager의 이벤트 실행
        // GameManger에서는 시작 시 Boolean 값 따라 Cursor 분기 처리
        // GameManger에서 이벤트를 구독해서 재발행 해서 FlashlightManager, PurificationObject, FSMController에서 구독 업데이트.
        // 세팅에 패널 하나 더 추가. UI/UX 구상 필요.

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
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
        fadeOutObj.SetActive(true);
        SetInactiveObjects();
        playerInput.enabled = false;

        StartCoroutine(DeathScenario());
    }

    IEnumerator DeathScenario()
    {
        playerCircleLight.SetActive(true);

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
        fadeOutObj.SetActive(true);
        SetInactiveObjects();
        playerInput.enabled = false;

        StartCoroutine(ClearScenario());
    }

    IEnumerator ClearScenario()
    {
        yield return StartCoroutine(FadeOutView());

        MainMonster mainMonsterCon = mainMonster.GetComponent<MainMonster>();
        mainMonsterCon.currentState = EnemyState.Idle;

        cineCamera.Follow = mainMonster.transform;

        fadeoutImg.color = new Color(0, 0, 0, 0);
        mainMonsterCircleLight.SetActive(true);
        mainMonsterAnimator.speed = 0.1f;
        mainMonsterAnimator.SetBool("IsDead", true);
        SoundManager.Instance.PlaySFXEnding(SoundType.EnemyScream, 0.5f);
        yield return new WaitForSeconds(1f);


        yield return StartCoroutine(FadeOutView());

        cineCamera.Follow = player.transform;
        fadeoutImg.color = new Color(0, 0, 0, 0);


        float timer = 0f;

        playerAnimator.SetBool("IsCleared", true);

        while (timer < 5.0f)
        {
            timer += Time.deltaTime;
            float intensity = Mathf.Lerp(0f, 1f, timer / 5.0f);
            globalLight.intensity = intensity;
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(FadeOutView());

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(FadeInOutText());

        GameSceneManager.Instance.LoadSceneByName("Ending");
    }
}
