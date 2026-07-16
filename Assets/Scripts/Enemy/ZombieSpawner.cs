using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Zombie Prefabs")]
    public GameObject[] zombiePrefabs;

    [Header("Boss")]
    public GameObject bossPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    public int zombiesToSpawn = 20;
    public float spawnPointCooldown = 5f;
    public float spawnCheckDelay = 0.2f;
    public bool spawnOnStart = true;

    [Header("Runtime")]
    public List<GameObject> aliveZombies = new List<GameObject>();

    private float[] nextAvailableTime;

    private int spawnedCount;
    private bool bossSpawned;

    private void Start()
    {
        nextAvailableTime = new float[spawnPoints.Length];

        if (spawnOnStart)
            StartSpawning();
        totalZombieDied = 0;
    }

    private void LateUpdate()
    {
        if(zombiesToSpawn == totalZombieDied && !bossSpawned)
        {
            zombieDiedCount();
            SpawnBoss();

        }
    }
    public void StartSpawning()
    {
        StopAllCoroutines();

        aliveZombies.Clear();

        spawnedCount = 0;
        bossSpawned = false;

        nextAvailableTime = new float[spawnPoints.Length];

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (spawnedCount < zombiesToSpawn)
        {
            if (TrySpawnZombie())
            {
                spawnedCount++;
                Debug.Log("Spawned Count : " + spawnedCount);
            }

            yield return new WaitForSeconds(spawnCheckDelay);
        }

        Debug.Log("Finished Spawning Zombies");
    }

    bool TrySpawnZombie()
    {
        if (zombiePrefabs.Length == 0)
        {
            Debug.LogError("Zombie Prefabs Missing");
            return false;
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn Points Missing");
            return false;
        }

        List<int> available = new List<int>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (Time.time >= nextAvailableTime[i])
                available.Add(i);
        }

        if (available.Count == 0)
            return false;

        int index = available[Random.Range(0, available.Count)];

        nextAvailableTime[index] = Time.time + spawnPointCooldown;

        Transform point = spawnPoints[index];

        GameObject zombie = Instantiate(
            zombiePrefabs[Random.Range(0, zombiePrefabs.Length)],
            point.position,
            point.rotation);

        aliveZombies.Add(zombie);

        EnemyHealth health = zombie.GetComponent<EnemyHealth>();

        if (health != null)
        {
            health.OnEnemyDeath -= ZombieKilled;
        }
        else
        {
            Debug.LogWarning(zombie.name + " has no ZombieDeath component.");
        }

        Debug.Log("Zombie Spawned : " + zombie.name);

        return true;
    }

    void ZombieKilled(GameObject zombie)
    {
        Debug.Log("Zombie Died : " + zombie.name);

        EnemyHealth health = zombie.GetComponent<EnemyHealth>();

        if (health != null)
        {
            health.OnEnemyDeath -= ZombieKilled;
        }

        aliveZombies.Remove(zombie);

        Debug.Log("Alive Zombies : " + aliveZombies.Count);

        
    }

    void SpawnBoss()
    {
        bossSpawned = true;

        if (bossPrefab == null)
        {
            Debug.LogError("Boss Prefab Missing!");
            return;
        }

        Transform point =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(
            bossPrefab,
            point.position,
            point.rotation);

        Debug.Log("==============================");
        Debug.Log("       BOSS SPAWNED");
        Debug.Log("==============================");
    }
    int totalZombieDied = 0;
    public void zombieDiedCount()
    {
        totalZombieDied++;
    }
}