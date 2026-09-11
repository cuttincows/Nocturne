using UnityEngine;

/// <summary>
/// Doesnt need to be assigned at all. Just stores information about the spawner and the power of the fish.
/// </summary>
public class Spawnable : MonoBehaviour
{
    private Spawning spawning;
    [HideInInspector]
    public float power;

    public void Spawn(Spawning spawner, float power)
    {
        spawning = spawner;
        this.power = power;
    }

    public void OnDestroy()
    {
        spawning.ReportDeath(this);
    }
}
