using System;
using System.Collections.Generic;
using UnityEngine;

public class Eater : MonoBehaviour
{
    public float munchDuration;
    bool eating;
    float lastMunchTime = -100;
    public bool IsEating => eating;

    public Action<Eater> OnEatStateChange;
    public List<Behaviour> disableWhileEaten;
    private void Update()
    {
        if (eating && Time.time - lastMunchTime > munchDuration)
        {
            eating = false;
            ToggleAll(true);
            OnEatStateChange?.Invoke(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out Bait bait))
        {
            bait.GetEatenBy(this);
            lastMunchTime = Time.time;
            eating = true;
            ToggleAll(false);
            OnEatStateChange?.Invoke(this);
        }
    }

    private void ToggleAll(bool on)
    {
        foreach (var comp in disableWhileEaten)
        {
            comp.enabled = on;
        }
    }
}
