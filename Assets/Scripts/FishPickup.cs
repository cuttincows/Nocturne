using UnityEngine;

public class FishPickup : Interactable {
    public FishInShip fish;

    private void Awake() {
        if (fish == null) fish = GetComponent<FishInShip>();
    }

    private void Update() {
        if (fish == null) {
            CanBeInteractedWith = false;
            return;
        }

        if (PlayerCarry.instance != null && PlayerCarry.instance.Held == fish) {
            CanBeInteractedWith = false;
            return;
        }
        CanBeInteractedWith = fish.CanCarry;
    }

    public override void Interact() {
        if (PlayerCarry.instance == null) return;

        PlayerCarry.instance.TryPickUp(fish);
    }
}
