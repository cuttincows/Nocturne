using UnityEngine;
using UnityEngine.Events;

public class CollisionEvent : MonoBehaviour
{
    public Rigidbody rb;
    public float minSpeed;
    public UnityEvent onCollide;
    public string hitTag;
    private void OnCollisionEnter(Collision collision)
    {
        if (rb.linearVelocity.magnitude > minSpeed && collision.collider.CompareTag(hitTag)) 
            onCollide.Invoke();
    }
}
