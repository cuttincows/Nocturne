using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    public List<Sprite> sprites;
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        Randomize();   
    }

    private void Randomize()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Count)];
    }
}
