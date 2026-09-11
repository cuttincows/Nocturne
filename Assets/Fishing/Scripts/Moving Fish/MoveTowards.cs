using UnityEngine;


public class MoveTowards : MonoBehaviour
{
    public float speed = 3f;
    public float acceleration = 1f;
    public Rigidbody rb;
    public DirectionProvider provider;

    void FixedUpdate()
    {
        Vector3 direction = provider.GetDirection();
        rb.AddForce(direction * acceleration, ForceMode.Acceleration);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, speed);
    }
}
