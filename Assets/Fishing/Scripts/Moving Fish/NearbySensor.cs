using System.Collections.Generic;
using UnityEngine;

public class NearbySensor : MonoBehaviour
{
    [Header("Does not avoid objects with these tags")]
    public List<string> blacklist;
    [Header("Only avoid objects with these tags, if enabled.")]
    public bool whitelistEnabled;
    public List<string> whitelist;
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
        PruneColliders();
        return new(nearbyColliders);
    }

    private void PruneColliders()
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
    }

    public int GetNearbyCount()
    {
        PruneColliders();
        return nearbyColliders.Count;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!Check(other)) return;
        nearbyColliders.Remove(other);
    }
}
