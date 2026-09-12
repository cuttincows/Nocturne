using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    public Rigidbody2D rb;
    public float minSpeed;
    public UnityEvent onCollide;
    public string hitTag;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.linearVelocity.magnitude > minSpeed && collision.collider.CompareTag(hitTag)) 
            onCollide.Invoke();
    }
}   
