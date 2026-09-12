using UnityEngine;
using UnityEngine.SceneManagement;

public class BootUp : MonoBehaviour {
    [SerializeField] string fishingSceneName = "Fishing";

    public bool shouldLoadFishingScene;

    private void Awake() 
    {
        if (shouldLoadFishingScene && !SceneManager.GetSceneByName(fishingSceneName).isLoaded)
        {
            SceneManager.LoadScene(fishingSceneName, LoadSceneMode.Additive);
        }
    }
}