using UnityEngine;

public class LookByRB : MonoBehaviour
{
    public Rigidbody rb;
    public float rotationSpeed = 5f;
    void Update()
    {
        Vector3 velocity = rb.linearVelocity;
        if (velocity != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
