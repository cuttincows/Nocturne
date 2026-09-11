using UnityEngine;

public class Bait : MonoBehaviour
{
    public void GetEatenBy(Eater eater)
    {
        Destroy(gameObject);
    }
}
