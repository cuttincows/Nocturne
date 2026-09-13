using UnityEngine;
using System;
using System.Collections.Generic;

public class Stunnable : DirectionProvider
{
    public bool IsStunned => timer > 0;
    public override bool Can_Perform => IsStunned;

    public Action<Stunnable> OnStunStateChange;

    public void Stun(float duration)
    {
        timer = duration;
        OnStunStateChange?.Invoke(this);
    }

    float timer;

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = 0;
                OnStunStateChange?.Invoke(this);
            }
        }
    }

    public override Vector3 GetDirection()
    {
        return Vector3.zero;
    }
}
