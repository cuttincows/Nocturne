using System;
using UnityEngine;

public class FishInShip : MonoBehaviour {

    public FishDefinition definition;
    public SpriteRenderer spriteRenderer;
    public Rigidbody rb;
    public Collider col;
    public float visualYOffset = 0f;

    public FishState State { get; private set; } = FishState.Raw;
    public Stove Pan { get; private set; }

    public Action<FishInShip> OnStateChanged;

    float heat;

    public bool CanCarry {
        get {
            if (State == FishState.Cooked) {
                return true;
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

        transform.position = anchor.position;
        transform.rotation = anchor.rotation;
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

        AlignVisual();
    }

    // The sprite pivot is centred, so lift the visual until its bottom edge
    // sits level with the bottom of the collider instead of sinking into the floor.
    private void AlignVisual() {
        if (spriteRenderer == null || spriteRenderer.sprite == null) {
            return;
        }

        float bottom = 0f;

        if (col is CapsuleCollider capsule) {
            bottom = capsule.center.y - (capsule.height * 0.5f);
        } else if (col is BoxCollider box) {
            bottom = box.center.y - (box.size.y * 0.5f);
        } else if (col is SphereCollider sphere) {
            bottom = sphere.center.y - sphere.radius;
        }

        Vector3 local = spriteRenderer.transform.localPosition;
        local.y = spriteRenderer.sprite.bounds.extents.y + bottom + visualYOffset;
        spriteRenderer.transform.localPosition = local;
    }

    public void RemoveFromPan() {
        Pan = null;
    }

    public void SetCooked() {
        SetState(FishState.Cooked);
    }

    public void Setup(FishDefinition def) {
        definition = def;
        ApplySprite();
    }
}