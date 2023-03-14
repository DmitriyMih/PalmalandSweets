using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distributor : MonoBehaviour
{
    [SerializeField] private PathController pathController;

    [Header("Spawn Settings")]
    [SerializeField] private PathFollower itemPrefab;
    [SerializeField] private Transform spawnPoint;

    [Space]
    [SerializeField] private int maxSpawnCount;
    [SerializeField] private int currentSpawnCount;

    [Space]
    [SerializeField] private float maxSpawnTime;
    [SerializeField] private float currentSpawnTime;

    [SerializeField] private float acceleration;

    public Action<PathFollower> InstatiateAction;

    private void Awake()
    {
        currentSpawnCount = maxSpawnCount;
        currentSpawnTime = maxSpawnTime;
    }

    private void Update()
    {
        if (currentSpawnCount <= 0 || pathController == null || spawnPoint == null)
            return;

        if (GameManager.Instance != null)
            if (!GameManager.Instance.GameInProgress)
                return;

        if (currentSpawnTime >= maxSpawnTime * acceleration)
        {
            currentSpawnTime = 0;
            currentSpawnCount -= 1;
            InstatiateSphere();
        }
        else
            currentSpawnTime += Time.deltaTime;
    }

    private void InstatiateSphere()
    {
        if (GameManager.Instance != null)
            if (!GameManager.Instance.GameInProgress)
                return;

        PathFollower item = Instantiate(itemPrefab, spawnPoint.position, Quaternion.identity);
        pathController.AddItemInPath(item);
        InstatiateAction?.Invoke(item);
    }

    public void ChangeSpeedAcceleration(float newSpeed)
    {
        acceleration = maxSpawnTime / newSpeed;
    }
}