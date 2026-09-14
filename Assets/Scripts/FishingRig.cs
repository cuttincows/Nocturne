using UnityEngine;

public class FishingRig : MonoBehaviour {
    public static FishingRig instance;

    public GameObject fishingCamera;
    public GameObject spearRig;
    public GameObject fishingAudio;

    public bool IsActive { get; private set; }

    private void Awake() {
        instance = this;
        SetRigActive(false);
    }

    public void Enter() {
        SetRigActive(true);
    }

    public void Exit() {
        SetRigActive(false);
    }

    private void SetRigActive(bool on) {
        IsActive = on;

        if (fishingCamera != null) {
            fishingCamera.SetActive(on);
        }

        if (spearRig != null) {
            spearRig.SetActive(on);
        }

        if (fishingAudio != null) {
            fishingAudio.SetActive(on);
        }
    }
}