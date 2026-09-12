using System.Collections.Generic;
using NUnit.Framework;
using Unity.FPS.Gameplay;
using Unity.FPS.UI;
using UnityEngine;

public class TalkToBot : Interactable
{
    public FishingGameDialogue dialogue;
    public LanguageInterpreter interpreter;
    public PlayerCharacterController playerController;

    public List<TextAsset> dialogueScripts;

    public override void Interact()
    {
        if (dialogueScripts.Count == 0)
        {
            return;
        }

        InteractWithObject.InteractionLocked = true;
        playerController.enabled = false;
        Cursor.visible = true;
        InGameMenuManager.ControllingCursor = false;
        Cursor.lockState = CursorLockMode.Confined;

        interpreter.scriptFiles.Clear();
        interpreter.scriptFiles.Add(dialogueScripts[0]);
        interpreter.Initialize();

        dialogue.gameObject.SetActive(true);
        if (dialogueScripts.Count > 0)
        {
            dialogueScripts.RemoveAt(0);
        }

        if (dialogueScripts.Count == 0)
        {
            CanBeInteractedWith = false;
        }
    }
}
