using UnityEngine;
using UnityEngine.Pool;

public class TilePool : MonoBehaviour
{
    public static TilePool Instance { get; private set; }

    public Tile tilePrefab;
    public RectTransform tileParent;
    private ObjectPool<Tile> _pool;
    private GameConfig _config;
    private int _lastLane = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _config = GameManager.Instance.GetGameConfig();

        _pool = new ObjectPool<Tile>(
            createFunc: () => CreateTile(),
            actionOnGet: (tile) => { tile.gameObject.SetActive(true); },
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: (tile) => DestroyTile(tile),
            collectionCheck: true,
            defaultCapacity: _config.initialPoolSize,
            maxSize: _config.initialPoolSize
        );

        _lastLane = -1;

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

    public void Release(Tile tile)
    {
        if (tile == null) return;
        _pool.Release(tile);
    }

    public void ReleaseAll()
    {
        foreach (var tile in tileParent.GetComponentsInChildren<Tile>(true))
        {
            if (tile.gameObject.activeSelf)
                Release(tile);
        }
    }

    private void OnReturnedToPool(Tile tile)
    {
        tile.OnReturnedToPool();
        tile.gameObject.SetActive(false);
    }

    private Tile CreateTile()
    {
        Tile t = Instantiate(tilePrefab, tileParent);
        t.gameObject.SetActive(false);

        return t;
    }

    private void DestroyTile(Tile tile)
    {
        Destroy(tile.gameObject);
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