using System.Collections.Generic;
using UnityEngine;

public class NearbySensorDirector : DirectionProvider
{
    public NearbySensor sensor;
    public SpawnBoxReference clamper;
    public float clampStrength = 1f;
    [Header("Attraction Strength. Negative Repels.")]
    public float attractStrength = 1f;

    public override bool Can_Perform => IsEnabled();

    public override Vector3 GetDirection()
    {
        Vector3 targetDirection = Vector3.zero;
        foreach (Collider2D col in sensor.GetNearbyColliders())
        {
            Vector3 dir = col.transform.position - transform.position;
            dir *= attractStrength;
            targetDirection += dir.normalized / dir.magnitude;
        }
        if (clamper != null)
        {
            Vector3 clampedTarget = clamper.SpawnBox.Clamp(transform.position + targetDirection * clampStrength);
            targetDirection = clampedTarget - transform.position;
        }

        return targetDirection.normalized;
    }

    private bool IsEnabled()
    {
        return enabled && sensor.GetNearbyCount() > 0;
    }
}
