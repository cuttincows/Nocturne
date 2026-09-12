using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class SetActiveToInteractable : MonoBehaviour
{
    public GameObject ToMakeActive;
    private Interactable _interactable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _interactable = GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        ToMakeActive.SetActive(_interactable.CanBeInteractedWith);
    }
}
