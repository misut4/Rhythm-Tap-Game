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
    // ── State ────────────────────────────────────────────────────────

    private float _nextBeatDspTime;    // dspTime of the next scheduled spawn
    private bool _isRunning;
    private AudioSource _audioSource;
    private GameConfig _config;

    // ── Unity lifecycle ──────────────────────────────────────────────

    private void Awake()
    {
        _audioSource = GameManager.Instance.audioSource;
        _config = GameManager.Instance.GetGameConfig();

        GameManager.Instance.OnGameStart += StartSpawning;
        GameManager.Instance.OnGameOver += StopSpawning;

        GameManager.Instance.SetTileSpawnerReady();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnGameStart -= StartSpawning;
        GameManager.Instance.OnGameOver -= StopSpawning;
    }


    private void Update()
    {
        if (!_isRunning) return;

        if (_isRunning && _audioSource != null && _audioSource.clip != null)
        {
            float clipLength = _audioSource.clip.length;
            float audioTime = _audioSource.time;

            while (_nextBeatDspTime <= clipLength && audioTime >= _nextBeatDspTime)
            {
                SpawnTile();
                _nextBeatDspTime += _config.beatInterval;
            }
        }
   
    }

    // ── Public API ───────────────────────────────────────────────────

    public void StartSpawning()
    {
        _isRunning = true;

        _nextBeatDspTime = _config.beatInterval;
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