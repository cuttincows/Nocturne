using UnityEngine;

public class DespawnTimer : MonoBehaviour
{
    public float timer;
    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 0;
            Destroy(gameObject);
        }
    }
}
