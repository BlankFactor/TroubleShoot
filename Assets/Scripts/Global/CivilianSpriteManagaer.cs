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

    private void Awake()
    {
        instance = this;
    }

    public Sprite GetSprtie_Normal() { return sprite_Normal; }

    public Color GetSpriteNormal() { return Color.white; }
    public Color GetSpriteInfect(){return Color.black;}
}
