using UnityEngine;

public interface IPositionProvider
{
    Vector3 GetRandomPosition();
}

[RequireComponent(typeof(BoxCollider))]
public class SpawnBox : MonoBehaviour
{
    public Vector3 GetRandomPosition()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        Vector3 min = boxCollider.bounds.min;
        Vector3 max = boxCollider.bounds.max;
        
        return new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z)
        );
    }
}
