using UnityEngine;

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
