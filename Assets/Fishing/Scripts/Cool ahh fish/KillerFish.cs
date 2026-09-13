using UnityEngine;

public class KillerFish : MonoBehaviour
{
    public Rigidbody2D rb;
    public float reqVelocity;
    private void OnCollisionEnter(Collision collision)
    {
        if (rb.linearVelocity.sqrMagnitude > reqVelocity)
        {
            if (TryGetComponent(out Death death) && !death.speared && death.skewerable)
                death.Skewer();
        }
    }
}
