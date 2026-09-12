using UnityEngine;
using System.Collections.Generic;

public class MoveTowards : MonoBehaviour
{
    public float speed = 3f;
    public float acceleration = 1f;
    public Rigidbody2D rb;
    public List<DirectionProvider> provider;

    void FixedUpdate()
    {
        Vector3 direction = GetDir();
        rb.AddForce(direction * acceleration, ForceMode2D.Force);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, speed);
    }

    private Vector3 GetDir()
    {
        foreach (DirectionProvider p in provider)
        {
            if (p.Can_Perform)
                return p.GetDirection();
        }
        return Vector3.zero;
    }
}
