using UnityEngine;
using UnityEngine.InputSystem;

public class Dumper : MonoBehaviour
{
    public SpearTip tip;
    private void Update()
    {
        // TODO: change this to be controller friendly
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            Dump();
        }
    }

    private void Dump()
    {
        // Implement the logic for dumping items here

    }
}
