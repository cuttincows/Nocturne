using UnityEngine;
using UnityEngine.Events;

public class Bomb : MonoBehaviour
{
    public NearbySensor sensor;
    public UnityEvent onExplode;
    public void Explode()
    {
        foreach (Collider2D collider in sensor.GetNearbyColliders())
        {
            if (collider.TryGetComponent(out Death deat))
            {
                deat.Cook();
            }
        }
        onExplode.Invoke();
        Destroy(gameObject);
    }
}
