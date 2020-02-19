using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("声音片")]
    public AudioClip bgm;
    public AudioClip clickButton;
    public AudioClip clickButtonInvild;
    public AudioClip infect;
    public AudioClip checkSuccess;
    public AudioClip checkFailed;
    public AudioClip coma;
    public AudioClip typer;
    public AudioClip disnifect;

    [Space]
    public AudioSource music;
    public AudioSource soundEffect;

    [Header("音量设定")]
    [Range(0,1)]
    public float volumn_BGM = 0.5f;
    [Range(0, 1)]
    public float volumn_SE = 0.5f;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        music.volume = volumn_BGM;
        soundEffect.volume = volumn_SE;
    }

    private void Update()
    {
        soundEffect.volume = volumn_SE;
    }

    public void SetVolumn_BGM(float _v) {
        volumn_BGM = _v;
        if (GameManager.instance.gameStart) {
            music.volume = volumn_BGM;
        }
    }
    public void SetVolumn_SE(float _v) {
        volumn_SE = _v;
        soundEffect.volume = volumn_SE;
    }

    public void Play_BGM() {
        music.clip = bgm;
        music.Play();
        StartCoroutine(AppearVolumn());
    }
    public void Stop_BGM() {
        StartCoroutine(DisappearVolumn());
    }

    public void Play_Typer() {
        music.volume = volumn_BGM;
        music.clip = typer;
        music.Play();
    }
    public void Stop_Typer() {
        music.volume = 0;
        music.Stop();
    }

    IEnumerator AppearVolumn() {
        while (music.volume != volumn_BGM) {
            music.volume += Time.deltaTime * 0.5f;

            music.volume = Mathf.Clamp(music.volume, 0, volumn_BGM);

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    IEnumerator DisappearVolumn()
    {
        while (music.volume != 0)
        {
            music.volume -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        music.Stop();
    }

    public void PlaySE_ClickButton() {
        soundEffect.clip = clickButton;
        soundEffect.Play();
    }
    public void PlaySE_ClickButtonInvild()
    {
        soundEffect.clip = clickButtonInvild;
        soundEffect.Play();
    }
    public void PlaySE_Infect()
    {
        if (soundEffect.isPlaying) return;
        soundEffect.clip = infect;
        soundEffect.Play();
    }
    public void PlaySE_CheckSuccess()
    {
        soundEffect.clip = checkSuccess;
        soundEffect.Play();
    }
    public void PlaySE_CheckFailed()
    {
        soundEffect.clip = checkFailed;
        soundEffect.Play();
    }
    public void PlaySE_Coma()
    {
        if (soundEffect.isPlaying) return;
        soundEffect.clip = coma;
        soundEffect.Play();
    }
    public void PlaySE_Disnifect()
    {
        soundEffect.clip = disnifect;
        soundEffect.Play();
    }

}
