using UnityEngine;

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
        GameManager.Instance.OnClearTiles += ClearTiles;

        GameManager.Instance.SetTileSpawnerReady();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnGameStart -= StartSpawning;
        GameManager.Instance.OnGameOver -= StopSpawning;
        GameManager.Instance.OnClearTiles -= ClearTiles;
    }


    private void Update()
    {
        if (!_isRunning) return;

        if (_isRunning && _audioSource != null && _audioSource.clip != null)
        {
            float clipLength = _audioSource.clip.length;
            float audioTime = _audioSource.time;

            if (_nextBeatDspTime <= clipLength && audioTime >= _nextBeatDspTime)
            {
                SpawnTile();
                _nextBeatDspTime += _config.beatInterval;
            }
            else
            {
                // we won
            }
        }
   
    }

    // ── Public API ───────────────────────────────────────────────────

    public void StartSpawning()
    {
        _isRunning = true;
    } 

    public void StopSpawning()
    {
        _isRunning = false;
        _nextBeatDspTime = _config.beatInterval;
    }

    // ── Internal ─────────────────────────────────────────────────────

    private void SpawnTile()
    {
        TilePool.Instance.SpawnIn();
    }

    private void ClearTiles()
    {
        foreach (var tile in transform.GetComponentsInChildren<Tile>())
        {
            tile.KillTile();
        }
    }
}