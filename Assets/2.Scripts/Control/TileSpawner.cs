using System.Collections;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    // ── State ────────────────────────────────────────────────────────

    private float _elapsedTime;    // dspTime of the next scheduled spawn
    private bool _isRunning;
    private AudioSource _audioSource;
    private GameConfig _config;
    private WaitForSeconds _wait;

    // ── Unity lifecycle ──────────────────────────────────────────────

    private void Awake()
    {
        _audioSource = GameManager.Instance.audioSource;
        _config = GameManager.Instance.GetGameConfig();
        _wait = new WaitForSeconds(_config.beatInterval);

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

    private IEnumerator SpawnTilesByInterval()
    {
        while (_isRunning)
        {
            yield return _wait;
            float clipLength = _audioSource.clip.length;
            if (_elapsedTime <= clipLength)
            {
                SpawnTile();
                _elapsedTime += Time.deltaTime;
            }
        }
    }

    public void StartSpawning()
    {
        _isRunning = true;
        StartCoroutine(SpawnTilesByInterval());
    }

    public void StopSpawning()
    {
        _isRunning = false;
    }

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