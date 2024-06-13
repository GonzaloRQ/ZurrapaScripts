using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableSpawn : MonoBehaviour
{
    public GameObject[] prefabsToSpawn;
    public GameObject spawnPlace;
    public float spawnRadius = 15f;
    public float spawnTime = 10f;
    public int spawnCapacity = 1;

    private List<GameObject> spawnList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnPrefabs", 0f, spawnTime); //Llamaremos a esta función cada x seg
    }

    void SpawnPrefabs()
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
