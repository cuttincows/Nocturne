using Unity.FPS.Gameplay;
using Unity.FPS.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class FishingStation : Interactable {
    public PlayerCharacterController playerController;

    private bool fishing;
    private int stoppedFrame = -1;
    private int startedFrame = -1;

    public override void Interact() {
        if (fishing) {
            return;
        }

        if (Time.frameCount == stoppedFrame) {
            return;
        }

        if (FishingRig.instance == null) {
            Debug.LogError("FishingStation: FishingRig.instance is null", this);
            return;
        }

        if (playerController == null) {
            Debug.LogError("FishingStation: playerController is not assigned", this);
            return;
        }

        fishing = true;
        startedFrame = Time.frameCount;

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

        if (Time.frameCount == startedFrame) {
            return;
        }

        Keyboard kb = Keyboard.current;

        if (kb == null) {
            return;
        }

        if (!kb.eKey.wasPressedThisFrame) {
            return;
        }

        StopFishing();
    }

    private void StopFishing() {
        fishing = false;
        stoppedFrame = Time.frameCount;

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