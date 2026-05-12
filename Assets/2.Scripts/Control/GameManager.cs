using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton that owns game state, score, and broadcasts events.
/// Other systems (Spawner, UI, VFX) subscribe rather than polling.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public AudioSource audioSource;
    public CanvasScaler canvasScaler;

    private GameConfig _config;

    // ── Events ───────────────────────────────────────────────────────
    public event Action<int> OnScoreChanged;   // new score
    public event Action<Tile> OnTileHit;        // tile that was hit
    public event Action<Tile> OnTileMissed;     // tile that was missed
    public event Action OnGameOver;
    public event Action OnGameStart;
    public event Action OnTilePoolReady;
    public event Action OnTileSpawnerReady;

    // ── State ────────────────────────────────────────────────────────
    public GameState CurrentState { get; private set; } = GameState.Idle;
    public ComboType CurrentComboType { get; private set; } = ComboType.None;

    public int Score { get; private set; }

    // ── Unity lifecycle ──────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        InitGameConfig();

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
        audioSource.Play();
    }

    public void RestartGame()
    {
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

    public float GetAudioTime()
    {
        return audioSource.clip.length;
    }

    public void SetTilePoolReady()
    {
        OnTilePoolReady?.Invoke();
    }

    public void SetTileSpawnerReady()
    {
        OnTileSpawnerReady?.Invoke();
    }

    private void InitGameConfig()
    {
        float scalingFactor = (float)Screen.width / canvasScaler.referenceResolution.x;

        _config = new GameConfig(bpm: 148f, heightScalingFactor: scalingFactor, tileHeight: 300f, laneCount: 4, missY: -5.5f, initialPoolSize: 12);
    }

    public GameConfig GetGameConfig()
    {
        return _config;
    }
}