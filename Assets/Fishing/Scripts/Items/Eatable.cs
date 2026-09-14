using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Destroyable))]
public class Eatable : MonoBehaviour
{
    public bool canEat;
    public bool GetEatenBy(Eater eater)
    {
        if (canEat)
        {
            GetComponent<Destroyable>().Destroy();
        }
        return canEat;
    }
}
