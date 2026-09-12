using System.Collections.Generic;
using UnityEngine;

public class NearbySensor : MonoBehaviour
{
    [Header("Does not avoid objects with these tags")]
    public List<string> blacklist;
    [Header("Only avoid objects with these tags, if enabled.")]
    public bool whitelistEnabled;
    public List<string> whitelist;
    List<Collider2D> nearbyColliders = new();

    private bool Check(Collider2D other)
    {
        if (!whitelistEnabled) return !blacklist.Contains(other.tag);
        return whitelist.Contains(other.tag);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Check(other)) return;
        nearbyColliders.Add(other);
    }

    public List<Collider2D> GetNearbyColliders()
    {
        PruneColliders();
        return new(nearbyColliders);
    }

    private void PruneColliders()
    {
        List<Collider2D> collidersToRemove = new();
        foreach (var collider in nearbyColliders)
        {
            if (collider == null)
            {
                collidersToRemove.Add(collider);
            }
        }
        foreach (Collider2D col in collidersToRemove)
        {
            nearbyColliders.Remove(col);
        }
    }

    public int GetNearbyCount()
    {
        PruneColliders();
        return nearbyColliders.Count;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!Check(other)) return;
        nearbyColliders.Remove(other);
    }
}
