using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HurricaneVR.Framework.Core.Utils;
using intheclouds;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState state;
    public static event Action<GameState> GameStateChanged;
    public List<PlayerStats> players;
    public AudioClip combatStartClip;
    public bool nextTurn;
    public KeyValuePair<BaseStats, int> currentCombatant;
    public int enemiesAlive;
    public int playersAlive;
    public TextMeshProUGUI turnOrderText;

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
        Dictionary<BaseStats, int> witsList = new Dictionary<BaseStats, int>();
        foreach (var enemy in enemyManager.enemyList)
        {
            enemiesAlive += 1;
            enemy.enemyEngaged = true;
            witsList.Add(enemy, enemy.wits);
        }

        foreach (var player in players)
        {
            playersAlive += 1;
            player.explorationMode = false;
            player.GetComponent<LocalUserObjects>().PlayerMovementAP.enabled = true;
            if (player.playerControlled)
            {
                SFXPlayer.Instance.PlaySFXAttach(combatStartClip, player.transform, 1f, 0.5f);
            }

            witsList.Add(player, player.wits);
        }

        var initialTurnOrder = from entry in witsList orderby entry.Value descending select entry;
        var newTurnList = initialTurnOrder.ToList();

        foreach (var character in newTurnList)
        {
            if (character.Key.TryGetComponent(out PlayerStats playerStats))
            {
                playerStats.inCombat = true;
            }
        }

        StartCoroutine(TurnOrderCoroutine(newTurnList));
    }

    private IEnumerator TurnOrderCoroutine(List<KeyValuePair<BaseStats, int>> turnOrder)
    {
        if (playersAlive == 0)
        {
            Debug.Log($"ALL PLAYERS DEAD. RESTART FROM LAST SAVE");
            HandleGameOver();
            StopCoroutine(TurnOrderCoroutine(turnOrder));
        }

        if (enemiesAlive == 0)
        {
            Debug.Log($"ENEMIES FELLED. EXITING COMBAT");
            // todo: exit combat. reset players AP
            
            
            StopCoroutine(TurnOrderCoroutine(turnOrder));
        }

        currentCombatant = turnOrder[0];
        if (currentCombatant.Key.TryGetComponent(out PlayerStats playerStats))
        {
            playerStats.Turn = true;
            playerStats.CurrentAP = playerStats.MaxAP;
        }
        else if (currentCombatant.Key.TryGetComponent(out EnemyStats enemyStats))
        {
            enemyStats.Turn = true;
            enemyStats.CurrentAP = enemyStats.MaxAP;
            Debug.Log("reset enemy ap");
        }
        
        turnOrderText.text = null;
        for (int i = 0; i < turnOrder.Count; i++)
        {
            turnOrderText.text += $"{i}. {turnOrder[i].Key.Name}, ";
        }

        nextTurn = false;

        while (!nextTurn)
        {
            yield return null;
        }

        // force current combatant turn off (for Next Turn debug UI)
        if (currentCombatant.Key.TryGetComponent(out PlayerStats playerStatsDebug))
        {
            playerStatsDebug.Turn = false;
        }
        else if (currentCombatant.Key.TryGetComponent(out EnemyStats enemyStatsDebug))
        {
            enemyStatsDebug.Turn = false;
        }

        turnOrder.Add(turnOrder[0]);
        turnOrder.Remove(turnOrder[0]);

        Debug.Log($"NEXT TURN: {turnOrder[0].Key.Name}");

        StartCoroutine(TurnOrderCoroutine(turnOrder));
    }

    private void HandleGameOver()
    {
        if (UserMenu.Instance.menuIsOpen)
        {
            UserMenu.Instance.ToggleMenu();
        }
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