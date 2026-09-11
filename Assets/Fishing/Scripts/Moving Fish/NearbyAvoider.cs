using System.Collections.Generic;
using UnityEngine;

public class NearbyAvoider : DirectionProvider
{
    [Header("Does not avoid objects with these tags")]
    public List<string> blacklist;
    [HideInInspector]   
    public List<Collider> nearbyColliders = new();
    public SpawnBox clamper;
    public float clampStrength = 1f;
    public float centerBias = 0.5f;

    public override Vector3 GetDirection()
    {
        Vector3 avoidanceDirection = Vector3.zero;
        List<Collider> collidersToRemove = new();
        foreach (Collider col in nearbyColliders)
        {
            if (col == null) continue; // Skip null colliders
            Vector3 dir = transform.position - col.transform.position;
            
            avoidanceDirection += dir.normalized / dir.magnitude;
        }
        foreach (Collider col in collidersToRemove)
        {
            nearbyColliders.Remove(col);
        }
        Vector3 clampedTarget = clamper.Clamp(transform.position + avoidanceDirection * clampStrength);
        Vector3 centerDirection = (clamper.transform.position - transform.position).normalized * centerBias;
        return (clampedTarget - transform.position + centerDirection).normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (blacklist.Contains(other.tag)) return;
        nearbyColliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (blacklist.Contains(other.tag)) return;
        nearbyColliders.Remove(other);
    }
}
