using UnityEngine;
using System.Collections.Generic;

public class MoveTowards : MonoBehaviour
{
    public float speed = 3f;
    public float acceleration = 1f;
    public Rigidbody rb;
    public List<DirectionProvider> provider;

    void FixedUpdate()
    {
        Vector3 direction = Vector3.zero;
        foreach (DirectionProvider p in provider)
        {
            direction = p.GetDirection();
            if (direction != Vector3.zero) break;
        }
        rb.AddForce(direction * acceleration, ForceMode.Acceleration);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, speed);
    }
}
