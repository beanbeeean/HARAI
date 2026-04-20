using System;
using TMPro;
using UnityEngine;

public class InfoManager : MonoBehaviour
{
    public static InfoManager Instance;

    [SerializeField] private TextMeshProUGUI purifyInfo;
    [SerializeField] private TextMeshProUGUI curseInfo;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateCurseInfo(int curseCount)
    {
        curseInfo.text = String.Format($"저주 중첩 : {curseCount}/10");
    }
    
    public void UpdatePurifyInfo(int purifyCount)
    {
        purifyInfo.text = String.Format($"정화 목표 : {purifyCount}/10");
    }
}
