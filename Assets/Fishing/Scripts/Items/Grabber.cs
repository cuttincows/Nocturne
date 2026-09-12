using UnityEngine;
using System.Collections.Generic;

public class Grabber : MonoBehaviour
{
    public Collider2D col;
    Grabbable grabbable;
    public float grabCooldown = 2f;
    float lastGrabTime = -100;
    public List<Behaviour> disableWhenGrabbing;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time - lastGrabTime < grabCooldown || this.grabbable != null) return;
        if (collision.collider.TryGetComponent(out Grabbable grabbable))
        {
            if (grabbable.Grab(this))
            {
                lastGrabTime = Time.time;
                this.grabbable = grabbable;
                ToggleAll(false);
                col.enabled = false;
            }
        }
    }

    private void ToggleAll(bool value)
    {
        foreach (Behaviour behaviour in disableWhenGrabbing)
        {
            behaviour.enabled = value;
        }
    }

    public void Detach(Grabbable grabbable)
    {
        if (grabbable == this.grabbable)
        {
            col.enabled = true;
            ToggleAll(true);
            this.grabbable = null;
        }
    }
}
