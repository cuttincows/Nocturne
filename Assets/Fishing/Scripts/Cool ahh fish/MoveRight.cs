using UnityEngine;
public class MoveRight : DirectionProvider
{
    public bool left;
    public override bool Can_Perform => enabled;

    public override Vector3 GetDirection()
    {
        if (left) return Vector3.left;
        else return Vector3.right;
    }
}
