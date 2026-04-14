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
            // Alert으로 저주가 옮겨붙었다는 알림을 줘야할듯

            // 수치 적용할 함수도 만들어야됨..
            ApplyCuresEffect(selectedCurse.type);

        }
        else
        {
            // 모든 저주 중첩 시 Alert으로 알려줘야할듯
        }
    }


    // Stack 삭제 방식으로 저주 해제하는 코드
    public void RemoveLastCurse()
    {
        if (isCleanseOnCooldown)
        {
            // Alert - 정화 쿨타임 중
            return;
        }

        if (activeCurseStack.Count > 0)
        {
            CurseData cleansed = activeCurseStack.Pop();

            // 수치 새롭게 적용할 함수
            ApplyCuresEffect(cleansed.type);

            StartCoroutine(CleanseCooldownRoutine());
        }
    }

    // 여기서 합산해서 전달..?  or 각 스크립트에서 합산..?

    public IEnumerator CleanseCooldownRoutine()
    {
        isCleanseOnCooldown = true;
        yield return new WaitForSeconds(cleanseCooldownTime);
        isCleanseOnCooldown = false;

        // Alert - 해제 쿨다운이 종료되어서 해제를 할 수 있다는 것
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
