using System.Collections.Generic;
using UnityEngine;

public class Clam : MonoBehaviour
{
    public Stunnable stunnable;
    public Death death;
    public Spearable spearable;

    public void Start()
    {
        stunnable.OnStunStateChange += OnEatStateChange;
    }

    private void OnDestroy()
    {
        if (stunnable != null)
            stunnable.OnStunStateChange -= OnEatStateChange;
    }

    private void OnEatStateChange(Stunnable stunnable)
    {
        if (death.dead) return;
        spearable.isSpearable = stunnable.IsStunned;
    }

    private void Update()
    {
        if (death.dead && !spearable.isSpearable)
        {
            spearable.isSpearable = true;
        }
    }
}
