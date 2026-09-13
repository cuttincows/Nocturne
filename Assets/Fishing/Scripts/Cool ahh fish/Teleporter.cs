using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public SpawnBoxReference spawnBox;
    public GameObject teleportVisual;
    public Rigidbody2D rb;
    public Stunnable stunner;
    public float cooldown = 2f;

    private void Start()
    {
        teleportVisual.transform.parent = null;
        RandomizeTeleport();
    }

    private void RandomizeTeleport()
    {
        teleportVisual.transform.position = spawnBox.SpawnBox.GetRandomPosition();
    }

    private void OnDestroy()
    {
        Destroy(teleportVisual);
    }

    public void Teleport()
    {
        if (stunner.IsStunned) return;
        rb.MovePosition(teleportVisual.transform.position);
        rb.linearVelocity = Vector3.zero;
        RandomizeTeleport();
        stunner.Stun(cooldown);
    }
}
