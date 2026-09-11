using System.Collections.Generic;
using UnityEngine;

public class NearbySensor : MonoBehaviour
{
    [Header("Does not avoid objects with these tags")]
    public List<string> blacklist;
    [Header("Only avoid objects with these tags, if enabled.")]
    public bool whitelistEnabled;
    public List<string> whitelist;
    [HideInInspector]
    List<Collider> nearbyColliders = new();

    private bool Check(Collider other)
    {
        if (!whitelistEnabled) return !blacklist.Contains(other.tag);
        return whitelist.Contains(other.tag);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Check(other)) return;
        nearbyColliders.Add(other);
    }

    public List<Collider> GetNearbyColliders()
    {
        List<Collider> collidersToRemove = new();
        foreach (var collider in nearbyColliders)
        {
            if (collider == null)
            {
                collidersToRemove.Add(collider);
            }
        }
        foreach (Collider col in collidersToRemove)
        {
            nearbyColliders.Remove(col);
        }
        return new(nearbyColliders);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Check(other)) return;
        nearbyColliders.Remove(other);
    }
}
