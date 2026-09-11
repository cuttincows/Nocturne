using UnityEngine;

public class RandomTargeting : DirectionProvider
{
    public SpawnBox spawnBox;
    private Vector3 target;
    private float doneDistance = 1f;
    void Start()
    {
        ChangeDirection();
    }
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, target) < doneDistance)
        {
            ChangeDirection();
        }
    }
    void ChangeDirection()
    {
        target = spawnBox.GetRandomPosition();
    }

    public override Vector3 GetDirection()
    {
        return (target - transform.position).normalized;
    }
}
