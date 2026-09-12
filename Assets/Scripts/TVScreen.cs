using System.Collections.Generic;
using Unity.FPS.Gameplay;
using Unity.FPS.UI;
using UnityEngine;

public class TVScreen : Interactable
{
    public List<TextAsset> dialogueScripts;


    public FishingGameDialogue dialogue;
    public LanguageInterpreter interpreter;
    public PlayerCharacterController playerController;

    override public void Interact()
    {
        InteractWithObject.InteractionLocked = true;
        playerController.enabled = false;
        Cursor.visible = true;
        InGameMenuManager.ControllingCursor = false;
        Cursor.lockState = CursorLockMode.Confined;

        interpreter.scriptFiles = dialogueScripts;
        interpreter.Initialize();

        dialogue.gameObject.SetActive(true);
    }
}
