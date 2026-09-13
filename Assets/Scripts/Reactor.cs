using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.Events;

public class Reactor : MonoBehaviour {
    public ReactorHatch hatch;
    public Transform plane;

    public bool matchPlaneSize = true;
    public Vector2 holeSize = new Vector2(2f, 2f);

    public float fallDistance = 1.5f;
    public float playerFallSpeed = 5f;

    public string deathCause = "Fell into the reactor";
    public UnityEvent onPlayerFell;

    PlayerCharacterController player;
    CharacterController body;
    bool playerFalling;
    bool playerGone;

    private void Start() {
        player = FindFirstObjectByType<PlayerCharacterController>();
        if (player != null) body = player.GetComponent<CharacterController>();
    }

    private void Update() {
        if (plane == null) return;

        bool open = hatch != null && hatch.IsOpen;
        float killY = plane.position.y - fallDistance;

        UpdateFish(open, killY);
        UpdatePlayer(open, killY);
    }

    void UpdateFish(bool open, float killY) {
        FishRegistry reg = FishRegistry.Existing;
        if (reg == null) return;

        for (int i = reg.All.Count - 1; i >= 0; i--) {
            FishInShip fish = reg.All[i];

            if (fish == null) continue;
            if (!OverHole(fish.transform.position)) continue;

            if (fish.transform.position.y <= killY) {
                Consume(fish);
                continue;
            }

            if (open && fish.col != null) fish.col.enabled = false;
        }
    }

    void UpdatePlayer(bool open, float killY) {
        if (playerGone) return;
        if (player == null) return;

        if (playerFalling) {
            player.transform.position += Vector3.down * playerFallSpeed * Time.deltaTime;

            if (player.transform.position.y <= killY) {
                playerGone = true;
                onPlayerFell.Invoke();

                DeathTracker tracker = FindFirstObjectByType<DeathTracker>();
                if (tracker != null) tracker.Die(deathCause);
            }
            return;
        }

        if (!open) return;
        if (!OverHole(player.transform.position)) return;

        playerFalling = true;
        player.enabled = false;

        if (body != null) body.enabled = false;
    }

    public Vector2 GetHoleSize() {
        if (!matchPlaneSize) return holeSize;
        if (plane == null) return holeSize;

        MeshFilter filter = plane.GetComponent<MeshFilter>();

        if (filter == null || filter.sharedMesh == null) return holeSize;

        Vector3 size = filter.sharedMesh.bounds.size;
        Vector3 scale = plane.lossyScale;

        return new Vector2(size.x * Mathf.Abs(scale.x), size.z * Mathf.Abs(scale.z));
    }

    bool OverHole(Vector3 pos) {
        Vector2 box = GetHoleSize();
        Vector3 offset = pos - plane.position;

        float x = Vector3.Dot(offset, plane.right);
        float z = Vector3.Dot(offset, plane.forward);

        if (Mathf.Abs(x) > box.x * 0.5f) return false;

        return Mathf.Abs(z) <= box.y * 0.5f;
    }

    public void Consume(FishInShip fish) {
        if (fish == null) return;

        if (fish.definition != null && FuelSystem.instance != null) {
            FuelSystem.instance.AddFuel(fish.definition.fuelValue);
        }

        Destroy(fish.gameObject);
    }

    private void OnDrawGizmosSelected() {
        if (plane == null) return;

        Vector2 box = GetHoleSize();

        Gizmos.color = Color.cyan;
        Gizmos.matrix = Matrix4x4.TRS(plane.position, plane.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(box.x, 0.02f, box.y));
        Gizmos.DrawWireCube(new Vector3(0f, -fallDistance, 0f), new Vector3(box.x, 0.02f, box.y));
    }
}
