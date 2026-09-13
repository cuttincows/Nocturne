using UnityEngine;
using UnityEngine.Events;

public class RandomDelay : MonoBehaviour
{
    public float delay = 10f;
    public float randomOffset = 1f;
    [SerializeField] float timer;
    public UnityEvent onTrigger;

    private void RandomizeTimer()
    {
        timer += delay + Random.Range(0, randomOffset);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            onTrigger.Invoke();
            RandomizeTimer();
        }
    }
}
