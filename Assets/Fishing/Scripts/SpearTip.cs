using UnityEngine;

public class SpearTip : MonoBehaviour
{
    public Spear spear;
    public Transform itemParent;
    public Spearable item;
    bool drop = false;

    private void OnTriggerEnter(Collider other)
    {
        if (CanSpear() && other.TryGetComponent(out Spearable spearable) && spearable.isSpearable)
        {
            SpearItem(spearable);
        }
    }

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
        if (state == SpearState.Throwing && item != null)
        {
            drop = true;
        }
        if (state == SpearState.Thrown && drop)
        {
            DropItem();
        }
    }

    private bool CanSpear()
    {
        // Only allow spearing if the spear is in the throwing state
        return item == null && spear.CurrentState == SpearState.Throwing;
    }

    private void SpearItem(Spearable itemToSpear)
    {
        itemToSpear.Spear(this);
        item = itemToSpear;
    }

    private void DropItem()
    {
        item.Unspear();
        item = null;
        drop = false;
    }
}
