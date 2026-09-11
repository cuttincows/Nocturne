using UnityEngine;
using UnityEngine.InputSystem;

public class Dumper : MonoBehaviour
{
    public SpearTip tip;
    public Spear spear;
    private void Update()
    {
        // TODO: change this to be controller friendly
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame && spear.CurrentState == SpearState.Held)
        {
            TryDump();
        }
    }

    private void TryDump()
    {
        // Implement the logic for dumping items here
        if (tip.item.TryGetComponent(out Dumpable dumpable) && dumpable.canDump)
        {
            FuelSystem.instance.AddFuel(dumpable.fuel);
            dumpable.Kill();
        }
    }
}
