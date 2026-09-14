using UnityEngine;
using UnityEngine.Events;

public class Destroyable : MonoBehaviour
{
    public UnityEvent onDestroy;

    public void Destroy()
    {
        onDestroy?.Invoke();
        Destroy(gameObject);
    }
}
