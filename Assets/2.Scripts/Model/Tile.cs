using DG.Tweening;
using Solo.MOST_IN_ONE;
using UnityEngine;
using UnityEngine.UI.ProceduralImage;

public class Tile : MonoBehaviour
{
    [HideInInspector] public int LaneIndex;

    public ParticleSystem hitEffect;
    public ParticleSystem missEffect;

    private float _speed;
    private float _missY;
    private bool _isDead;

    private ProceduralImage _image;
    private RectTransform _rectTransform;


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

    public void Activate(float speed, float missY, float tileWidth, float tileHeight)
    {
        _speed = speed;
        _missY = missY;
        _isDead = false;
        _image.color = Color.black;
        _rectTransform.sizeDelta = new Vector2(tileWidth, tileHeight);
        gameObject.SetActive(true);
    }

    public bool TryHit()
    {
        if (_isDead) return false;

        Hit();
        return true;
    }

    private void Hit()
    {
        _isDead = true;
        GameManager.Instance.RegisterHit(this);

        // VIBRATE FEEDBACK
        MOST_HapticFeedback.Generate(MOST_HapticFeedback.HapticTypes.LightImpact);

        // PLAY HIT EFFECT
        hitEffect.gameObject.SetActive(true);
        hitEffect.Play();

        // FADE OUT TILE
        _image.DOFade(0f, hitEffect.main.duration).OnComplete(() =>
        {
            Release();
        });
    }

    private void Miss()
    {
        _isDead = true;

        // PLAY MISS EFFECT
        missEffect.gameObject.SetActive(true);
        missEffect.Play();

        GameManager.Instance.RegisterMiss(this);
    }

    public void Release()
    {
        TilePool.Instance.Release(this);
    }

    public void OnReturnedToPool()
    {
        _image.DOKill();
        if (hitEffect != null)
        {
            hitEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            hitEffect.gameObject.SetActive(false);
        }
        if (missEffect != null)
        {
            missEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            missEffect.gameObject.SetActive(false);
        }
    }
}