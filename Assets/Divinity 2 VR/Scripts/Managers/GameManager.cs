using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HurricaneVR.Framework.Core.Utils;
using intheclouds;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState state;
    public static event Action<GameState> GameStateChanged;
    public List<PlayerStats> players;
    public AudioClip combatStartClip;
    public bool firstTurn;
    public bool turnGameManager;
    KeyValuePair<GameObject, int> currentCombatant;
    public int enemiesAlive;
    public int playersAlive;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Exploration, null);
    }

    public void UpdateGameState(GameState newState, EnemyStats enemyEngaged)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Exploration:
                HandleExploration();
                break;
            case GameState.CombatStart:
                HandleCombatStart(enemyEngaged);
                break;
            case GameState.PlayerTurn:
                HandlePlayerTurn();
                break;
            case GameState.EnemyTurn:
                HandleEnemyTurn();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        GameStateChanged?.Invoke(newState);
    }

    private void HandleExploration()
    {
        // disable AP scripts?
    }

    private void HandleCombatStart(EnemyStats enemyEngaged)
    {
        Debug.Log("COMBAT START");
        var enemyManager = enemyEngaged.GetComponentInParent<EnemyManager>();
        Dictionary<GameObject, int> witsList = new Dictionary<GameObject, int>();
        foreach (var enemy in enemyManager.enemyList)
        {
            enemiesAlive += 1;
            enemy.enemyEngaged = true;
            witsList.Add(enemy.gameObject, enemy.attributes.wits);
        }

        foreach (var player in players)
        {
            playersAlive += 1;
            player.explorationMode = false;
            player.GetComponent<PlayerMovementAP>().enabled = true;
            if (player.playerControlled)
            {
                SFXPlayer.Instance.PlaySFXAttach(combatStartClip, player.transform, 1f, 1f);
            }

            witsList.Add(player.gameObject, player.attributes.wits);
        }

        var initialTurnOrder = from entry in witsList orderby entry.Value descending select entry;

        foreach (var keyValuePair in initialTurnOrder)
        {
            Debug.Log($"character: {keyValuePair.Key}, wits: {keyValuePair.Value}");
        }

        firstTurn = true;
        StartCoroutine(TurnOrderCoroutine(initialTurnOrder));
    }

    private IEnumerator TurnOrderCoroutine(IOrderedEnumerable<KeyValuePair<GameObject, int>> turnOrder)
    {
        if (playersAlive == 0)
        {
            Debug.Log($"ALL PLAYERS DEAD. RESTART FROM LAST SAVE");
            StopCoroutine(TurnOrderCoroutine(turnOrder));
        }

        if (enemiesAlive == 0)
        {
            Debug.Log($"ENEMIES FELLED. EXITING COMBAT");
            StopCoroutine(TurnOrderCoroutine(turnOrder));
        }

        if (firstTurn)
        {
            firstTurn = false;
            currentCombatant = turnOrder.First();
            if (currentCombatant.Key.TryGetComponent(out PlayerStats playerStats))
            {
                playerStats.turn = true;
                turnGameManager = true;
            }
            else if (currentCombatant.Key.TryGetComponent(out EnemyStats enemyStats))
            {
                enemyStats.turn = true;
                turnGameManager = true;
            }
        }

        while (turnGameManager)
        {
            yield return null;
        }


        var newTurnList = turnOrder.ToList();
        newTurnList.Add(newTurnList[0]);
        newTurnList.Remove(newTurnList[0]);

        var newTurnOrder = newTurnList.AsEnumerable();
        foreach (var keyValuePair in newTurnOrder)
        {
            Debug.Log($"NEW TURN ORDER! character: {keyValuePair.Key}");
        }

        Debug.Log("NEXT TURN");
    }

    private void HandleEnemyTurn()
    {
        // reset enemy AP
    }

    private void HandlePlayerTurn()
    {
        // reset player AP
    }
}

public enum GameState
{
    Exploration,
    CombatStart,
    PlayerTurn,
    EnemyTurn,
    Dialogue
}