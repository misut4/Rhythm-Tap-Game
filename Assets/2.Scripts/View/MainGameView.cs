using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameView : MonoBehaviour
{
    public ResultView resultView;
    private bool _isTilePoolReady = false;
    private bool _isTileSpawnerReady = false;

    private void Awake()
    {
        GameManager.Instance.OnTilePoolReady += OnTilePoolReady;
        GameManager.Instance.OnTileSpawnerReady += OnTileSpawnerReady;
        StartCoroutine(StartGameCoroutine());
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnTilePoolReady -= OnTilePoolReady;
        GameManager.Instance.OnTileSpawnerReady -= OnTileSpawnerReady;
    }

    private void OnTilePoolReady()
    {
        _isTilePoolReady = true;
    }

    private void OnTileSpawnerReady()
    {
        _isTileSpawnerReady = true;
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return new WaitUntil(() => _isTilePoolReady && _isTileSpawnerReady);
        GameManager.Instance.StartGame();
    }
}
