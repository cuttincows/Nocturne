using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FoodSystem : MonoBehaviour
{
    public static FoodSystem instance;
    public UnityEvent onDie;

    private void Awake()
    {
        instance = this;
    }
    [SerializeField] float decaySpeed = 1;
    [SerializeField] Slider slider;
    public void AddFood(int fuel)
    {
        slider.value += fuel;
    }

    private void FixedUpdate()
    {
        slider.value -= Time.fixedDeltaTime * decaySpeed;
    }
}
