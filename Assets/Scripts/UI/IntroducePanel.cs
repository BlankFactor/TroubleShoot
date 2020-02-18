using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroducePanel : MonoBehaviour
{
    [Header("文本设置")]
    public string introduce = string.Empty;
    public float duration = 0.05f;
    private string tip = string.Empty;

    public Text text_Introduce;
    public Text text_Tip;
    private Animator ani;

    [Header("面板状态")]
    public bool readyToStart = false;

    void Start()
    {
        ani = GetComponent<Animator>();
        text_Introduce.text = null;
        tip = text_Tip.text;
        text_Tip.text = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (readyToStart) {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
                ani.SetBool("ReadyToStart", true);
            }
        }
    }

    public void StartGame() {
        GameManager.instance.StartGame();
        Destroy(gameObject);
    }

    public void DisplayIntroduce() {
        StartCoroutine(introduceShowder());
    }
    public void DisplayTip() {
        StartCoroutine(tipShowder()) ;
    }

    IEnumerator introduceShowder() {
        foreach (var i in introduce) {
            text_Introduce.text += i;

            yield return new WaitForSeconds(duration);
        }

        DisplayTip();
        readyToStart = true;
    }
    IEnumerator tipShowder() {
        foreach (var i in tip)
        {
            text_Tip.text += i;

            yield return new WaitForSeconds(duration);
        }
    }

    public void DisableBegningPanel() {
        UIManager.instance.SetBegnningPanel(false);
    }
}
