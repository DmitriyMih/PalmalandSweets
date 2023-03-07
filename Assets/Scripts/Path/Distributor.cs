using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distributor : MonoBehaviour
{
    [SerializeField] private PathController pathController;

    [Header("Spawn Settings")]
    [SerializeField] private BaseSphereItem itemPrefab;
    [SerializeField] private Transform spawnPoint;

    [Space]
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private int currentSpawnCount;

    [Space]
    [SerializeField] private float maxSpawnTime;
    [SerializeField] private float currentSpawnTime;

    private void Awake()
    {
        currentSpawnCount = maxSpawnCount;
    }

    private void Update()
    {
        if (currentSpawnCount <= 0 || pathController == null || spawnPoint == null)
            return;

        if (currentSpawnTime < 0)
        {
            currentSpawnTime = maxSpawnTime;
            currentSpawnCount -= 1;

            BaseSphereItem sphereItem = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
            pathController.AddSphereItem(sphereItem);
        }
        else
            currentSpawnTime -= Time.deltaTime;
    }
}