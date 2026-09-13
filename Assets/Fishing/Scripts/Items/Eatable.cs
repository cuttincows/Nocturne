using UnityEngine;

public class Eatable : MonoBehaviour
{
    public bool canEat;
    public bool GetEatenBy(Eater eater)
    {
        if (canEat)
            Destroy(gameObject);
        return canEat;
    }
}
