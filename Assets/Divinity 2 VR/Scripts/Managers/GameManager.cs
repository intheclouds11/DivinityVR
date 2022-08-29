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
    public KeyValuePair<ICharacter, int> currentCombatant;
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
        Dictionary<ICharacter, int> witsList = new Dictionary<ICharacter, int>();
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
            player.GetComponent<PlayerMovementAP>().enabled = true;
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
            if (character.Key.CharacterType.TryGetComponent(out PlayerStats playerStats))
            {
                playerStats.inCombat = true;
            }
        }

        StartCoroutine(TurnOrderCoroutine(newTurnList));
    }

    private IEnumerator TurnOrderCoroutine(List<KeyValuePair<ICharacter, int>> turnOrder)
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

        currentCombatant = turnOrder[0];
        if (currentCombatant.Key.CharacterType.TryGetComponent(out PlayerStats playerStats))
        {
            playerStats.turn = true;
            playerStats.currentAP = playerStats.maxAP;
        }
        else if (currentCombatant.Key.CharacterType.TryGetComponent(out EnemyStats enemyStats))
        {
            enemyStats.turn = true;
            enemyStats.currentAP = enemyStats.maxAP;
        }
        
        turnOrderText.text = null;
        for (int i = 0; i < turnOrder.Count; i++)
        {
            turnOrderText.text += $"{i}. {turnOrder[i].Key.Name}, ";
            Debug.Log($"character: {turnOrder[i].Key}, wits: {turnOrder[i].Value}");
        }

        nextTurn = false;

        while (!nextTurn)
        {
            yield return null;
        }

        // force current combatant turn off (for Next Turn debug UI)
        if (currentCombatant.Key.CharacterType.TryGetComponent(out PlayerStats playerStatsDebug))
        {
            playerStatsDebug.turn = false;
        }
        else if (currentCombatant.Key.CharacterType.TryGetComponent(out EnemyStats enemyStatsDebug))
        {
            enemyStatsDebug.turn = false;
        }

        turnOrder.Add(turnOrder[0]);
        turnOrder.Remove(turnOrder[0]);

        foreach (var keyValuePair in turnOrder)
        {
            Debug.Log($"NEW TURN ORDER! character: {keyValuePair.Key}");
        }

        Debug.Log("NEXT TURN");

        StartCoroutine(TurnOrderCoroutine(turnOrder));
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