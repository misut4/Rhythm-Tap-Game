using UnityEngine;

/// <summary>
/// Spawns tiles on a BPM grid using AudioSettings.dspTime for drift-free timing.
///
/// Spawn pattern rules (can be extended):
///   - One tile per beat
///   - Never the same lane more than 2 times in a row
///   - Guarantees minimum hand distance for playability
/// </summary>
public class TileSpawner : MonoBehaviour
{
    [Header("References")]
    public GameConfig config;

    // ── State ────────────────────────────────────────────────────────

    private double _nextBeatDspTime;    // dspTime of the next scheduled spawn
    private bool _isRunning;

    // ── Unity lifecycle ──────────────────────────────────────────────

    private void OnEnable()
    {
        GameManager.Instance.OnGameStart += StartSpawning;
        GameManager.Instance.OnGameOver += StopSpawning;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnGameStart -= StartSpawning;
        GameManager.Instance.OnGameOver -= StopSpawning;
    }


    private void Update()
    {
        if (!_isRunning) return;

        // Keep spawning beats until we're caught up with current dsp time.
        // This handles any frames where dspTime jumped over a beat boundary.
        while (AudioSettings.dspTime >= _nextBeatDspTime)
        {
            SpawnTile();
            _nextBeatDspTime += config.BeatInterval;
        }
    }

    // ── Public API ───────────────────────────────────────────────────

    public void StartSpawning()
    {
        _isRunning = true;

        // Schedule first beat slightly ahead so the first tile isn't instant
        _nextBeatDspTime = AudioSettings.dspTime + config.BeatInterval;
    }

    public void StopSpawning()
    {
        _isRunning = false;
    }

    // ── Internal ─────────────────────────────────────────────────────

    private void SpawnTile()
    {
        TilePool.Instance.SpawnIn();
    }
}