using System;
using System.Collections.Generic;
using UnityEngine;

public class Eater : DirectionProvider
{
    public float munchDuration;
    bool eating;
    float lastMunchTime = -100;
    public bool IsEating => eating;
    public override bool Can_Perform => eating;
    public override Vector3 GetDirection()
    {
        return Vector3.zero;
    }

    public Action<Eater> OnEatStateChange;
    private void Update()
    {
        if (eating && Time.time - lastMunchTime > munchDuration)
        {
            eating = false;
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
            OnEatStateChange?.Invoke(this);
        }
    }
}
