using UnityEngine;

public class Spearable : MonoBehaviour
{
    public bool isSpearable = true;
    // Can be null if not speared
    public SpearTip spearTip;
    public Collider col;

    public bool Speared => spearTip != null;

    public void Spear(SpearTip spearTip)
    {
        col.enabled = false;
        this.spearTip = spearTip;
        UpdatePos();
    }

    private void Update()
    {
        if (Speared) UpdatePos();
    }

    private void UpdatePos()
    {
        transform.position = spearTip.transform.position;
    }

    public void Unspear()
    {
        UpdatePos();
        this.spearTip = null;
        col.enabled = true;
    }
}
