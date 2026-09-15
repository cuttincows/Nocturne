using UnityEngine;

public class FishingRig : MonoBehaviour {
    public static FishingRig instance;

    public GameObject fishingCamera;
    public GameObject spearRig;
    public GameObject fishingAudio;
    public GameObject backHint;

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

        if (backHint != null)
        {
            backHint.SetActive(on);
        }

        if (fishingCamera != null) {
            fishingCamera.SetActive(on);
        }

        if (spearRig != null) {
            spearRig.SetActive(on);
        }

        if (fishingAudio != null) {
            fishingAudio.SetActive(on);

            FishAudio audio = fishingAudio.GetComponent<FishAudio>();

            if (audio != null) {
                audio.Mute(!on);
            }
        }
    }
}