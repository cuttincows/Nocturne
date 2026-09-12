using UnityEngine;

public class Interactable : MonoBehaviour
{
    public void Start()
    {
        InteractWithObject.allInteractables.Add(this);
    }

    public virtual void Interact()
    {

    }
}
