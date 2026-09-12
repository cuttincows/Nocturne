using UnityEngine;

public class ReactorHatch : MonoBehaviour {
    public Lever lever;
    public Animator animator;
    public string boolParameter = "IsOpen";
    public Collider blocker;

    public bool IsOpen {
        get {
            if (lever == null) {
                return false;
            }

            return lever.isOn;
        }
    }

    private void Start() {
        if (lever != null) {
            lever.OnToggled += OnLeverToggled;
        }

        Apply(IsOpen);
    }

    private void OnDestroy() {
        if (lever != null) {
            lever.OnToggled -= OnLeverToggled;
        }
    }

    private void OnLeverToggled(bool on) {
        Apply(on);
    }

    private void Apply(bool open) {
        if (animator != null) {
            animator.SetBool(boolParameter, open);
        }

        if (blocker != null) {
            blocker.enabled = !open;
        }
    }

    public void EnableBlocker() {
        if (blocker != null) {
            blocker.enabled = true;
        }
    }

    public void DisableBlocker() {
        if (blocker != null) {
            blocker.enabled = false;
        }
    }
}