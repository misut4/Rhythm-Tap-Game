using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Simple object pool for Tile prefabs.
/// Pre-warms on Awake, grows automatically if needed.
/// </summary>
public class TilePool : MonoBehaviour
{
    public static TilePool Instance { get; private set; }

    public Tile tilePrefab;
    public RectTransform tileParent;
    private ObjectPool<Tile> _pool;
    private GameConfig _config;
    private int _lastLane = -1;
    private int _sameCount = 0;      // consecutive same-lane count
    // ── Unity lifecycle ──────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _config = GameManager.Instance.GetGameConfig();

        _pool = new ObjectPool<Tile>(
            createFunc: () => CreateTile(),
            actionOnGet: (tile) => { tile.gameObject.SetActive(true); },
            actionOnRelease: (tile) => Release(tile),
            actionOnDestroy: (tile) => DestroyTile(tile),
            collectionCheck: true,
            defaultCapacity: _config.initialPoolSize,
            maxSize: _config.initialPoolSize
        );

        _lastLane = -1;
        _sameCount = 0;

        GameManager.Instance.SetTilePoolReady();
    }

    private void OnDestroy()
    {
    }

    public void SpawnIn()
    {
        int lane = PickLane();

        Tile tile = _pool.Get();
        float laneWidth = tileParent.sizeDelta.x / _config.laneCount;

        tile.LaneIndex = lane;
        tile.Activate(_config.tileSpeed, _config.missY, laneWidth, _config.tileHeight);

        // Position: lane X, spawn Y, keep Z at 0
        tile.transform.localPosition = new Vector3(
            lane * laneWidth,
            0f,
            0f
        );
    }

    private void Release(Tile tile)
    {
        tile.gameObject.SetActive(false);
        _pool.Release(tile);
    }

    private Tile CreateTile()
    {
        Tile t = Instantiate(tilePrefab, tileParent);
        t.gameObject.SetActive(false);

        return t;
    }

    private void DestroyTile(Tile tile)
    {
        tile.KillTile();
    }

    /// Pick a lane index with playability rules.
    private int PickLane()
    {
        int newLane;
        do { newLane = Random.Range(0, _config.laneCount); }
        while (newLane == _lastLane);
        
        _lastLane = newLane;
        return newLane;
    }   
}