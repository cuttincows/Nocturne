using UnityEngine;

public class ShipHold : MonoBehaviour {
    public static ShipHold instance;

    public FishInShip fishPrefab;
    public Transform exitPoint;
    public float exitForce = 1f;
    public float spawnRadius = 0.5f;

    private void Awake() {
        instance = this;
    }

    public FishInShip Receive(FishDefinition definition) {
        if (fishPrefab == null) {
            return null;
        }

        if (definition == null) {
            return null;
        }

        Transform spawn = exitPoint;

        if (spawn == null) {
            spawn = transform;
        }
        Vector2 circle = Random.insideUnitCircle * spawnRadius;
        Vector3 offset = new Vector3(circle.x, 0f, circle.y);
        FishInShip fish = Instantiate(fishPrefab, spawn.position + offset, Quaternion.identity);
        fish.Setup(definition);

        if (fish.rb != null) {
            fish.rb.isKinematic = false;
            fish.rb.useGravity = true;
            fish.rb.AddForce(spawn.forward * exitForce, ForceMode.Impulse);
        }

        return fish;
    }
}