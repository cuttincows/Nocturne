using UnityEngine;
using UnityEngine.SceneManagement;

public class BootUp : MonoBehaviour {
    [SerializeField] string fishingSceneName = "Fishing";

    private void Awake() {
        if (!SceneManager.GetSceneByName(fishingSceneName).isLoaded) {
            SceneManager.LoadScene(fishingSceneName, LoadSceneMode.Additive);
        }
    }
}