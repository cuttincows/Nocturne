using System;
using System.Security.Cryptography;
using UnityEngine;

public class TweenObjectTo : MonoBehaviour
{
    public GameObject ObjectToTween;
    public Transform TweenToTr;
    private Vector3 startPos;

    public Ease.DelegateType delType;
    public Ease.EaseType easeType;

    private float prevFuelValue;
    public float fuelEaseCatchupSpeed = 0.1f;

    [Header("Skybox Blur")]
    public float MinSkyboxBlur = 2f;
    public float MaxSkyboxBlur = 60f;

    public Material skyboxMaterial;

    void Start()
    {
        startPos = ObjectToTween.transform.position;
        prevFuelValue = 1f - FuelSystem.instance.GetRemainingFuelPercent();

    }

    void Update()
    {
        float easedT = Mathf.Lerp(
            prevFuelValue,
            1f - FuelSystem.instance.GetRemainingFuelPercent(),
            Time.deltaTime * fuelEaseCatchupSpeed
        );
        prevFuelValue = easedT;
        TweenByToWith(easedT, delType, easeType);
    }

    private void TweenByToWith(float t, Ease.DelegateType delType, Ease.EaseType easeType)
    {
        t = Ease.GetDelegate(delType, easeType)(t);

        // What actually moves the object
        ObjectToTween.transform.position = Vector3.Lerp(startPos, TweenToTr.transform.position, t);
        skyboxMaterial.SetFloat("_BlurAmount", Mathf.Lerp(MinSkyboxBlur, MaxSkyboxBlur, t));
    }
}
