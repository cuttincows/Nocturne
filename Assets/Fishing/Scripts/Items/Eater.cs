using System;
using System.Collections.Generic;
using UnityEngine;

public class Eater : MonoBehaviour
{
    public float munchDuration;
    public Stunnable stunner;
    public string EatTag = "Bait";
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out Eatable bait) && collision.collider.CompareTag(EatTag))
        {
            bait.GetEatenBy(this);
            stunner.Stun(munchDuration);
        }
    }
}
