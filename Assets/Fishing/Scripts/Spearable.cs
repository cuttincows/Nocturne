using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spearable : MonoBehaviour
{
    public bool isSpearable = true;
    // Can be null if not speared
    public SpearTip spearTip;
    public List<Behaviour> disableWhileSpeared;
    public Collider2D col;
    public UnityEvent onSpeared;

    public bool Speared => spearTip != null;

    public void Spear(SpearTip spearTip)
    {
        foreach (var comp in disableWhileSpeared)
        {
            comp.enabled = false;
        }
        col.enabled = false;
        this.spearTip = spearTip;
        onSpeared?.Invoke();
        UpdatePos();
    }

    private void Update()
    {
        if (Speared) UpdatePos();
    }

    private void UpdatePos()
    {
        transform.position = spearTip.itemParent.position;
    }

    public void Unspear()
    {
        transform.position = spearTip.transform.position;
        this.spearTip = null;
        foreach (var comp in disableWhileSpeared)
        {
            comp.enabled = true;
        }
        col.enabled = true;
    }

    public void SetSpearable(bool spearable)
    {
        this.isSpearable = spearable;
    }
}
