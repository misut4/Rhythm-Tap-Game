using System;
using UnityEngine;

/// <summary>
/// Singleton that owns game state, score, and broadcasts events.
/// Other systems (Spawner, UI, VFX) subscribe rather than polling.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Config")]
    public GameConfig config;

    // ── Events ───────────────────────────────────────────────────────
    public event Action<int> OnScoreChanged;   // new score
    public event Action<Tile> OnTileHit;        // tile that was hit
    public event Action<Tile> OnTileMissed;     // tile that was missed
    public event Action OnGameOver;
    public event Action OnGameStart;

    // ── State ────────────────────────────────────────────────────────
    public GameState CurrentState { get; private set; } = GameState.Idle;
    public ComboType CurrentComboType { get; private set; } = ComboType.None;

    public int Score { get; private set; }

    // ── Unity lifecycle ──────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── Public API ───────────────────────────────────────────────────

    public void StartGame()
    {
        if (CurrentState == GameState.Playing) return;

        Debug.LogError("StartGame");
        Score = 0;
        CurrentState = GameState.Playing;
        OnScoreChanged?.Invoke(Score);
        OnGameStart?.Invoke();
    }

    public void RestartGame()
    {
        // Let TileSpawner/Pool clean themselves up via OnGameStarted
        StartGame();
    }

    /// Called by Tile when successfully tapped.
    public void RegisterHit(Tile tile)
    {
        if (CurrentState != GameState.Playing) return;

        int hitPoints = 0;

        switch (CurrentComboType)
        {
            case ComboType.Good:
                hitPoints = Constants.GOOD_SCORE;
                break;
            case ComboType.Great:
                hitPoints = Constants.GREAT_SCORE;
                break;
            case ComboType.Perfect:
                hitPoints = Constants.PERFECT_SCORE;
                break;
        }

        Score += hitPoints;
        OnScoreChanged?.Invoke(Score);
        OnTileHit?.Invoke(tile);
    }

    /// Called by Tile when it passes missY untapped.
    public void RegisterMiss(Tile tile)
    {
        if (CurrentState != GameState.Playing) return;

        OnTileMissed?.Invoke(tile);
        TriggerGameOver();
    }

    // ── Internal ─────────────────────────────────────────────────────

    private void TriggerGameOver()
    {
        CurrentState = GameState.GameOver;
        OnGameOver?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartGame();
        }
    }
}