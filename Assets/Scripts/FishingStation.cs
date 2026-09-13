using Unity.FPS.Gameplay;
using Unity.FPS.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingStation : Interactable {
    public PlayerCharacterController playerController;

    private bool fishing;

    public override void Interact() {
        if (fishing) {
            return;
        }

        if (FishingRig.instance == null) {
            return;
        }

        fishing = true;

        InteractWithObject.InteractionLocked = true;
        playerController.enabled = false;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        InGameMenuManager.ControllingCursor = false;

        SetPlayerCameraActive(false);
        FishingRig.instance.Enter();
    }

    private void Update() {
        if (!fishing) {
            return;
        }

        Keyboard kb = Keyboard.current;

        if (kb == null) {
            return;
        }

        if (!kb.escapeKey.wasPressedThisFrame) {
            return;
        }

        StopFishing();
    }

    private void StopFishing() {
        fishing = false;

        if (FishingRig.instance != null) {
            FishingRig.instance.Exit();
        }

        SetPlayerCameraActive(true);

        InteractWithObject.InteractionLocked = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InGameMenuManager.ControllingCursor = true;
        playerController.enabled = true;
    }

    private void SetPlayerCameraActive(bool on) {
        if (playerController == null) {
            return;
        }

        if (playerController.PlayerCamera == null) {
            return;
        }

        playerController.PlayerCamera.gameObject.SetActive(on);
    }
}