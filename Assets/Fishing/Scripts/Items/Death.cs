using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// determines cooked, speared, and alive state.
/// really cooked and speared should be their own script but meh
/// </summary>
public class Death : DirectionProvider
{
    public bool dead { get; private set; }
    public float expirationTime = 10f;
    float expirationTimer;
    public SpriteRenderer spriteRenderer;
    public bool cookable;
    public bool skewerable;
    public bool speared {get; private set; }

    public UnityEvent onDead;
    public bool cooked { get; private set; }
    public override bool Can_Perform => dead;
    public void Cook()
    {
        if (!cookable) return;
        cooked = true;
        SetDead();
    }

    public void Skewer()
    {
        if (skewerable)
            SpearHole();
    }

    public void SpearHole()
    {
        speared = true;
        SetDead();
    }

    private void SetDead()
    {
        onDead?.Invoke();
        tag = "DeadFish";
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
