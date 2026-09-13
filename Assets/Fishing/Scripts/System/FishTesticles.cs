using System.Collections.Generic;
using UnityEngine;


public class FishTesticles : MonoBehaviour
{
    private void Start()
    {
        TestFishies();
    }
    [ContextMenu("Test Fish")]
    public void TestFishies()
    {
        foreach (var c in FindObjectsByType<Spawning>(FindObjectsSortMode.None))
        {
            c.SetTest();
        }
    }
}
