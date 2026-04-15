using System.Collections;
using System.Collections.Generic;
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

    public bool IsCleanseOnCooldown => isCleanseOnCooldown;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
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
            int randomIdx = Random.Range(0, validCurse.Count);
            CurseData selectedCurse = validCurse[randomIdx];

            activeCurseStack.Push(selectedCurse);

            ApplyCuresEffect(selectedCurse.type);
            AlertManager.Instance.ShowAlert(AlertKey.GetCurse, selectedCurse.curseName);

        }
        else
        {
            AlertManager.Instance.ShowAlert(AlertKey.CannotGetCurse);
        }
    }


    // Stack 삭제 방식으로 저주 해제하는 코드
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
            StartCoroutine(CleanseCooldownRoutine());
        }
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

        //if (isCursed)
        //{
        //    // Alert으로 이동속도가 감소했다는 문구 넣으면 좋을듯
        //}
        //else
        //{
        //    // Alert으로 이동속도 저주가 풀렸다는 문구 넣으면 좋을듯
        //}
    }
    public List<CurseData> GetActiveCurseList()
    {
        return new List<CurseData>(activeCurseStack);
    }

}
