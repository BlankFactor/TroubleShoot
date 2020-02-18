using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("面板对象")]
    public GameObject panel_Command;
    public GameObject panel_APRecorder;
    public GameObject panel_Begning;
    public GameObject panel_Introduce;

    [Header("按钮对象")]
    public Button button_Disni;
    public Button button_Detect;
    public Button button_Troubleshoot;
    public Button button_TemperMeasu;

    [Space]
    public Button button_Start;
    public Button button_Exit;

    [Header("文本对象")]
    public TextMeshProUGUI text_APRecorder;

    private void Awake()
    {
        instance = this;

        // 检测游戏是否初次启动 仅打包exe时去掉注释
        // CheckFirstStartGame();
    }

    public void DisplayPanel_Command() {
        panel_Command.SetActive(true);
    }
    public void DisablePanel_Command()
    {
        panel_Command.SetActive(false);
    }

    public void DisplayPanel_APRecorder() {
        panel_APRecorder.SetActive(true);
    }
    public void DisablePanel_APRecorder() {
        panel_APRecorder.SetActive(false);
    }

    public void SetBegnningPanel(bool _v) {
        panel_Begning.SetActive(_v);
    }

    public void ReflashAPRecorder(int _ap) {
        text_APRecorder.text = _ap.ToString();
        text_APRecorder.color = Color.black;
    }

    /// <summary>
    /// 进入主游戏
    /// </summary>
    public void EnterMainGame() {
        panel_Introduce.SetActive(true);
        button_Start.GetComponent<Animator>().enabled = true;
        button_Exit.GetComponent<Animator>().enabled = true;
    }

    /// <summary>
    /// 退出程序
    /// </summary>
    public void ExitGame() {
        button_Start.GetComponent<Animator>().enabled = true;
        button_Exit.GetComponent<Animator>().enabled = true;
        Application.Quit();
    }

    public void CheckFirstStartGame()
    {
        // 若当前值为 1 则非初次启动游戏 跳过开始界面
        if (PlayerPrefs.GetInt("AppFirstStart") == 1)
        {
            panel_Begning.SetActive(false);
            GameManager.instance.StartGame();
        }
    }

    public void ReflashCommandPanel(int _ap,int _dis,int _det,int _trouble,int _temper) {
        button_Disni.interactable = _ap - _dis < 0 ? false : true;
        button_Detect.interactable = _ap - _det < 0 ? false : true;
        button_Troubleshoot.interactable = _ap - _trouble < 0 ? false : true;
        button_TemperMeasu.interactable = _ap - _temper < 0 ? false : true;
    } 
}
