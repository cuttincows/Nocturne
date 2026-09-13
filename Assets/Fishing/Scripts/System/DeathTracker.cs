using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathTracker : MonoBehaviour
{
    float timeElapsed = 0;
    [SerializeField] TMP_Text label;
    [SerializeField] TMP_Text causeText;
    [SerializeField] GameObject deathScreen;
    private void Update()
    {
        timeElapsed += Time.deltaTime;
    }

    private void Start()
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
    }

    public void Die(string cause = "idk")
    {
        causeText.text = "Cause: " + cause;
        deathScreen.SetActive(true);
        enabled = false;
        label.text = timeElapsed.ToString("F1");
        SceneManager.LoadScene("MainMenu");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }
}
