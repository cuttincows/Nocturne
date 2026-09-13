using System;
using UnityEngine;

public class FishInShip : MonoBehaviour {

    public FishDefinition definition;
    public SpriteRenderer spriteRenderer;
    public Rigidbody rb;
    public Collider col;

    public FishState State { get; private set; } = FishState.Raw;
    public Stove Pan { get; private set; }

    public Action<FishInShip> OnStateChanged;

    float heat;

    public bool CanCarry {
        get {
            if (State != FishState.Raw) {
                return false;
            }

            return Pan == null;
        }
    }

    public float CookProgress {
        get {
            if (definition == null) {
                return 1f;
            }

            if (definition.cookTime <= 0f) {
                return 1f;
            }

            return Mathf.Clamp01(heat / definition.cookTime);
        }
    }

    private void OnEnable() {
        FishRegistry.instance.Register(this);
        ApplySprite();
    }

    private void OnDisable() {
        FishRegistry reg = FishRegistry.Existing;

        if (reg != null) {
            reg.Unregister(this);
        }
    }

    public void PlaceInPan(Stove stove, Transform anchor) {
        Pan = stove;

        if (rb != null) {
            rb.isKinematic = true;
        }

        if (col != null) {
            col.enabled = false;
        }

        transform.SetParent(anchor, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void AddHeat(float amount) {
        if (State != FishState.Raw) {
            return;
        }

        if (definition == null) {
            return;
        }

        heat += amount;

        if (heat >= definition.cookTime) {
            SetState(FishState.Cooked);
        }
    }

    private void SetState(FishState newState) {
        if (State == newState) {
            return;
        }

        State = newState;
        ApplySprite();

        if (OnStateChanged != null) {
            OnStateChanged.Invoke(this);
        }
    }

    private void ApplySprite() {
        if (spriteRenderer == null) {
            return;
        }

        if (definition == null) {
            return;
        }

        if (State == FishState.Cooked) {
            spriteRenderer.sprite = definition.cookedSprite;
        } else {
            spriteRenderer.sprite = definition.rawSprite;
        }
    }

    public void Setup(FishDefinition def) {
        definition = def;
        ApplySprite();
    }
}