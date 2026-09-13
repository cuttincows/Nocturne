using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScener : MonoBehaviour
{
    public string Scene;

    public void Load()
    {
        SceneManager.LoadScene(Scene);
    }
}
