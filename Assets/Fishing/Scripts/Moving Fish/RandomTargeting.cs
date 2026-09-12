using UnityEngine;

public class RandomTargeting : DirectionProvider
{
    public SpawnBoxReference spawnBox;
    private Vector3 target;
    private float doneDistance = 1f;

    public override bool Can_Perform => enabled;

    void Start()
    {
        RandomizeTarget();
    }
    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, target) < doneDistance)
        {
            RandomizeTarget();
        }
    }
    void RandomizeTarget()
    {
        target = spawnBox.SpawnBox.GetRandomPosition();
    }

    public override Vector3 GetDirection()
    {
        return (target - transform.position).normalized;
    }
}
