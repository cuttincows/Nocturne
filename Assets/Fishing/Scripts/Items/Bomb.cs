using UnityEngine;

public class Bomb : MonoBehaviour
{
    public NearbySensor sensor;
    public void Explode()
    {
        foreach (Collider2D collider in sensor.GetNearbyColliders())
        {
            if (collider.TryGetComponent(out Death deat))
            {
                deat.Cook();
            }
        }
        Destroy(gameObject);
    }
}
