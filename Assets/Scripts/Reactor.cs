using Unity.FPS.Gameplay;
using UnityEngine;
using UnityEngine.Events;

public class Reactor : MonoBehaviour {
    public string deathCause = "Fell into the reactor";
    public UnityEvent onPlayerFell;

    bool playerGone;

    private void OnTriggerEnter(Collider other) {
        FishInShip fish = other.GetComponentInParent<FishInShip>();

        if (fish != null) {
            Consume(fish);
            return;
        }
        if (playerGone) return;

        PlayerCharacterController player = other.GetComponentInParent<PlayerCharacterController>();

        if (player == null) return;

        playerGone = true;
        KillPlayer();
    }

    public void Consume(FishInShip fish) {
        if (fish == null) return;

        if (fish.definition != null && FuelSystem.instance != null) {
            FuelSystem.instance.AddFuel(fish.definition.fuelValue);
        }

        Destroy(fish.gameObject);
    }

    void KillPlayer() {
        onPlayerFell.Invoke();

        DeathTracker tracker = FindFirstObjectByType<DeathTracker>();
        if (tracker != null) tracker.Die(deathCause);
    }
}
