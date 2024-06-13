               using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaSpawn : MonoBehaviour
{
    public GameObject spawn;
    public GameObject[] prefabsToSpawn;
    public GameObject spawnPlace;
    public float spawnRadius = 15f;
    public float spawnTime = 10f;
    bool spawning;  
    public int spawnCapacity = 30;
    public new Light light;
    public GameObject partyLight;

    private List<GameObject> spawnList = new List<GameObject>();
    // Start is called before the first frame update
    void OnEnable()
    {
        
        if (spawn.activeSelf)
        {
            partyLight.SetActive(true);
            light.intensity = 0.005f;
            spawning = true;
            InvokeRepeating("SpawnPrefabs", 0f, spawnTime); //Llamaremos a esta función cada x seg
            InvokeRepeating("DeactivateGameObject", 15f,0f);
        }

    }

    private void Update()
    {
      
    }
    void SpawnPrefabs()
    {
        if (spawn.activeSelf && spawning) 
        {
            foreach (GameObject prefab in spawnList)
            {
                Destroy(prefab);
            }

            spawnList.Clear();
            for (int i = 0; i < spawnCapacity; i++)
            {
                int randomIndex = Random.Range(0, prefabsToSpawn.Length);
                Vector3 randomPosition = Random.insideUnitSphere;
                randomPosition.y = 0f;

                Vector3 spawnPosition = transform.position + randomPosition * spawnRadius;
                spawnPosition.y = transform.position.y;

                GameObject spawnedPrefab = Instantiate(prefabsToSpawn[randomIndex], spawnPosition, Quaternion.identity);
                spawnList.Add(spawnedPrefab);
            }
        }
    }
    void DeactivateGameObject()
    {
        if (spawn.activeSelf) 
        {
            light.intensity = 0.75f;
            foreach (GameObject prefab in spawnList)
            {
                Destroy(prefab);
            }
            spawn.SetActive(false);
            partyLight.SetActive(false);
            spawning = false;
        }
    }
  
}
