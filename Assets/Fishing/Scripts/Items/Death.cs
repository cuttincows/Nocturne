using UnityEngine;

public class Death : DirectionProvider
{
    bool dead = false;
    public float expirationTime = 10f;
    float expirationTimer;
    public SpriteRenderer spriteRenderer;
    public Sprite deadSprite;
    public override bool Can_Perform => dead;
    public void Kill()
    {
        spriteRenderer.sprite = deadSprite;
        dead = true;
    }

    private void Update()
    {
        if (dead)
        {
            expirationTimer += Time.deltaTime;
            float ratio = expirationTimer / expirationTime;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1 - ratio);
            if (expirationTimer >= expirationTime)
            {
                Destroy(gameObject);
            }
        }
    }

    public override Vector3 GetDirection()
    {
        return Vector3.zero;
    }
}
