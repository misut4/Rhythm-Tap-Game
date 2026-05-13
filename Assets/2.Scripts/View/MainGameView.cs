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
        GameManager.Instance.OnGameOver += OnGameOver;
        StartCoroutine(StartGameCoroutine());
    }

    private void OnTilePoolReady()
    {
        _isTilePoolReady = true;
    }

    private void OnTileSpawnerReady()
    {
        _isTileSpawnerReady = true;
    }

    private void OnGameOver()
    {
        resultView.ShowResult();
    }

    private IEnumerator StartGameCoroutine()
    {
        yield return new WaitUntil(() => _isTilePoolReady && _isTileSpawnerReady);
        GameManager.Instance.StartGame();
    }
}
