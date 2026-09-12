using Unity.FPS.Gameplay;
using Unity.FPS.UI;
using UnityEngine;

public class TalkToBot : Interactable
{
    public FishingGameDialogue dialogue;
    public PlayerCharacterController playerController;

    public override void Interact()
    {
        InteractWithObject.InteractionLocked = true;
        playerController.enabled = false;
        Cursor.visible = true;
        InGameMenuManager.ControllingCursor = false;
        Cursor.lockState = CursorLockMode.Confined;
        dialogue.gameObject.SetActive(true);
    }
}
