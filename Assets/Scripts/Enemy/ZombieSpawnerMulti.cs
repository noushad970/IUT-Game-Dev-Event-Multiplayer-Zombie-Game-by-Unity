using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ZombieSpawnerMulti : MonoBehaviourPun
{
    public GameObject[] zombiePrefabs;

    public GameObject bossPrefab;

    public Transform[] spawnPoints;

    public int zombiesToSpawn = 20;

    public float spawnDelay = 2f;

    private int spawnedCount;

    private bool bossSpawned;

    public List<GameObject> aliveZombies =
        new List<GameObject>();

    void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (spawnedCount < zombiesToSpawn)
        {
            SpawnZombie();

            spawnedCount++;

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnZombie()
    {
        Transform point =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        string prefabName =
            zombiePrefabs[Random.Range(0, zombiePrefabs.Length)].name;

        GameObject zombie =
            PhotonNetwork.InstantiateRoomObject(
                prefabName,
                point.position,
                point.rotation);

        aliveZombies.Add(zombie);

        EnemyHealthMulti hp =
            zombie.GetComponent<EnemyHealthMulti>();

        hp.OnZombieDeath += ZombieKilled;
    }

    public void ZombieKilled(GameObject zombie)
    {
        aliveZombies.Remove(zombie);

        Debug.Log("Alive Zombies : " + aliveZombies.Count);

        if (aliveZombies.Count == 0 && !bossSpawned)
        {
            SpawnBoss();
        }
    }

    void SpawnBoss()
    {
        bossSpawned = true;

        Transform point =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        PhotonNetwork.InstantiateRoomObject(
            bossPrefab.name,
            point.position,
            point.rotation);
    }
    
}