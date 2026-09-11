using UnityEngine;

public interface IPositionProvider
{
    Vector3 GetRandomPosition();
}

[RequireComponent(typeof(BoxCollider))]
public class SpawnBox : MonoBehaviour
{
    BoxCollider boxCollider;

    public Vector3 GetRandomPosition()
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
        Vector3 min = boxCollider.bounds.min;
        Vector3 max = boxCollider.bounds.max;
        
        return new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z)
        );
    }

    public Vector3 Clamp(Vector3 input)
    {
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider>();
        Vector3 min = boxCollider.bounds.min;
        Vector3 max = boxCollider.bounds.max;

        return new Vector3(
            Mathf.Clamp(input.x, min.x, max.x),
            Mathf.Clamp(input.y, min.y, max.y),
            Mathf.Clamp(input.z, min.z, max.z)
        );
    }
}
