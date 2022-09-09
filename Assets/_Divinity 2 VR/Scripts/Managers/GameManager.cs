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
    private AudioSource audioSource;
    public AudioSource BGMAudiosource;
    public AudioClip combatStartClip;
    public AudioClip combatEndClip;
    public AudioClip gameOverClip;
    public GameState state;
    public static event Action<GameState> GameStateChanged;
    public List<PlayerStats> players;
    public bool NextTurn
    {
        get { return _nextTurn; }
        set
        {
            _nextTurn = value;
        }
    }
    private bool _nextTurn;
    private BaseStats activeCombatant;
    public int enemiesAlive;
    public int playersAlive;
    public TextMeshProUGUI turnOrderText;
    private Coroutine turnOrderCoroutine;
    public List<KeyValuePair<BaseStats, int>> turnOrderList;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        UpdateGameState(GameState.Exploration, null);
    }

    private void Update()
    {
        if (!audioSource.isPlaying && BGMAudiosource.gameObject.activeInHierarchy && !BGMAudiosource.isPlaying)
        {
            BGMAudiosource.Play();
        }
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
        Dictionary<BaseStats, int> witsList = new Dictionary<BaseStats, int>();
        foreach (var enemy in EnemyManager.Instance.enemyList)
        {
            enemiesAlive += 1;
            enemy.InCombat = true;
            witsList.Add(enemy, enemy.wits);
        }

        foreach (var player in players)
        {
            playersAlive += 1;
            player.ExplorationMode = false;
            player.GetComponent<LocalUserObjects>().PlayerMovementAP.enabled = true;
            if (player.PlayerControlled)
            {
                SFXPlayer.Instance.PlaySFXAttach(combatStartClip, player.transform, 1f, 0.5f);
            }

            witsList.Add(player, player.wits);
        }

        var sortedTurnOrderEnumerable = from entry in witsList orderby entry.Value descending select entry;
        turnOrderList = sortedTurnOrderEnumerable.ToList();

        foreach (var character in turnOrderList)
        {
            if (character.Key.TryGetComponent(out PlayerStats playerStats))
            {
                playerStats.InCombat = true;
            }
        }

        turnOrderCoroutine = StartCoroutine(TurnOrderCoroutine(turnOrderList));
    }

    private IEnumerator TurnOrderCoroutine(List<KeyValuePair<BaseStats, int>> turnOrder)
    {
        activeCombatant = turnOrder[0].Key;
        Debug.Log("--------------------------------");
        Debug.Log($"Active combatant: {activeCombatant.Name}");
        if (activeCombatant.TryGetComponent(out PlayerStats playerStats))
        {
            playerStats.Turn = true;
            playerStats.CurrentAP = playerStats.MaxAP;
        }
        else if (activeCombatant.TryGetComponent(out EnemyStats enemyStats))
        {
            enemyStats.Turn = true;
            enemyStats.CurrentAP = enemyStats.MaxAP;
        }

        UpdateTurnOrderText(turnOrder);

        NextTurn = false;

        while (!NextTurn)
        {
            if (playersAlive == 0)
            {
                HandleGameOver();
            }

            if (enemiesAlive == 0)
            {
                EndCombat();
            }

            yield return null;
        }

        turnOrder.Add(turnOrder[0]);
        turnOrder.Remove(turnOrder[0]);
        
        turnOrderCoroutine = StartCoroutine(TurnOrderCoroutine(turnOrder));
    }

    private void EndCombat()
    {
        Debug.Log($"ENEMIES FELLED. EXITING COMBAT");
        StopCoroutine(turnOrderCoroutine);
        audioSource.PlayOneShot(combatEndClip);
        BGMAudiosource.Stop();
        turnOrderText.text = "";
        foreach (var playerEndCombat in players)
        {
            playerEndCombat.CurrentAP = playerEndCombat.MaxAP;
            playerEndCombat.ExplorationMode = true;
        }
    }

    private void HandleGameOver()
    {
        Debug.Log($"PLAYERS FELLED. RESTART FROM LAST SAVE");
        StopCoroutine(turnOrderCoroutine);
        audioSource.PlayOneShot(gameOverClip);
        BGMAudiosource.Stop();
        turnOrderText.text = "";

        foreach (var enemy in EnemyManager.Instance.enemyList)
        {
            enemy.InCombat = false;
        }

        if (UserMenu.Instance.menuIsOpen)
        {
            UserMenu.Instance.ToggleMenu();
        }
    }

    public void UpdateTurnOrderText(List<KeyValuePair<BaseStats, int>> turnOrder)
    {
        turnOrderText.text = null;
        for (int i = 0; i < turnOrder.Count; i++)
        {
            turnOrderText.text += $"{i}. {turnOrder[i].Key.Name}, ";
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

    public PlayerStats FindControlledPlayer()
    {
        foreach (var instancePlayer in players)
        {
            if (instancePlayer.PlayerControlled)
            {
                return instancePlayer;
            }
        }

        Debug.LogError("FindControlledPlayer() couldn't find a controlled player!");
        return null;
    }
    
    public void UpdateCombatantTurn()
    {
        if (activeCombatant.TryGetComponent(out PlayerStats player))
        {
            player.Turn = false;
        }
        else if (activeCombatant.TryGetComponent(out EnemyStats enemyStatsDebug))
        {
            enemyStatsDebug.Turn = false;
        }
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