using UnityEngine;

public class BillboardUpright : MonoBehaviour {
    private Camera target;

    private void LateUpdate() {
        if (target == null) {
            target = Camera.main;
        }

        if (target == null) {
            return;
        }

        Vector3 dir = transform.position - target.transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f) {
            return;
        }

        transform.rotation = Quaternion.LookRotation(dir);
    }
}