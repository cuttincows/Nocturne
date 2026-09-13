using UnityEngine;

// For dumping fish into the ship.
public class Dumpable : MonoBehaviour {
    public bool canDump = true;
    public FishDefinition definition;

    public void Kill() {
        Destroy(gameObject);
    }
}