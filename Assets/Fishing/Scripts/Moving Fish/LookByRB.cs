using UnityEngine;

public class LookByRB : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer sprite;
    public float rotationSpeed = 5f;

    [Header("Stops fish rotating past vertical and swimming upside down")]
    public bool keepUpright = true;

    bool flipped;
    bool started;

    void Awake()
    {
        if (sprite == null)
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void FixedUpdate()
    {
        Vector2 velocity = rb.linearVelocity;

        if (velocity.sqrMagnitude < 0.0001f)
        {
            return;
        }

        float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        bool flip = false;

        if (keepUpright && (angle > 90f || angle < -90f))
        {
            angle -= 180f;
            flip = true;
        }

        if (sprite != null)
        {
            sprite.flipX = flip;
        }

        float z;

        if (flip != flipped || !started)
        {
            z = angle;
            flipped = flip;
            started = true;
        }
        else
        {
            z = Mathf.LerpAngle(transform.eulerAngles.z, angle, rotationSpeed * Time.fixedDeltaTime);
        }

        transform.rotation = Quaternion.Euler(0f, 0f, z);
    }
}
