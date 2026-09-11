using System.Collections.Generic;
using UnityEngine;

public class Clam : MonoBehaviour
{
    public Eater eater;
    public Spearable spearable;
    public GameObject openVisuals;

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
        spearable.isSpearable = eater.IsEating;
        openVisuals.SetActive(spearable.isSpearable);
    }
}
