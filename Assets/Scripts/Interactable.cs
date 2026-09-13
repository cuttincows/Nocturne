using UnityEngine;

public class Interactable : MonoBehaviour
{
    public bool CanBeInteractedWith = true;

    public void Start()
    {
        InteractWithObject.allInteractables.Add(this);
    }

    public void OnDestroy()
    {
        InteractWithObject.allInteractables.Remove(this);
    }

    public virtual void Interact()
    {

    }
}
