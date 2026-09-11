using UnityEngine;

public interface IPositionProvider
{
    Vector3 GetRandomPosition();
}

[System.Serializable]
public class SpawnBoxReference
{
    [Header("By Default, uses the most recently loaded SpawnBox unless this is assigned.")]
    [SerializeField] SpawnBox spawnBoxOverride;
    public SpawnBox SpawnBox => spawnBoxOverride != null ? spawnBoxOverride : SpawnBox.instance;
}

[RequireComponent(typeof(BoxCollider))]
public class SpawnBox : MonoBehaviour
{
    BoxCollider boxCollider;
    public static SpawnBox instance;

    private void Awake()
    {
        instance = this;
    }

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
