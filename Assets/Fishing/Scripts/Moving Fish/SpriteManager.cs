using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    Sprite defaultSprite;
    public Death death;
    public Stunnable stunnable;
    public Sprite spearedAndCookedSprite;
    public Sprite spearedSprite;
    public Sprite cookedSprite;
    public Sprite eatingSprite;

    private void Start()
    {
        defaultSprite = spriteRenderer.sprite;
    }

    private void Update()
    {
        spriteRenderer.sprite = GetSprite();
    }


    public Sprite GetSprite()
    {
        if (death != null)
        {
            if (death.speared && death.cooked)
                return spearedAndCookedSprite;
            if (death.cooked)
                return cookedSprite;
            if (death.speared)
                return spearedSprite;
        }
        if (stunnable != null && stunnable.IsStunned)
            return eatingSprite;
        return defaultSprite;
    }
}
