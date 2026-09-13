using UnityEngine;

public class Invisibility : DirectionProvider
{
    [SerializeField] SpriteRenderer spriteRenderer;
    public DirectionProvider reflection;

    public override bool Can_Perform => reflection.Can_Perform;

    public override Vector3 GetDirection()
    {
        LerpColor(growSpeed * Time.deltaTime);
        return reflection.GetDirection();
    }

    public float decaySpeed;
    public float growSpeed;
    private void Update()
    {
        // move toward less visible
        LerpColor(-decaySpeed * Time.deltaTime);
    }

    private void LerpColor(float step)
    {
        Color color = spriteRenderer.color;
        float alpha = Mathf.Clamp01(color.a + step);
        spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
    }
}
