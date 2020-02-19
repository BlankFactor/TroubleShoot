using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianSpriteManagaer : MonoBehaviour
{
    public static CivilianSpriteManagaer instance;

    [Header("精灵")]
    public Sprite sprite_Normal;
    public Sprite sprite_Cough;
    public Sprite sprite_Heat;
    public Sprite sprite_Masking;

    private void Awake()
    {
        instance = this;
    }

    public Sprite GetSprtie_Normal() { return sprite_Normal; }
    public Sprite GetSprtie_Cough() { return sprite_Cough; }
    public Sprite GetSprtie_Heat() { return sprite_Heat; }
    public Sprite GetSprtie_Masking() { return sprite_Masking; }

    public Color GetSpriteNormal() { return Color.white; }
    public Color GetSpriteInfect(){return Color.black;}
}
