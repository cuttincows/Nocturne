using System.Collections.Generic;
using UnityEngine;

public class Clam : MonoBehaviour
{
    public Eater eater;
    public Death death;
    public Spearable spearable;
    public GameObject openVisuals;
    public GameObject closeVisuals;

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
        openVisuals.SetActive(spearable.isSpearable);
        closeVisuals.SetActive(!spearable.isSpearable);
    }

    private void Update()
    {
        if (death.dead && openVisuals.activeSelf)
        {
            spearable.isSpearable = true;
            openVisuals.SetActive(false);
            closeVisuals.SetActive(true);
        }
    }
}
