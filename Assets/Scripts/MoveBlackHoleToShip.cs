using System;
using System.Security.Cryptography;
using UnityEngine;

[ExecuteInEditMode]
public class MoveBlackHoleToShip : MonoBehaviour
{
    public GameObject ObjectToTween;
    public Transform TweenToTr;

    public Ease.DelegateType delType;
    public Ease.EaseType easeType;

    private float prevFuelValue;
    public float fuelEaseCatchupSpeed = 0.1f;

    [Header("Skybox Blur")]
    public float MinSkyboxBlur = 2f;
    public float MaxSkyboxBlur = 60f;

    public Material skyboxMaterial;

    [Header("Sting")]
    public AudioClip stingClip;
    public AudioSource blackHoleSource;

    [Range(0,1)]
    public float TestEditorT = 0f;

    public float minBlackHolePosOffet = -.4f;
    public float maxBlackHolePosOffste = 0.9f;

    public Transform BlackHoleSpawnPoint;

    void Start()
    {
        if (!Application.isPlaying)
        {
            return;
        }
        prevFuelValue = 1f - FuelSystem.instance.GetRemainingFuelPercent();

    }

    private bool hasPlayedSting = false;
    void Update()
    {
        float easedT = TestEditorT;
        if (Application.isPlaying) {
            easedT = Mathf.Lerp(
                prevFuelValue,
                1f - FuelSystem.instance.GetRemainingFuelPercent(),
                Time.deltaTime * fuelEaseCatchupSpeed
            );
        }
        if (Application.isPlaying && !hasPlayedSting && easedT >= 1)
        {
            blackHoleSource.clip = stingClip;
            blackHoleSource.loop = false;
            blackHoleSource.Play();
            hasPlayedSting = true;
        }
        prevFuelValue = easedT;
        TweenByToWith(easedT, delType, easeType);
    }

    private void TweenByToWith(float t, Ease.DelegateType delType, Ease.EaseType easeType)
    {
        t = Ease.GetDelegate(delType, easeType)(t);

        // What actually moves the object
        ObjectToTween.transform.position = Vector3.Lerp(BlackHoleSpawnPoint.position, TweenToTr.transform.position, t);
        if (skyboxMaterial != null)
        {
            skyboxMaterial.SetFloat("_BlurAmount", Mathf.Lerp(MinSkyboxBlur, MaxSkyboxBlur, t));
        }
    }
}
