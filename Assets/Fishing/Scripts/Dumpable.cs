using UnityEngine;

// For dumping fish into the ship.
public class Dumpable : MonoBehaviour
{
    public bool canDump = true;
    public int fuel;

    public void Kill()
    {
        Destroy(gameObject);
    }
}
