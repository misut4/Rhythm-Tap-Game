using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

/// <summary>
/// Represents one falling tile.
/// Moves downward, reports Hit or Miss to GameManager, then returns itself to the pool.
/// </summary>
public class Tile : MonoBehaviour
{
    // Set by TilePool.Get()
    [HideInInspector] public int LaneIndex;

    private float _speed;
    private float _missY;
    private bool _isDead;      // consumed (hit or missed) — ignore further updates

    private ProceduralImage _image;
    private RectTransform _rectTransform;

    // ── Unity lifecycle ──────────────────────────────────────────────

    private void Awake()
    {
        _image = GetComponent<ProceduralImage>();
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (_isDead) return;

        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        // Move down
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        // Check miss boundary
        if (transform.position.y < _missY)
            Miss();
    }

    // ── Public API (called by TilePool and InputHandler) ─────────────

    /// TilePool calls this after positioning the tile.
    public void Activate(float speed, float missY, float tileWidth, float tileHeight)
    {
        _speed = speed;
        _missY = missY;
        _isDead = false;
        _image.color = Color.black;
        _rectTransform.sizeDelta = new Vector2(tileWidth, tileHeight);
        gameObject.SetActive(true);
    }

    /// InputHandler calls this when a tap lands on this tile.
    /// Returns true if the hit was valid (within hit window + not already dead).
    public bool TryHit()
    {
        if (_isDead) return false;

        // Optional: enforce a hit window so tapping too early does nothing.
        // Remove this block if you want taps to work anywhere on screen.
        // float tileY = transform.position.y;
        // if (tileY > _hitWindowHalfHeight) return false;   // too early

        Hit();
        return true;
    }

    // ── Internal ─────────────────────────────────────────────────────

    private void Hit()
    {
        _isDead = true;
        GameManager.Instance.RegisterHit(this);

        _image.DOFade(0.5f, 0.1f).OnComplete(() => {
            KillTile();
        });
    }

    private void Miss()
    {
        _isDead = true;
        GameManager.Instance.RegisterMiss(this);
    }

    public void KillTile()
    {
        Destroy(gameObject);
    }
}