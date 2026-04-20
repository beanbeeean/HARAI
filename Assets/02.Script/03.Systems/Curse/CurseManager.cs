using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurseManager : MonoBehaviour
{
    public static CurseManager instance;


    [Header("Curse DB")]
    [SerializeField] private List<CurseData> allCurseDatabase = new List<CurseData>();
    [SerializeField] private List<CurseData> activeCurseList;
    

    [Header("Current Status")]
    private Stack<CurseData> activeCurseStack = new Stack<CurseData>();
    [SerializeField] private bool isCleanseOnCooldown = false;
    public float currentCleanseCDT = 0f;
    [SerializeField] private float cleanseCooldownTime = 30f;
    [SerializeField] private int maxCount = 3;

    [Header("Ref")]
    [SerializeField] private PlayerMove2D playerMove2D;
    [SerializeField] private PlayerHPManager playerHPManager;
    [SerializeField] private FlashlightManager flashlightManager;


    [Header("Conditions")]
    [SerializeField] private int maxTotalCurse = 10;
    [SerializeField] private int currentTotalCurse = 0;


    [Header("Gimmicks")]
    [SerializeField] private float gimmickTimer = 5f;
    [SerializeField] private float gimmcikRepeatTime = 120f;
    [SerializeField] private bool successGimmick = true;
    [SerializeField] private TextMeshProUGUI gimmickTimerUIText;

    [Header("Effect")]
    [SerializeField] private GameObject curseEffectPrefab;

    public int MaxTotalCurse => maxTotalCurse;
    public int CurrentTotalCurse => currentTotalCurse;

    public bool IsCleanseOnCooldown => isCleanseOnCooldown;
    public Action CurseGameoverEvent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InvokeRepeating("SuddenGimmick", gimmcikRepeatTime, gimmcikRepeatTime);

        // 테스트용
        // InvokeRepeating("SuddenGimmick", 15f, gimmcikRepeatTime);
    }

    public void AddRandomCurse()
    {
        List<CurseData> validCurse = new List<CurseData>();

        foreach (CurseData curse in allCurseDatabase)
        {
            int count = 0;

            foreach(CurseData activeCurse in activeCurseStack)
            {
                if(activeCurse.curseID == curse.curseID)
                {
                    count++;
                }
            }

            if(count < maxCount)
            {
                validCurse.Add(curse);
            }
        }

        if (validCurse.Count > 0)
        {
            int randomIdx = UnityEngine.Random.Range(0, validCurse.Count);
            CurseData selectedCurse = validCurse[randomIdx];

            activeCurseStack.Push(selectedCurse);

            ApplyCuresEffect(selectedCurse.type);
            AlertManager.Instance.ShowAlert(AlertKey.GetCurse, selectedCurse.curseName);
            currentTotalCurse++;
            InfoManager.Instance.UpdateCurseInfo(activeCurseStack.Count);

            if (curseEffectPrefab != null)
            {
                StartCoroutine(ShowCurseEffect());
            }
            
        }
        else
        {
            AlertManager.Instance.ShowAlert(AlertKey.CannotGetCurse);
        }

        if (currentTotalCurse >= maxTotalCurse)
        {
            Debug.Log("CurseGameoverEvent Start");
            CurseGameoverEvent?.Invoke();
        }
        
        if(maxTotalCurse - 1 == currentTotalCurse)
        {
            AlertManager.Instance.ShowAlert(AlertKey.CurseWarning);
        }
    }


    public void RemoveLastCurse()
    {
        if (isCleanseOnCooldown)
        {
            return;
        }

        if (activeCurseStack.Count > 0)
        {
            CurseData cleansed = activeCurseStack.Pop();

            ApplyCuresEffect(cleansed.type);
            AlertManager.Instance.ShowAlert(AlertKey.CleanseCurse);
            currentTotalCurse--;
            StartCoroutine(CleanseCooldownRoutine());
        }

        InfoManager.Instance.UpdateCurseInfo(activeCurseStack.Count);
    }

    public IEnumerator ShowCurseEffect()
    {
        curseEffectPrefab.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        curseEffectPrefab.SetActive(false);
    }
    public IEnumerator CleanseCooldownRoutine()
    {
        isCleanseOnCooldown = true;
        currentCleanseCDT = cleanseCooldownTime;

        while (currentCleanseCDT > 0)
        {
            currentCleanseCDT -= Time.deltaTime;
            yield return null;
        }

        currentCleanseCDT = 0f;
        isCleanseOnCooldown = false;
    }

    public void ApplyCuresEffect(CurseType selectedType)
    {

        CommonMonster[] allMonsters = FindObjectsByType<CommonMonster>(FindObjectsSortMode.None);

        float totalValue = 0;
        foreach (CurseData curse in activeCurseStack)
        {
            if (curse.type == selectedType) totalValue += curse.value;
        }
        switch (selectedType)
        {
            case CurseType.MoveSpeed:
                playerMove2D.UpdateMoveSpeed(totalValue);
                break;
            case CurseType.BatteryRecharge:
                flashlightManager.UpdateRechargeTime(totalValue);
                break;
            case CurseType.MaxHp:
                playerHPManager.UpdateMaxHp(totalValue);
                break;
            case CurseType.MaxBattery:
                flashlightManager.UpdateMaxPower(totalValue);
                break;
            case CurseType.MonsterDamage:
                foreach (var monster in allMonsters)
                {
                    monster.UpdateAttackDamage(totalValue);
                }
                break;
            case CurseType.MonsterDetection:
                foreach (var monster in allMonsters)
                {
                    monster.UpdateChaseRange(totalValue);
                }
                
                break;
            default: break;
        }

        activeCurseList.Clear();

        activeCurseList = GetActiveCurseList();

    }
    public List<CurseData> GetActiveCurseList()
    {
        return new List<CurseData>(activeCurseStack);
    }

    void SuddenGimmick()
    {
        if(currentTotalCurse >= 10 || PurificationManager.Instance.CurrentPurified >= 10 || playerHPManager.IsDead)
        {
            CancelInvoke("SuddenGimmick");
            return;
        }
        successGimmick = true;
        int randomIdx = UnityEngine.Random.Range(0, 2);
        Debug.Log($"{randomIdx}번 기믹");

        // 테스트용
        // randomIdx = 1;

        switch (randomIdx)
        {
            case 0:
                StartCoroutine(MovingGimmick());
                break;
            case 1:
                StartCoroutine(LightGimmick());
                break;
        }
    }

    IEnumerator MovingGimmick()
    {
        AlertManager.Instance.ShowAlert(AlertKey.StartMovingGimmick);

        yield return new WaitForSeconds(4f);


        Vector3 markingPlayerPos = new Vector3();
        markingPlayerPos.x = playerMove2D.gameObject.transform.position.x;
        markingPlayerPos.y = playerMove2D.gameObject.transform.position.y;
        float timer = 0f;

        while (timer < gimmickTimer)
        {
            timer += Time.deltaTime;
            ShowGimmickTimeUI(gimmickTimer - timer);
            Vector3 pos = playerMove2D.gameObject.transform.position;
            Debug.Log("pos : " + pos);
            if (markingPlayerPos != pos)
            {
                // + 이미지 추가 예정
                SoundManager.Instance.PlaySFX("EnemyLaugh_2");
                Debug.Log("기믹 실패");
                AlertManager.Instance.ShowAlert(AlertKey.FailMovingGimmick);

                AddRandomCurse();
                successGimmick = false;

                break;
            }

            yield return null;
        }

        gimmickTimerUIText.gameObject.SetActive(false);

        if (successGimmick)
        {

            AlertManager.Instance.ShowAlert(AlertKey.SuccessMovingGimmick);

            SoundManager.Instance.PlaySFX("MainMonsterDied");
            AlertManager.Instance.ShowAlert(AlertKey.SuccessGimmick);
        }
    }
    
    IEnumerator LightGimmick()
    {
        AlertManager.Instance.ShowAlert(AlertKey.StartLightGimmick);

        yield return new WaitForSeconds(4f);

        float timer = 0f;

        while (timer < gimmickTimer)
        {
            timer += Time.deltaTime;
            ShowGimmickTimeUI(gimmickTimer - timer);
            if (flashlightManager.isPowerOn)
            {
                // + 이미지 추가 예정
                SoundManager.Instance.PlaySFX("EnemyLaugh_2");
                Debug.Log("기믹 실패");
                AlertManager.Instance.ShowAlert(AlertKey.FailLightGimmick);

                AddRandomCurse();
                successGimmick = false;

                break;
            }

            yield return null;
        }

        gimmickTimerUIText.gameObject.SetActive(false);

        if (successGimmick)
        {

            AlertManager.Instance.ShowAlert(AlertKey.SuccessLightGimmick);

            // + SoundManager Name을 Key로 관리해야할듯.
            SoundManager.Instance.PlaySFX("MainMonsterDied");

            AlertManager.Instance.ShowAlert(AlertKey.SuccessGimmick);
        }
    }
    

    void ShowGimmickTimeUI(float timer)
    {
        if (!gimmickTimerUIText.gameObject.activeSelf)
        {
            gimmickTimerUIText.gameObject.SetActive(true);
        }
        gimmickTimerUIText.text = String.Format($"[령]의 속삭임에 넘어가서는 안됩니다.\n남은 시간 : {(int)timer}초");
    }


}
