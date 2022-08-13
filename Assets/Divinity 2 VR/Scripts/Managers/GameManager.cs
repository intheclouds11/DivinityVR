using System;
using HurricaneVR.Framework.Core.Utils;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState state;
    public static event Action<GameState> GameStateChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Exploration);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.PlayerTurn:
                HandlePlayerTurn();
                break;
            case GameState.EnemyTurn:
                HandleEnemyTurn();
                break;
            case GameState.Exploration:
                HandleExploration();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        GameStateChanged?.Invoke(newState);
    }

    private void HandleEnemyTurn()
    {
        // reset enemy AP
    }

    private void HandlePlayerTurn()
    {
        // reset player AP
    }

    private void HandleExploration()
    {
        // disable AP scripts?
    }
}

public enum GameState
{
    PlayerTurn,
    EnemyTurn,
    Exploration,
    Dialogue
}