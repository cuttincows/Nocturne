using System.Collections.Generic;
using UnityEngine;

public class FishRegistry : MonoBehaviour {
    static FishRegistry _instance;

    public static FishRegistry instance {
        get {
            if (_instance == null) {
                _instance = FindFirstObjectByType<FishRegistry>();
                if (_instance == null)
                    _instance = new GameObject(nameof(FishRegistry)).AddComponent<FishRegistry>();
            }
            return _instance;
        }
    }

    public static FishRegistry Existing => _instance;

    readonly List<FishInShip> fish = new();
    public IReadOnlyList<FishInShip> All => fish;
    public int Count => fish.Count;

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }

    private void OnDestroy() {
        if (_instance == this) _instance = null;
    }

    public void Register(FishInShip f) {
        if (f != null && !fish.Contains(f)) fish.Add(f);
    }

    public void Unregister(FishInShip f) {
        fish.Remove(f);
    }

    public int CountOf(FishState state) {
        int n = 0;
        foreach (var f in fish)
            if (f != null && f.State == state) n++;
        return n;
    }
}
