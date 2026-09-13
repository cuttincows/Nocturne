using UnityEngine;

public class DespawnTimer : MonoBehaviour
{
    public float timer;
    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                timer = 0;
            }
        }
    }

    public void TryDie()
    {
        if (timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}
