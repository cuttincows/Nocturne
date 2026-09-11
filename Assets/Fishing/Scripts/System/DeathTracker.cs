using TMPro;
using UnityEngine;

public class DeathTracker : MonoBehaviour
{
    float timeElapsed = 0;
    [SerializeField] TMP_Text label;
    [SerializeField] GameObject deathScreen;
    private void Update()
    {
        timeElapsed += Time.deltaTime;
    }

    public void Die()
    {
        deathScreen.SetActive(true);
        enabled = false;
        label.text = timeElapsed.ToString("F1");
    }
}
