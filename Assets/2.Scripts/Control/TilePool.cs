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

    [Header("References")]
    public GameConfig config;
    public Tile tilePrefab;
    public Transform tileParent;

    private ObjectPool<Tile> _pool;
    // ── Unity lifecycle ──────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _pool = new ObjectPool<Tile>(
            createFunc: () => CreateTile(),
            actionOnGet: (tile) => Get(),
            actionOnRelease: (tile) => Release(tile),
            actionOnDestroy: (tile) => DestroyTile(tile),
            collectionCheck: true,
            defaultCapacity: config.initialPoolSize,
            maxSize: config.initialPoolSize
        );

    }

    private void OnDestroy()
    {
    }

    public Tile Get()
    {
        return _pool.Get();
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
        tile.OnDeath();
    }
}