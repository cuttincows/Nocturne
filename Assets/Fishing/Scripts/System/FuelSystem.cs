using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FuelSystem : MonoBehaviour
{
    public static FuelSystem instance;
    public UnityEvent onRunOut;

    public float GetRemainingFuelPercent()
    {
        return slider.value / slider.maxValue;
    }

    private void Awake()
    {
        instance = this;
    }
    [SerializeField] float decaySpeed = 1;
    [SerializeField] float decayAcceleration = 1/100f;
    [Header("Seconds before the drain starts speeding up at all")]
    [SerializeField] float accelerationDelay = 120f;
    [Header("1 = straight ramp, higher = stays gentle longer then bites")]
    [SerializeField] float accelerationPower = 1f;
    float timeElapsed;

    [SerializeField] Slider slider;
    public void AddFuel(int fuel)
    {
        slider.value += fuel;
    }

    private void FixedUpdate()
    {
        timeElapsed += Time.fixedDeltaTime;
        slider.value -= Func(timeElapsed) * Time.fixedDeltaTime * decaySpeed;
        if (slider.value <= 0)
        {
            onRunOut.Invoke();
            enabled = false;
        }
    }

    private float Func(float x)
    {
        float t = Mathf.Max(0f, x - accelerationDelay);
        return 1f + (decayAcceleration * Mathf.Pow(t, accelerationPower));
    }
}
