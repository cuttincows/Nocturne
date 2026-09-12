using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractWithObject : MonoBehaviour
{
    public GameObject interactPrompt;
    public GameObject player;

    [HideInInspector]
    public Interactable closestInteractable;
    public float closestDist = Mathf.Infinity;

    public float maxInteractDist = 2f;

    public static List<Interactable> allInteractables = new();

    // Toggle this when you are locked in an interaction state
    public static bool InteractionLocked = false;

    private void Awake()
    {
        InteractionLocked = false;
        allInteractables.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        // Early return if we can't interact with anything
        if (InteractionLocked) 
        {
            interactPrompt.SetActive(false);
            return;
        }

        // Reset closest 
        closestDist = Mathf.Infinity;
        foreach (Interactable interactable in allInteractables)
        {
            if (!interactable.CanBeInteractedWith)
            {
                continue;
            }

            float dist = Vector3.Distance(player.transform.position, interactable.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestInteractable = interactable;
            }
        }

        bool closeEnough = closestDist < maxInteractDist;
        interactPrompt.SetActive(closeEnough);
        if (closeEnough && Keyboard.current.eKey.wasPressedThisFrame)
        {
            closestInteractable.Interact();
        }
    }
}
