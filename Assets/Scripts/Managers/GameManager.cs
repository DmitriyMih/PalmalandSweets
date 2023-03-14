using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Timer Settings")]
    [SerializeField] private float maxGameTimer = 15f;
    [SerializeField] private float currentGameTimer;
    [SerializeField] private bool gameInProgress;

    public bool GameInProgress => gameInProgress;
    public Action<bool> GameStateChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameStateChanged += GameManager_GameStateChanged;
        StartGame();
    }

    private void GameManager_GameStateChanged(bool newState)
    {
        gameInProgress = newState;
    }

    private void Update()
    {
        if (!GameInProgress)
            return;

        if (currentGameTimer <= 0)
        {
            EndGame();
        }
        else
            currentGameTimer -= Time.deltaTime;
    }

    private void StartGame()
    {
        currentGameTimer = maxGameTimer;
        GameStateChanged?.Invoke(true);
    }

    private void EndGame()
    {
        GameStateChanged?.Invoke(false);
    }
}