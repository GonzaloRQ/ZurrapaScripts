using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] objectsToUse;  // Asigna estos objetos en el editor
    public GameObject spawnPlace;
    public float spawnRadius = 25f;
    public float spawnTime = 25f;
    public int spawnCapacity = 20;

    private List<GameObject> activeObjects = new List<GameObject>();
    private List<GameObject> pool = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        InitializePool();
        InvokeRepeating("SpawnPrefabs", 0f, spawnTime);
    }

    private void Update()
    {
        int score = ScoreManager.GetScore();

        if (score >= 3000)
        {
            spawnTime = 5f;
            spawnCapacity = 25;
        }
        else if (score >= 2000)
        {
            spawnTime = 10f;
            spawnCapacity = 20;
        }
        else if (score >= 1000)
        {
            spawnTime = 15f;
            spawnCapacity = 15;
        }
    }

    void InitializePool()
    {
        foreach (GameObject obj in objectsToUse)
        {
            obj.SetActive(false);
            pool.Add(obj);
        }
    }

    void SpawnPrefabs()
    {
        // Deactivate all active objects
        foreach (GameObject obj in activeObjects)
        {
            obj.SetActive(false);
            pool.Add(obj);
        }
        activeObjects.Clear();

        for (int i = 0; i < spawnCapacity; i++)
        {
            if (pool.Count > 0)
            {
                int randomIndex = Random.Range(0, pool.Count);
                GameObject obj = pool[randomIndex];
                pool.RemoveAt(randomIndex);

                Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
                randomPosition.y = 0f;

                Vector3 spawnPosition = transform.position + randomPosition;
                spawnPosition.y = transform.position.y;

                obj.transform.position = spawnPosition;
                obj.SetActive(true);
                activeObjects.Add(obj);
            }
        }
    }
}
