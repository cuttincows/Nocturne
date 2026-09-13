using System.Collections.Generic;
using UnityEngine;

public class Clam : MonoBehaviour
{
    public Eater eater;
    public Death death;
    public Spearable spearable;

    public void Start()
    {
        eater.OnEatStateChange += OnEatStateChange;
    }

    private void OnDestroy()
    {
        if (eater != null)
            eater.OnEatStateChange -= OnEatStateChange;
    }

    private void OnEatStateChange(Eater eater)
    {
        if (death.dead) return;
        spearable.isSpearable = eater.IsEating;
    }

    private void Update()
    {
        if (death.dead && !spearable.isSpearable)
        {
            spearable.isSpearable = true;
        }
    }
}
