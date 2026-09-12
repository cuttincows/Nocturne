using UnityEngine;

public class Death : DirectionProvider
{
    bool dead = false;
    public float expirationTime = 10f;
    public override bool Can_Perform => dead;
    public void Kill()
    {
        dead = true;
    }

    private void Update()
    {
        if (dead)
        {
            expirationTime -= Time.deltaTime;
            if (expirationTime < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public override Vector3 GetDirection()
    {
        return Vector3.zero;
    }
}
