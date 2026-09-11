using UnityEngine;

public class ReenterBox : DirectionProvider
{
    public SpawnBoxReference targetBox;
    public override Vector3 GetDirection()
    {
        Vector3 offset = transform.position - targetBox.SpawnBox.transform.position;
        offset = targetBox.SpawnBox.Clamp(offset);
        Vector3 insideBoxPos = targetBox.SpawnBox.transform.position + offset;
        if (insideBoxPos == transform.position) return Vector3.zero;
        return (insideBoxPos - transform.position).normalized;
    }
}
