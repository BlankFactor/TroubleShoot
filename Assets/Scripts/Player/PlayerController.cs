﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("玩家当前状态")]
    public int remain_AP;

    [Space]
    public bool selectedCommand;
    public CommandType cur_Command = CommandType.None;

    [Header("玩家设定")]
    [Range(1,5)]
    public int count_AP_PerRound = 4;
    public int dec_Command_Disnifect = 1;
    public int dec_Command_Troubleshoot = 3;
    public int dec_Command_Detect = 1;
    public int dec_Command_TemperMeasu = 2;

    [Space]
    [Range(1,5)]
    public int range_TempChecker = 1;

    [Space]
    public LayerMask unitLayer;
    private Camera cam;

    [Header("指令对象")]
    public GameObject temperatureChecker;
    public List<GameObject> tempCheckers = new List<GameObject>();

    public enum CommandType {
        None = -1,
        Disnifect,
        Troubleshoot,
        Detect,
        TemperMeasur
    }

    void Start()
    {
        cam = Camera.main;
        remain_AP = count_AP_PerRound;
    }

    // Update is called once per frame
    void Update()
    {
        // 点数为 0 下一次点击操作为进入切换到npc移动回合
        if (remain_AP.Equals(0))
        {
            if (Input.GetMouseButtonDown(0))
            {
                ReflashActionPoint();
            }
        }
        else
        {
            Action();
            DisplayTempCheckerRange();
        }
    }

    void Action() {
        if (selectedCommand && Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(ray.origin.x, ray.origin.y), Vector2.zero);

            if (hit)
            {
                ExecuteCommand(hit);
            }
            else
            {
                ResetCommand();
            }
        }
        else if (Input.GetMouseButtonDown(1)) {
            ResetCommand();
        }
    }

    public void SetCommand_Disnifect() {
        if (remain_AP - dec_Command_Disnifect < 0) return;
        ResetCommand();

        cur_Command = CommandType.Disnifect;
        selectedCommand = true;
    }
    public void SetCommand_Troubleshoot()
    {
        if (remain_AP - dec_Command_Troubleshoot < 0) return;
        ResetCommand();

        cur_Command = CommandType.Troubleshoot;
        selectedCommand = true;
    }
    public void SetCommand_Detect() {
        if (remain_AP - dec_Command_Detect < 0) return;
        ResetCommand();

        cur_Command = CommandType.Detect;
        selectedCommand = true;
    }
    public void SetCommand_TemperMeasur() {
        if (remain_AP - dec_Command_TemperMeasu < 0) return;
        ResetCommand();

        cur_Command = CommandType.TemperMeasur;
        selectedCommand = true;

        temperatureChecker.SetActive(true);
        temperatureChecker.transform.localScale = new Vector2(range_TempChecker * 3 - 0.05f, range_TempChecker * 3 - 0.05f);
    }
    public void ResetCommand() {
        cur_Command = CommandType.None;
        selectedCommand = false;

        // 关闭指令范围
        temperatureChecker.SetActive(false);
    }
    public void ExecuteCommand(RaycastHit2D _hit) {
        switch (cur_Command) {
            case CommandType.Disnifect:
                {
                    if (_hit.transform.tag.Equals("PublicPlace")) {
                        _hit.transform.SendMessage("Disnifect");
                        remain_AP -= dec_Command_Disnifect;
                    }
                    break;
                }
            case CommandType.Detect: {
                    if (_hit.transform.tag.Equals("Civilian")) {
                        if (_hit.transform.GetComponent<Civilian>().Get_Infected()) {
                            _hit.transform.GetComponent<Civilian>().SendToHosipital(); 
                        }
                        remain_AP -= dec_Command_Detect;
                    }

                    break;
                }
            case CommandType.Troubleshoot: {
                    if (_hit.transform.tag.Equals("Civilian"))
                    {
                        if (_hit.transform.GetComponent<Civilian>().Get_Infected())
                        {
                            _hit.transform.GetComponent<Civilian>().ShowContacts();
                            remain_AP -= dec_Command_Troubleshoot;
                        }
                    }
                    break;
                }
            case CommandType.TemperMeasur: {
                    GameObject tc = Instantiate<GameObject>(temperatureChecker);
                    tc.SendMessage("Create");
                    tempCheckers.Add(tc);
                    remain_AP -= dec_Command_TemperMeasu;
                    break;
                }
            default:break;
        }

        //ReflashActionPoint();
        ResetCommand();
    }

    void DisableTempCheckers() {
        foreach (var i in tempCheckers) {
            Destroy(i);
        }

        tempCheckers.Clear();
    }

    void DisplayTempCheckerRange()
    {
        if (!selectedCommand) return;

        switch (cur_Command) {
            case CommandType.TemperMeasur:{
                    Vector2 sp = cam.ScreenToWorldPoint(Input.mousePosition);
                    temperatureChecker.transform.position = AmendPos(sp);
                    break;
                }
            default:break;
        }
    }

    /// <summary>
    /// 坐标修正
    /// </summary>
    /// <param name="_pos"></param>
    /// <returns></returns>
    private Vector2 AmendPos(Vector2 _pos)
    {
        Vector2 dis;

        if (_pos.x < 0)
        {
            float inter = Mathf.Abs(_pos.x);
            dis.x = -(Mathf.Floor(inter) + 0.5f);
        }
        dis.x = Mathf.Floor(_pos.x) + 0.5f;

        if (_pos.y < 0)
        {
            float inter = Mathf.Abs(_pos.y);
            dis.y = -(Mathf.Floor(inter) + 0.5f);
        }
        dis.y = Mathf.Floor(_pos.y) + 0.5f;

        return dis;
    }

    public void ReflashActionPoint() {
        if (remain_AP.Equals(0))
        {
            GameManager.instance.TurnToRound_Civilian();
            remain_AP = count_AP_PerRound;
        }
    }
}
