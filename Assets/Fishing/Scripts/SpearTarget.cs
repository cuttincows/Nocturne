using UnityEngine;
using UnityEngine.InputSystem;

public class SpearTarget : MonoBehaviour
{
    public Spear Spear;
    public Camera Camera;
    public LayerMask TargetLayerMask;

    private void Update()
    {
        Ray ray = Camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, TargetLayerMask))
        {
            if (hit.collider != null)
            {
                transform.position = hit.point;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Camera != null)
        {
            Ray ray = Camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * 100f);
        }
    }
}
