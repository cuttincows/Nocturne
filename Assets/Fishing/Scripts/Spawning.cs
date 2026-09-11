using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnObject
{
    public string name;
    public Spawnable spawnable;
    // How many there should realistically be in the scene at once.
    public float idealMaxAmountInSceneAtOnce;
    // How rare this fish is to spawn compared to others.
    public float relativeRarity;
}
/// <summary>
/// contact ande for help (: (sorry)
/// </summary>
public class Spawning : MonoBehaviour
{
    float currPower = 0;
    public List<SpawnObject> spawnObjects;
    public SpawnBox spawnBox;

    public float spawnInterval = 0.5f;
    float timer = 0;
    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer -= spawnInterval;
            TrySpawn();
        }
    }

    public void TrySpawn()
    {
        float spawnChance = GetSpawnChance();
        if (Random.value < spawnChance)
        {
            SpawnObject objToSpawn = GetRandomSpawnObject();
            if (objToSpawn != null)
            {
                Vector3 pos = spawnBox.GetRandomPosition();
                Quaternion rot = objToSpawn.spawnable.transform.rotation;
                GameObject prefab = objToSpawn.spawnable.gameObject;
                Spawnable spawnable = Instantiate(prefab, pos, rot).GetComponent<Spawnable>();
                float powerToAssign = 1f / objToSpawn.idealMaxAmountInSceneAtOnce;
                spawnable.Spawn(this, powerToAssign);
                currPower += powerToAssign;
                spawnable.gameObject.SetActive(true);
            }
        }
    }

    public void ReportDeath(Spawnable spawnable)
    {
        currPower -= spawnable.power;
    }

    private SpawnObject GetRandomSpawnObject()
    {
        float totalRarity = 0;
        foreach (SpawnObject obj in spawnObjects)
        {
            totalRarity += obj.relativeRarity;
        }

        float randomValue = Random.value * totalRarity;
        foreach (SpawnObject obj in spawnObjects)
        {
            if (randomValue < obj.relativeRarity)
            {
                return obj;
            }
            randomValue -= obj.relativeRarity;
        }

        throw new System.Exception("No spawn object found. Check if the spawnObjects list is empty or if all rarities are zero.");
    }

    private float GetSpawnChance()
    {
        float ratio = currPower;
        return 2 * (1 - (1/(1 + Mathf.Exp(-4 * ratio))));
    }
}
