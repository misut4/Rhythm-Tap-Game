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
    public event Action<int, ComboType> OnScoreChanged;   // new score
    public event Action<Tile> OnTileHit;        // tile that was hit
    public event Action<Tile> OnTileMissed;     // tile that was missed
    public event Action OnGameOver;
    public event Action OnGameStart;
    public event Action OnTilePoolReady;
    public event Action OnTileSpawnerReady;
    public event Action OnClearTiles;

    // ── State ────────────────────────────────────────────────────────
    public GameState CurrentState { get; private set; } = GameState.Idle;
    public ComboType CurrentComboType { get; private set; } = ComboType.None;

    public int Score { get; private set; }
    private float elapsedTime = 0f;

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

        Score = 0;
        CurrentState = GameState.Playing;
        OnClearTiles?.Invoke();
        OnScoreChanged?.Invoke(Score, ComboType.None);
        OnGameStart?.Invoke();
        audioSource.time = 0f;
        StartCoroutine(WaitForAudioStartDelay());
        StartCoroutine(StartElapsedTime());
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

        CurrentComboType = CheckCombo();

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
        OnScoreChanged?.Invoke(Score, CurrentComboType);
        OnTileHit?.Invoke(tile);
    }

    private IEnumerator WaitForAudioStartDelay()
    {
        yield return new WaitForSeconds(_config.audioStartDelay);
        audioSource.Play();
    }

    private ComboType CheckCombo()
    {
        ComboType comboType = ComboType.None;

        if (elapsedTime < Constants.PERFECT_HIT_WINDOW)
        {
            comboType = ComboType.Perfect;
        }
        else if (elapsedTime < Constants.GREAT_HIT_WINDOW)
        {
            comboType = ComboType.Great;
        }
        else if (elapsedTime < Constants.GOOD_HIT_WINDOW)
        {
            comboType = ComboType.Good;
        }

        elapsedTime = 0f;
        return comboType;
    }

    private IEnumerator StartElapsedTime()
    {
        while (CurrentState == GameState.Playing)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    /// Called by Tile when it passes missY untapped.
    public void RegisterMiss(Tile tile)
    {
        if (CurrentState != GameState.Playing) return;

        OnTileMissed?.Invoke(tile);
        TriggerGameOver(GameState.GameOver);
    }

    // ── Internal ─────────────────────────────────────────────────────

    public void TriggerGameOver(GameState gameState)
    {
        CurrentState = gameState;
        audioSource.Stop();
        OnGameOver?.Invoke();
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

        float bpm = 148f;
        float beatInterval = 60f / bpm;
        float tileSpeed = 300f * scalingFactor / beatInterval;
        float audioStartDelay = (Screen.height - 300 * scalingFactor) / tileSpeed;

        Debug.LogError("audioStartDelay: " + audioStartDelay);

        _config = new GameConfig(bpm: bpm, tileSpeed: tileSpeed, tileHeight: 300f, laneCount: 4, missY: -5.5f, initialPoolSize: 12, beatInterval: beatInterval, audioStartDelay: audioStartDelay);
    }

    public GameConfig GetGameConfig()
    {
        return _config;
    }
}