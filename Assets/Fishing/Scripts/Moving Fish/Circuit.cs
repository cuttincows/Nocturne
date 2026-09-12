using System.Collections.Generic;
using UnityEngine;

public class Circuit : DirectionProvider
{
    List<Vector3> positions = new();
    int currIndex = 0;
    Vector3 currTarget => positions == null ? transform.position : positions[currIndex];

    public override bool Can_Perform => enabled;

    public SpawnBoxReference spawnBoxRef;
    public int maxPositions = 6;
    public float doneDistance = 1f;

    public override Vector3 GetDirection()
    {
        return (currTarget - transform.position).normalized;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, currTarget) < doneDistance)
        {
            currIndex = (currIndex + 1) % positions.Count;
        }
    }

    private void Start()
    {
        GeneratePositions();
    }

    private void GeneratePositions()
    {
        int positionsCount = Random.Range(3, maxPositions + 1);
        positions = new List<Vector3>(positionsCount);
        positions.Add(transform.position);
        for (int i = 1; i < positionsCount; i++)
        {
            positions.Add(spawnBoxRef.SpawnBox.GetRandomPosition());
        }
    }

    public void OnDrawGizmosSelected()
    {
        if (positions == null) return;
        Gizmos.color = Color.green;
        foreach (Vector3 pos in positions)
        {
            Gizmos.DrawSphere(pos, 0.1f);
        }
    }
}
