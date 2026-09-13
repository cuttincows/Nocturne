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

    private void TryDump() {
        if (tip.item == null) return;
        if (!tip.item.TryGetComponent(out Dumpable dumpable)) return;
        if (!dumpable.canDump) return;

        if (ShipHold.instance != null) {
            ShipHold.instance.Receive(dumpable.definition);
        }

        tip.item = null;
        dumpable.Kill();
    }
}
