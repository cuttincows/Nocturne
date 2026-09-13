using System.Collections.Generic;
using UnityEngine;

public class NearbySensor : MonoBehaviour
{
    [Header("Does not avoid objects with these tags")]
    public List<string> blacklist;
    [Header("Only avoid objects with these tags, if enabled.")]
    public bool whitelistEnabled;
    public List<string> whitelist;
    List<Collider2D> untaggedNearbyColliders = new();
    List<Collider2D> taggedNearbyColliders = new();

    private bool Check(Collider2D other)
    {
        if (other == null) return false;
        if (!whitelistEnabled) return !blacklist.Contains(other.tag);
        return whitelist.Contains(other.tag);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Check(other))
            taggedNearbyColliders.Add(other);
        else
            untaggedNearbyColliders.Add(other);
    }

    public List<Collider2D> GetNearbyColliders()
    {
        PruneColliders();
        return new(taggedNearbyColliders);
    }

    private void PruneColliders()
    {
        List<Collider2D> collidersToRemove = new();
        foreach (var collider in untaggedNearbyColliders)
        {
            if (Check(collider))
            {
                collidersToRemove.Add(collider);
                taggedNearbyColliders.Add(collider);
            }
        }
        foreach (Collider2D col in collidersToRemove)
        {
            untaggedNearbyColliders.Remove(col);
        }

        collidersToRemove.Clear();
        foreach (var collider in taggedNearbyColliders)
        {
            if (collider == null)
            {
                collidersToRemove.Add(collider);
            }
        }
        foreach (Collider2D col in collidersToRemove)
        {
            taggedNearbyColliders.Remove(col);
        }
    }

    public int GetNearbyCount()
    {
        PruneColliders();
        return taggedNearbyColliders.Count;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        untaggedNearbyColliders.Remove(other);
        taggedNearbyColliders.Remove(other);
    }
}
