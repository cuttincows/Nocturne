using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SpearTip : MonoBehaviour
{
    public Spear spear;
    public Transform itemParent;
    public Spearable item;
    public float radius = 0.5f;
    public LayerMask mask;

    private void Start()
    {
        spear.OnStateChanged += OnSpearStateChanged;
    }

    private void OnDestroy()
    {
        spear.OnStateChanged -= OnSpearStateChanged;
    }

    private void OnSpearStateChanged(SpearState state)
    {
        if (state == SpearState.Thrown)
        {
            if (item != null)
                DropItem();
            else
                TrySpear();
        }
    }

    private void SpearItem(Spearable itemToSpear)
    {
        itemToSpear.Spear(this);
        item = itemToSpear;
    }

    private void TrySpear()
    {
        Spearable spearable = FishForSpearedItems();
        if (spearable != null)
            SpearItem(spearable);
    }

    private Spearable FishForSpearedItems()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask: mask);
        colliders = colliders.OrderBy(c => (c.transform.position - transform.position).sqrMagnitude).ToArray();
        foreach (Collider2D hit in colliders)
        {
            if (hit.TryGetComponent(out Spearable spearable) && spearable.isSpearable && spearable.enabled)
            {
                return spearable;
            }
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void DropItem()
    {
        item.transform.position = transform.position;
        item.Unspear();
        item = null;
    }
}
