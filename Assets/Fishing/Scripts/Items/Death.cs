using UnityEngine;

public class Death : DirectionProvider
{
    bool dead = false;
    public override bool Can_Perform => dead;
    public void Kill()
    {
        dead = true;
    }

    public override Vector3 GetDirection()
    {
        return Vector3.zero;
    }
}
