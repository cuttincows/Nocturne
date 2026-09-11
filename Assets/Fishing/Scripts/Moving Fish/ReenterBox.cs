using UnityEngine;

public class ReenterBox : DirectionProvider
{
    public SpawnBoxReference targetBox;
    public float tolerance = 0.1f;
    public override Vector3 GetDirection()
    {
        Vector3 insideBoxPos = targetBox.SpawnBox.Clamp(transform.position);
        if (Vector3.Distance(transform.position, insideBoxPos) < tolerance) return Vector3.zero;
        return (insideBoxPos - transform.position).normalized;
    }
}
