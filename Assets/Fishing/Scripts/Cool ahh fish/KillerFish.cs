using UnityEngine;

public class KillerFish : MonoBehaviour
{
    public Rigidbody2D rb;
    public float reqVelocity;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enabled && rb.linearVelocity.sqrMagnitude > reqVelocity)
        {
            if (collision.collider.TryGetComponent(out Death death) && !death.speared && death.skewerable)
                death.Skewer();
        }
    }
}
