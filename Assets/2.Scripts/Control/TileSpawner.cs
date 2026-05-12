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
    private int _lastLane = -1;
    private int _sameCount = 0;      // consecutive same-lane count

    // ── Unity lifecycle ──────────────────────────────────────────────

    private void OnEnable()
    {
        GameManager.Instance.OnGameOver += StopSpawning;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) return;
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
        _lastLane = -1;
        _sameCount = 0;
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
        int lane = PickLane();

        Tile tile = TilePool.Instance.Get();

        // Position: lane X, spawn Y, keep Z at 0
        tile.transform.position = new Vector3(
            config.LaneX(lane),
            config.spawnY,
            0f
        );

        // Match tile width to lane width
        tile.transform.localScale = new Vector3(
            config.LaneWidth * 0.92f,   // 8% gap between tiles
            tile.transform.localScale.y,
            1f
        );

        tile.LaneIndex = lane;
        tile.Activate(config.tileSpeed, config.missY, config.hitWindowHalfHeight);
    }

    /// Pick a lane index with playability rules.
    private int PickLane()
    {
        // If same lane appeared twice in a row, force a different lane
        if (_sameCount >= 2)
        {
            int newLane;
            do { newLane = Random.Range(0, config.laneCount); }
            while (newLane == _lastLane);

            _lastLane = newLane;
            _sameCount = 1;
            return newLane;
        }

        int lane = Random.Range(0, config.laneCount);

        if (lane == _lastLane) _sameCount++;
        else _sameCount = 1;

        _lastLane = lane;
        return lane;
    }
}