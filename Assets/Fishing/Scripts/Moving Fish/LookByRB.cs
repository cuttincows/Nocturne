using UnityEngine;

public class LookByRB : MonoBehaviour
{
    public Rigidbody2D rb;
    public float rotationSpeed = 5f;
    void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;
        if (velocity != Vector3.zero)
        {
            transform.right = Vector3.Lerp(transform.right, velocity, rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
