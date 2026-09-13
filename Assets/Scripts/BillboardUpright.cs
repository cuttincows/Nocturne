using UnityEngine;

public class BillboardUpright : MonoBehaviour {
    public float maxTilt = 25f;
    public bool randomFlip = true;

    private Camera target;
    private float tilt;

    private void Awake() {
        tilt = Random.Range(-maxTilt, maxTilt);

        if (randomFlip) {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            if (sr != null) {
                sr.flipX = Random.value < 0.5f;
            }
        }
    }

    private void LateUpdate() {
        // Re-resolve whenever the cached camera is gone or switched off, so fish
        // spawned during fishing mode don't stay locked to the fishing camera.
        if (target == null || !target.isActiveAndEnabled) {
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

        transform.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0f, 0f, tilt);
    }
}
