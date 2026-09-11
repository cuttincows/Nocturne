using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    public string Scene;

    public void Restart()
    {
        SceneManager.LoadScene(Scene);
    }
}
