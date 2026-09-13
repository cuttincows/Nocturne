using System;
using System.Security.Cryptography;
using UnityEngine;

public class TweenObjectTo : MonoBehaviour
{
    public GameObject ObjectToTween;
    public Transform TweenToTr;
    private Vector3 startPos;

    Ease.DelegateType delType;
    Ease.EaseType easeType;

    void Start()
    {
        startPos = ObjectToTween.transform.position;
    }

    void Update()
    {
        TweenByToWith(FuelSystem.instance.GetRemainingFuelPercent(), delType, easeType);
    }

    private void TweenByToWith(float t, Ease.DelegateType delType, Ease.EaseType easeType)
    {
        t = Ease.GetDelegate(delType, easeType)(t);
        ObjectToTween.transform.position = Vector3.Lerp(startPos, TweenToTr.transform.position, t);
    }
}
