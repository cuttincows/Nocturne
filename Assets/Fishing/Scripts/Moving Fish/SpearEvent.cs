using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class SpearEvent : MonoBehaviour
{
    public List<Behaviour> behaviours;
    public NearbySensor sensor;
    SpearState spearState;
    public UnityEvent onSpeared;

    private void Start()
    {
        Spear.OnAnySpearStateChanged += OnAnySpearStateChanged;
    }

    private void OnDestroy()
    {
        Spear.OnAnySpearStateChanged -= OnAnySpearStateChanged;
    }

    private void OnAnySpearStateChanged(Spear spear, SpearState state)
    {
        spearState = state;
        foreach (var behaviour in behaviours)
        {
            behaviour.enabled = IsSpearingNearby();
            
        }
        if (IsSpearingNearby())
        {
            onSpeared?.Invoke();
        }
    }

    public bool IsSpearingNearby()
    {
        return sensor.GetNearbyCount() > 0 && spearState == SpearState.Throwing;
    }
}
