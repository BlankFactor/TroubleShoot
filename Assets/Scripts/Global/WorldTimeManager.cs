using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTimeManager : MonoBehaviour
{
    public static WorldTimeManager instance;

    [Header("当前时间状态")]
    public int day = 0;

    [Space]
    public int currentRound;

    [Header("回合设定")]
    [Range(1,10)]
    public int roundPerDay = 1;

    private List<Civilian> infectedCivilian = new List<Civilian>();
    private List<PublicPlace> publicPlaces = new List<PublicPlace>();

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentRound = roundPerDay;
    }

    /// <summary>
    /// 完成一回合
    /// </summary>
    public void FinishedRound() {
        currentRound--;

        // 天数结束
        if (currentRound.Equals(0)) {
            currentRound = roundPerDay;
            ReflashComaDay();
            
            day++;
        }


        ContactsMarker.instance.Reset();
        ReflashPublicPlances();

        GameObject.Find("PlayerController").SendMessage("ReflashActionPoint");
        GameObject.Find("PlayerController").SendMessage("DisableTempCheckers");

    }

    public void ReflashComaDay() {
        for (int i = 0; i < infectedCivilian.Count; i++) {
            infectedCivilian[i].ReflashComaDay();
            if (infectedCivilian[i].GetDay_Coma().Equals(0)) {
                infectedCivilian.Remove(infectedCivilian[i]);
            }
        }
    }

    /// <summary>
    /// 刷新公共区域感染情况
    /// </summary>
    public void ReflashPublicPlances() {
        foreach (var i in publicPlaces) {
            i.Infect();
        }
    }

    public void RemoveInfectedCivilian(Civilian _c) {
        infectedCivilian.Remove(_c);
    }
    public void AddInfectedCivilian(Civilian _c) {
        infectedCivilian.Add(_c);
    }
    public void AddPublicPlace(PublicPlace _p) {
        publicPlaces.Add(_p);
    }
}
