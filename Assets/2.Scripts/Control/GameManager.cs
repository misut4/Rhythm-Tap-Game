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

    // ── State ────────────────────────────────────────────────────────
    public enum State { Idle, Playing, GameOver }
    public State CurrentState { get; private set; } = State.Idle;

    public int Score { get; private set; }

    // ── Constants ────────────────────────────────────────────────────
    private const int HIT_POINTS = 100;

    // ── Unity lifecycle ──────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ── Public API ───────────────────────────────────────────────────

    public void StartGame()
    {
        if (CurrentState == State.Playing) return;

        Score = 0;
        CurrentState = State.Playing;
        OnScoreChanged?.Invoke(Score);
    }

    public void RestartGame()
    {
        // Let TileSpawner/Pool clean themselves up via OnGameStarted
        StartGame();
    }

    /// Called by Tile when successfully tapped.
    public void RegisterHit(Tile tile)
    {
        if (CurrentState != State.Playing) return;

        Score += HIT_POINTS;
        OnScoreChanged?.Invoke(Score);
        OnTileHit?.Invoke(tile);
    }

    /// Called by Tile when it passes missY untapped.
    public void RegisterMiss(Tile tile)
    {
        if (CurrentState != State.Playing) return;

        OnTileMissed?.Invoke(tile);
        TriggerGameOver();
    }

    // ── Internal ─────────────────────────────────────────────────────

    private void TriggerGameOver()
    {
        CurrentState = State.GameOver;
        OnGameOver?.Invoke();
    }
}