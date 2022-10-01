using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HurricaneVR.Framework.Core.Utils;
using intheclouds;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private AudioSource audioSource;
    public AudioSource MusicAudioSource;
    public AudioClip combatStartClip;
    public AudioClip combatEndClip;
    public AudioClip gameOverClip;
    public GameState state;
    public static event Action<GameState> GameStateChanged;
    public List<PlayerStats> players;
    public bool NextTurn;
    public bool NewRound;
    private BaseStats activeCombatant;
    private BaseStats firstCombatant;
    public int enemiesAlive;
    public int playersAlive;
    public TextMeshProUGUI turnOrderText;
    private Coroutine turnOrderCoroutine;
    public List<KeyValuePair<BaseStats, int>> turnOrderList;
    public PlayerStats controlledPlayer;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        controlledPlayer = FindControlledPlayer();
    }

    private void Start()
    {
        UpdateGameState(GameState.Exploration);
    }

    private void Update()
    {
        // Resume music after other audiosource finished
        if (!audioSource.isPlaying && MusicAudioSource.gameObject.activeInHierarchy && !MusicAudioSource.isPlaying)
        {
            MusicAudioSource.Play();
        }
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Exploration:
                HandleExploration();
                break;
            case GameState.CombatStart:
                HandleCombatStart();
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

    private void HandleCombatStart()
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
            player.InCombat = true;
            player.GetComponent<LocalUserObjects>().PlayerMovementAP.enabled = true;
            if (player.PlayerControlled)
            {
                SFXPlayer.Instance.PlaySFXAttach(combatStartClip, player.transform, 1f, 0.5f);
            }

            witsList.Add(player, player.Wits);
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
        Debug.Log($"---Active combatant: {activeCombatant.Name}---");
        if (!firstCombatant)
        {
            firstCombatant = activeCombatant;
        }
        else if (activeCombatant == firstCombatant)
        {
            NewRound = true;
            SurfaceEffectsContainer.Instance.Cooldown();
        }
        if (activeCombatant.TryGetComponent(out BaseStats combatantStats))
        {
            combatantStats.Turn = true;
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
        MusicAudioSource.Stop();
        turnOrderText.text = "";
        firstCombatant = null;
        foreach (var playerEndCombat in players)
        {
            playerEndCombat.CurrentAP = playerEndCombat.MaxAP;
            playerEndCombat.InCombat = false;
        }
    }

    private void HandleGameOver()
    {
        Debug.Log($"PLAYERS FELLED. RESTART FROM LAST SAVE");
        StopCoroutine(turnOrderCoroutine);
        audioSource.PlayOneShot(gameOverClip);
        MusicAudioSource.Stop();
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

    public void ForceNextTurn()
    {
        if (state == GameState.CombatStart)
        {
            if (activeCombatant.TryGetComponent(out BaseStats combatantStats))
            {
                combatantStats.Turn = false;
            }
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