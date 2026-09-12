using UnityEngine;

public class Reactor : MonoBehaviour {
    public bool CanConsume(FishInShip f) => f != null && f.State == FishState.Raw;

    public bool TryConsume(FishInShip f) {
        if (!CanConsume(f)) return false;
        if (FuelSystem.instance != null)
            FuelSystem.instance.AddFuel(f.definition.fuelValue);
        Destroy(f.gameObject);
        return true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out FishInShip f)) TryConsume(f);
    }
}
