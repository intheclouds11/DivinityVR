using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HighlightPlus;
using HurricaneVR.Framework.Core.Utils;
using intheclouds;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState state;
    public List<KeyValuePair<BaseStats, int>> turnOrderList;
    public List<PlayerStats> players;
    public PlayerStats controlledPlayer;
    [field: SerializeField] public BaseStats activeCombatant { get; private set; }
    public bool NextTurn;
    public static event Action<GameState> GameStateChanged;
    [Header("Setup")]
    public AudioSource MusicAudioSource;
    public AudioClip combatStartClip;
    public AudioClip enemyJoinedClip;
    public AudioClip nextTurnClip;
    public AudioClip combatEndClip;
    public AudioClip gameOverClip;
    public TextMeshProUGUI turnOrderText;
    public GameObject turnOrderUI;

    private BaseStats firstCombatant;
    private BaseStats previousCombatant;
    private Coroutine turnOrderCoroutine;
    private AudioSource audioSource;

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
        var witsList = new Dictionary<BaseStats, int>();

        foreach (var player in players)
        {
            player.InCombat = true;
            player.GetComponent<LocalUserObjects>().PlayerMovementAP.enabled = true;
            if (player.PlayerControlled)
            {
                SFXPlayer.Instance.PlaySFX(combatStartClip, player.LocalUserObjects.Camera.transform.position, 1f, 0.5f, 10, false, false);
            }

            witsList.Add(player, player.Wits);
        }

        EnemyManager.Instance.PopulateEnemiesInCombatList();
        foreach (var enemy in EnemyManager.Instance.EnemiesInCombat)
        {
            enemy.InCombat = true;
            witsList.Add(enemy, enemy.wits);
        }

        var sortedTurnOrderEnumerable = from entry in witsList orderby entry.Value descending select entry;
        turnOrderList = sortedTurnOrderEnumerable.ToList();

        foreach (var character in turnOrderList)
        {
            character.Key.InCombat = true;
        }

        turnOrderUI.SetActive(true);
        turnOrderCoroutine = StartCoroutine(TurnOrderCoroutine());
    }

    private IEnumerator TurnOrderCoroutine()
    {
        while (true)
        {
            activeCombatant = turnOrderList[0].Key;
            UpdateTurnOrderText(turnOrderList);
            activeCombatant.Turn = true;
            NextTurn = false;

            if (!firstCombatant)
            {
                firstCombatant = activeCombatant;
            }
            else if (activeCombatant == firstCombatant)
            {
                StartNewRound();
            }

            while (!NextTurn)
            {
                if (!players.Any() || !EnemyManager.Instance.EnemiesInCombat.Any())
                {
                    EndCombat();
                }

                yield return null;
            }

            // Move current combatant to end of turnOrderList
            turnOrderList.Add(turnOrderList[0]);
            turnOrderList.Remove(turnOrderList[0]);
            
            previousCombatant = activeCombatant;
            SFXPlayer.Instance.PlaySFX(nextTurnClip, LocalUserObjects.Instance.ITCPlayerController.transform.position, 1f, 1f, 10, false, false);
        }
    }

    private static void StartNewRound()
    {
        SurfaceEffectsContainer.Instance.Cooldown();
    }

    private void EndCombat()
    {
        if (!players.Any())
        {
            Debug.Log($"PLAYERS FELLED. RESTART FROM LAST SAVE");
            audioSource.PlayOneShot(gameOverClip);
            MusicAudioSource.Stop();
            if (!UserMenu.Instance.menuIsOpen)
            {
                UserMenu.Instance.ToggleMenu(true);
            }
        }
        else
        {
            Debug.Log($"ENEMIES FELLED. EXITING COMBAT");
            audioSource.PlayOneShot(combatEndClip);
            MusicAudioSource.Pause();
        }

        StopCoroutine(turnOrderCoroutine);
        turnOrderUI.SetActive(false);
        turnOrderText.text = "";
        firstCombatant = null;
        activeCombatant = null;

        foreach (var player in players)
        {
            player.CurrentAP = player.MaxAP;
            player.InCombat = false;
            player.Leaning = false;
            player.Turn = false;
        }

        foreach (var enemy in EnemyManager.Instance.EnemiesInCombat)
        {
            enemy.InCombat = false;
            enemy.Turn = false;
        }
    }

    public void UpdateTurnOrderText(List<KeyValuePair<BaseStats, int>> turnOrder)
    {
        var highlight = activeCombatant.GetComponentInChildren<HighlightEffect>();
        if (highlight)
        {
            highlight.highlighted = true;
        }

        if (previousCombatant)
        {
            var highlightPrev = previousCombatant.GetComponentInChildren<HighlightEffect>();
            if (highlightPrev)
            {
                highlightPrev.highlighted = false;
            }
        }

        turnOrderText.text = "Turn Order: <br>";
        for (int i = 0; i < turnOrder.Count; i++)
        {
            turnOrderText.text += $"{i + 1}. {turnOrder[i].Key.Name}<br>";
        }
    }

    public void EnemyJoinedCombat(EnemyStats enemyStats)
    {
        SFXPlayer.Instance.PlaySFX(enemyJoinedClip, enemyStats.transform.position, 1f, 1f, 10, false, false);
        EnemyManager.Instance.EnemiesInCombat.Add(enemyStats);
        turnOrderList.Add(new KeyValuePair<BaseStats, int>(enemyStats, enemyStats.wits));
        enemyStats.InCombat = true;
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