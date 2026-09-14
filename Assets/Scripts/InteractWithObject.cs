using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractWithObject : MonoBehaviour
{
    public GameObject interactPrompt;
    public GameObject player;

    [Header("Drag the player camera here. Interactables outside this cone are ignored.")]
    public Transform lookFrom;
    public float maxLookAngle = 50f;

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
        closestInteractable = null;

        Transform eye = lookFrom != null ? lookFrom : player.transform;

        foreach (Interactable interactable in allInteractables)
        {
            if (!interactable.CanBeInteractedWith)
            {
                continue;
            }

            float dist = Vector3.Distance(player.transform.position, interactable.transform.position);
            if (dist > maxInteractDist)
            {
                continue;
            }

            Vector3 toTarget = interactable.transform.position - eye.position;
            if (Vector3.Angle(eye.forward, toTarget) > maxLookAngle)
            {
                continue;
            }

            if (dist < closestDist)
            {
                closestDist = dist;
                closestInteractable = interactable;
            }
        }

        bool closeEnough = closestInteractable != null;
        interactPrompt.SetActive(closeEnough);
        if (closeEnough && Keyboard.current.eKey.wasPressedThisFrame)
        {
            closestInteractable.Interact();
        }
    }
}
