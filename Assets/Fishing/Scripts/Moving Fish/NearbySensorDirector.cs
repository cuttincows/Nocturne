using System.Collections.Generic;
using UnityEngine;

public class NearbySensorDirector : DirectionProvider
{
    public NearbySensor sensor;
    public SpawnBox clamper;
    public float clampStrength = 1f;
    // strange should remove later.
    public float centerBias = 0.5f;
    [Header("Attraction Strength. Negative Repels.")]
    public float attractStrength = 1f;

    public override Vector3 GetDirection()
    {
        if (!enabled) return Vector3.zero;
        Vector3 targetDirection = Vector3.zero;
        foreach (Collider col in sensor.GetNearbyColliders())
        {
            Vector3 dir = col.transform.position - transform.position;
            dir *= attractStrength;
            targetDirection += dir.normalized / dir.magnitude;
        }
        if (clamper != null)
        {
            Vector3 clampedTarget = clamper.Clamp(transform.position + targetDirection * clampStrength);
            // ): uhrm for when you dont want to leave so hard.
            Vector3 centerDirection = (clamper.transform.position - transform.position).normalized * centerBias;
            targetDirection = (clampedTarget - transform.position) + centerDirection;
        }

        return targetDirection.normalized;
    }
}
