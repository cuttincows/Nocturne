using UnityEngine;

// For dumping fish into the ship.
[RequireComponent(typeof(Destroyable))]
public class Dumpable : MonoBehaviour {
    public bool canDump = true;
    public FishDefinition definition;

    public void Kill() {
        GetComponent<Destroyable>().Destroy();
    }
}