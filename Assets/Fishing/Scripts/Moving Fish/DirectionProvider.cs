using UnityEngine;

public abstract class DirectionProvider : MonoBehaviour
{
    public abstract bool Can_Perform { get; }
    public abstract Vector3 GetDirection();
}
