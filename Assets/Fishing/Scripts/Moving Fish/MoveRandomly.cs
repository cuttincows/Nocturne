using UnityEngine;

public class MoveRandomly : MonoBehaviour
{
    public float speed = 1f;
    public float acceleration = 1f;
    public Rigidbody rb;
    public SpawnBox spawnBox;
    private Vector3 target;
    private float doneDistance = 1f;
    void Start()
    {
        ChangeDirection();
    }
    void FixedUpdate()
    {
        Vector3 direction = (target - transform.position).normalized;
        rb.AddForce(direction * acceleration, ForceMode.Acceleration);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, speed);
        if (Vector3.Distance(transform.position, target) < doneDistance)
        {
            ChangeDirection();
        }
    }
    void ChangeDirection()
    {
        target = spawnBox.GetRandomPosition();
    }
}
