using System;
using UnityEngine;

public class Lever : Interactable {
    public Animator animator;
    public string boolParameter = "IsOn";
    public bool isOn;

    public Action<bool> OnToggled;

    private void Awake() {
        Apply();
    }

    public override void Interact() {
        isOn = !isOn;
        Apply();
        OnToggled?.Invoke(isOn);
    }

    private void Apply() {
        if (animator != null) animator.SetBool(boolParameter, isOn);
    }
}
