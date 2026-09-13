using UnityEngine;

public class ReactorHatch : MonoBehaviour {
    public Lever lever;
    public Animator animator;
    public string boolParameter = "IsOpen";
    public Collider blocker;
    public Collider[] extraBlockers;
    public float openDuration = 5f;

    bool open;
    float closeAt;

    public bool IsOpen => open;

    private void Start() {
        if (lever != null) lever.OnToggled += OnLeverToggled;

        SetOpen(false);
    }

    private void OnDestroy() {
        if (lever != null) lever.OnToggled -= OnLeverToggled;
    }

    private void OnLeverToggled(bool on) {
        if (!on) {
            SetOpen(false);
            return;
        }

        SetOpen(true);
        closeAt = Time.time + openDuration;
    }

    private void Update() {
        if (!open) return;
        if (Time.time < closeAt) return;

        if (lever != null) {
            lever.SetOn(false);
        }
        else {
            SetOpen(false);
        }
    }

    private void SetOpen(bool value) {
        open = value;

        if (animator != null) animator.SetBool(boolParameter, value);

        SetBlockers(!value);

        if (value) WakeFish();
    }

    void SetBlockers(bool solid) {
        if (blocker != null) blocker.enabled = solid;

        if (extraBlockers == null) return;

        foreach (Collider col in extraBlockers) {
            if (col == null) continue;

            col.enabled = solid;
        }
    }

    public void EnableBlocker() {
        SetBlockers(true);
    }

    public void DisableBlocker() {
        SetBlockers(false);
    }

    void WakeFish() {
        FishRegistry reg = FishRegistry.Existing;
        if (reg == null) return;

        foreach (FishInShip fish in reg.All) {
            if (fish == null) continue;
            if (fish.rb == null) continue;

            fish.rb.WakeUp();
        }
    }
}
