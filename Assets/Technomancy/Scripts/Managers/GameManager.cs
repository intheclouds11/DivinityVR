using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HighlightPlus;
using HurricaneVR.Framework.Core.Utils;
using intheclouds;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
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

    private BaseStats _firstCombatant;
    private BaseStats _previousCombatant;
    private Coroutine _turnOrderCoroutine;
    private AudioSource _audioSource;

    private void Awake()
    {
        instance = this;
        _audioSource = GetComponent<AudioSource>();
        controlledPlayer = FindControlledPlayer();
    }

    private void Start()
    {
        UpdateGameState(GameState.Exploration);
    }

    private void Update()
    {
        // Resume music after other audiosource finished
        if (!_audioSource.isPlaying && MusicAudioSource.gameObject.activeInHierarchy && !MusicAudioSource.isPlaying)
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

        EnemyManager.instance.PopulateEnemiesInCombatList();
        foreach (var enemy in EnemyManager.instance.EnemiesInCombat)
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
        _turnOrderCoroutine = StartCoroutine(TurnOrderCoroutine());
    }

    private IEnumerator TurnOrderCoroutine()
    {
        while (true)
        {
            activeCombatant = turnOrderList[0].Key;
            UpdateTurnOrderText(turnOrderList);
            activeCombatant.Turn = true;
            NextTurn = false;

            if (!_firstCombatant)
            {
                _firstCombatant = activeCombatant;
            }
            else if (activeCombatant == _firstCombatant)
            {
                StartNewRound();
            }

            while (!NextTurn)
            {
                if (!players.Any() || !EnemyManager.instance.EnemiesInCombat.Any())
                {
                    EndCombat();
                }

                yield return null;
            }

            // Move current combatant to end of turnOrderList
            turnOrderList.Add(turnOrderList[0]);
            turnOrderList.Remove(turnOrderList[0]);
            
            _previousCombatant = activeCombatant;
            SFXPlayer.Instance.PlaySFX(nextTurnClip, LocalUserObjects.instance.ITCPlayerController.transform.position, 1f, 1f, 10, false, false);
        }
    }

    private static void StartNewRound()
    {
        SurfaceEffectsContainer.instance.Cooldown();
    }

    public void EndCombat()
    {
        if (!players.Any())
        {
            Debug.Log($"PLAYERS FELLED. RESTART FROM LAST SAVE");
            _audioSource.PlayOneShot(gameOverClip);
            MusicAudioSource.Stop();
            if (!UserMenu.instance.menuIsOpen)
            {
                UserMenu.instance.ToggleMenu(true);
            }
        }
        else
        {
            Debug.Log($"ENEMIES FELLED. EXITING COMBAT");
            _audioSource.PlayOneShot(combatEndClip);
            MusicAudioSource.Pause();
        }

        StopCoroutine(_turnOrderCoroutine);
        turnOrderUI.SetActive(false);
        turnOrderText.text = "";
        _firstCombatant = null;
        activeCombatant = null;

        foreach (var player in players)
        {
            player.InCombat = false;
        }

        foreach (var enemy in EnemyManager.instance.EnemiesInCombat)
        {
            enemy.InCombat = false;
            enemy.Turn = false;
        }
        
        UpdateGameState(GameState.Exploration);
    }

    public void UpdateTurnOrderText(List<KeyValuePair<BaseStats, int>> turnOrder)
    {
        var highlight = activeCombatant.GetComponentInChildren<HighlightEffect>();
        if (highlight)
        {
            highlight.highlighted = true;
        }

        if (_previousCombatant)
        {
            var highlightPrev = _previousCombatant.GetComponentInChildren<HighlightEffect>();
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
        EnemyManager.instance.EnemiesInCombat.Add(enemyStats);
        turnOrderList.Add(new KeyValuePair<BaseStats, int>(enemyStats, enemyStats.wits));
        UpdateTurnOrderText(turnOrderList);
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