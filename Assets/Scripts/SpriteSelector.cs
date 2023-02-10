using System;
using UnityEngine;

public class SpriteSelector : MonoBehaviour
{
    public Sprite[] Sprites;
    private static SpriteSelector Instance;

    private void Awake()
    {
        Instance = this;
    }

    public static Sprite GetSpriteFromInt(int value)
    {
        return value < 0 ? null : Instance.Sprites[value];
    }
}