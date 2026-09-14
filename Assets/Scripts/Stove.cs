using UnityEngine;

public class Stove : Interactable {
    public Transform panAnchor;
    public float cookTime = 2f;

    public FishInShip CurrentFish { get; private set; }

    float cooked;

    public bool IsEmpty => CurrentFish == null;

    private void Update() {
        if (CurrentFish != null && CurrentFish.Pan != this) {
            CurrentFish = null;
            cooked = 0f;
        }

        CanBeInteractedWith = CanPlace();

        if (CurrentFish == null) return;

        if (panAnchor != null) {
            CurrentFish.transform.position = panAnchor.position;
        }

        if (CurrentFish.State != FishState.Raw) return;

        cooked += Time.deltaTime;

        if (cooked >= cookTime) CurrentFish.SetCooked();
    }

    bool CanPlace() {
        if (CurrentFish != null) return false;
        if (PlayerCarry.instance == null) return false;

        FishInShip held = PlayerCarry.instance.Held;

        if (held == null) return false;

        return held.State == FishState.Raw;
    }

    public override void Interact() {
        if (!CanPlace()) return;

        FishInShip fish = PlayerCarry.instance.Release();

        if (fish == null) return;

        Transform anchor = panAnchor;
        if (anchor == null) anchor = transform;

        CurrentFish = fish;
        cooked = 0f;
        fish.PlaceInPan(this, anchor);
    }
}
