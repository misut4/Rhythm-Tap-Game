using UnityEngine;

/// <summary>
/// Central config for tuning. Create via Assets > Create > MagicTiles > GameConfig.
/// Drag into GameManager and TileSpawner inspector fields.
/// </summary>
[CreateAssetMenu(menuName = "MagicTiles/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Music")]
    [Tooltip("BPM of the background track")]
    public float bpm = 120f;

    [Header("Tile Movement")]
    [Tooltip("Units per second tiles fall")]
    public float tileSpeed = 8f;

    [Header("Lane Setup")]
    public int laneCount = 4;
    [Tooltip("Total width all lanes occupy")]
    public float totalLaneWidth = 7.2f;

    [Header("Spawn / Despawn")]
    [Tooltip("World Y where tiles are created")]
    public float spawnY = 7f;
    [Tooltip("World Y where an untapped tile triggers a miss")]
    public float missY = -5.5f;

    [Header("Hit Window")]
    [Tooltip("Vertical tolerance (units) for a valid hit around the tile center")]
    public float hitWindowHalfHeight = 1.2f;

    [Header("Pooling")]
    public int initialPoolSize = 12;

    // ── Derived helpers ──────────────────────────────────────────────

    /// Seconds between beats
    public float BeatInterval => 60f / bpm;

    /// World X position for a given lane index (0-based)
    public float LaneX(int lane)
    {
        float laneWidth = totalLaneWidth / laneCount;
        float leftEdge = -totalLaneWidth / 2f + laneWidth / 2f;
        return leftEdge + lane * laneWidth;
    }

    /// Width of a single lane
    public float LaneWidth => totalLaneWidth / laneCount;
}