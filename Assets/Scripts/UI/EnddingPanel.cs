using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnddingPanel : MonoBehaviour
{

    public string string_Victory;
    public string string_Failed;

    [Space]
    public float duration = 0.05f;

    [Space]
    public bool readyToExit;

    [Space]
    private string mainText;
    private string tipString;
    public Text text;
    public Text tip;

    private void Start()
    {
        text.text = null;
        tipString = tip.text;
        tip.text = null;
    }

    public void Display(bool _victory) {
        if (_victory)
        {
            mainText = string_Victory;
        }
        else {
            mainText = string_Failed;
        }

        gameObject.SetActive(true);
    }

    public void DisplayText() {
        AudioManager.instance.Play_Typer();
        StartCoroutine(textShowder());
    }

    IEnumerator textShowder() {
        foreach (var i in mainText) {
            text.text += i;
            yield return new WaitForSeconds(duration);
        }

        StartCoroutine(tipShowder());
    }

    IEnumerator tipShowder() {
        foreach (var i in tipString)
        {
            tip.text += i;
            yield return new WaitForSeconds(duration);
        }

        SetReady();
    }

    public void SetReady() {
        readyToExit = true;
        AudioManager.instance.Stop_Typer();
    }

    // Update is called once per frame
    void Update()
    {
        if (readyToExit) {
            if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                Application.Quit();
        }    
    }
}
