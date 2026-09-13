using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarry : MonoBehaviour {
    public static PlayerCarry instance;

    public Transform holdPoint;
    public Vector3 holdRotation = new Vector3(0f, 155f, 25f);
    public float followSpeed = 12f;
    public float turnSpeed = 14f;
    public float throwForce = 7f;
    public float throwLift = 2f;

    public FishInShip Held { get; private set; }

    private void Awake() {
        instance = this;
    }
    public bool TryPickUp(FishInShip fish) {
        if (Held != null) return false;
        if (fish == null) return false;
        if (holdPoint == null) return false;
        if (!fish.CanCarry) return false;

        Held = fish;

        if (fish.rb != null) {
            fish.rb.linearVelocity = Vector3.zero;
            fish.rb.angularVelocity = Vector3.zero;
            fish.rb.isKinematic = true;
        }

        if (fish.col != null) fish.col.enabled = false;

        BillboardUpright board = fish.GetComponentInChildren<BillboardUpright>();
        if (board != null) {
            board.enabled = false;
            board.transform.localRotation = Quaternion.identity;
        }

        return true;
    }

    public FishInShip Release() {
        FishInShip fish = Held;

        if (fish == null) return null;

        Held = null;

        BillboardUpright board = fish.GetComponentInChildren<BillboardUpright>();
        if (board != null) board.enabled = true;

        if (fish.col != null) fish.col.enabled = true;

        if (fish.rb != null) {
            fish.rb.isKinematic = false;
        }
        return fish;
    }

    public void Throw() {
        Transform aim = holdPoint;
        FishInShip fish = Release();

        if (fish == null) return;
        if (fish.rb == null) return;

        Vector3 force = (aim.forward * throwForce) + (Vector3.up * throwLift);
        fish.rb.AddForce(force, ForceMode.Impulse);
    }

    private void Update() {
        if (Held == null) return;
        if (InteractWithObject.InteractionLocked) return;

        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame) Throw();
    }

    private void LateUpdate() {
        if (Held == null) return;
        if (holdPoint == null) return;

        Transform t = Held.transform;
        Quaternion target = holdPoint.rotation * Quaternion.Euler(holdRotation);

        t.position = Vector3.Lerp(t.position, holdPoint.position, followSpeed * Time.deltaTime);
        t.rotation = Quaternion.Slerp(t.rotation, target, turnSpeed * Time.deltaTime);
    }
}
