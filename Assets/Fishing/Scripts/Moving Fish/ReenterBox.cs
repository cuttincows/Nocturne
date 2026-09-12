using UnityEngine;

public class ReenterBox : DirectionProvider
{
    public SpawnBoxReference targetBox;
    public float tolerance = 0.1f;

    public override bool Can_Perform => IsOutsideBox() && enabled;

    private bool IsOutsideBox()
    {
        Vector3 insideBoxPos = targetBox.SpawnBox.Clamp(transform.position);
        return Vector3.Distance(transform.position, insideBoxPos) > tolerance;
    }

    public override Vector3 GetDirection()
    {
        Vector3 insideBoxPos = targetBox.SpawnBox.Clamp(transform.position);
        return (insideBoxPos - transform.position).normalized;
    }
}
