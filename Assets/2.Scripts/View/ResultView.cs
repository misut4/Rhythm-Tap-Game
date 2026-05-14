using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultView : MonoBehaviour
{
    public TextMeshProUGUI txtResult;
    public TextMeshProUGUI txtScore;
    public Button btnPlay;
    public Button btnQuit;

    public Image starLeft;
    public Image starRight;
    public Image starCenter;
    public Transform mainPanel;
    public Transform blur;


    private float _mainPanelYHidden = 1100f;
    private float _mainPanelYShown = 64f;
    private float _tweenDuration = 1f;
    private Color _goldStarColor = ColorUtility.TryParseHtmlString("#FFD74E", out Color color) ? color : Color.white;

    private bool _isLeftStarShown = false;
    private bool _isCenterStarShown = false;
    private bool _isRightStarShown = false;

    void Awake()
    {
        btnPlay.onClick.AddListener(OnBtnPlayClick);
        btnQuit.onClick.AddListener(OnBtnQuitClick);

        GameManager.Instance.OnGameOver += OnGameOver;
    }

    void OnDestroy()
    {
        GameManager.Instance.OnGameOver -= OnGameOver;
    }

    private void OnGameOver()
    {
        ShowResult();
    }

    private void OnBtnPlayClick()
    {
        mainPanel.DOLocalMoveY(_mainPanelYHidden, _tweenDuration)
        .SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            blur.gameObject.SetActive(false);
            GameManager.Instance.RestartGame();
        });
    }

    private void OnBtnQuitClick()
    {
        Application.Quit();
    }

    public void ShowResult()
    {
        blur.gameObject.SetActive(true);
        CalculateGoldStars();
        mainPanel.DOLocalMoveY(_mainPanelYShown, _tweenDuration)
        .SetEase(Ease.OutBounce)
        .OnComplete(() =>
        {
            txtResult.text = GameManager.Instance.CurrentState == GameState.Won ? "You Won!" : "You Lost!";
            txtScore.text = GameManager.Instance.Score.ToString();


        });
    }

    private void CalculateGoldStars()
    {
        switch (GameManager.Instance.CurrentState)
        {
            case GameState.Won:
                _isLeftStarShown = true;
                _isCenterStarShown = true;
                _isRightStarShown = true;
                break;
            case GameState.GameOver:
                _isLeftStarShown = true;
                _isCenterStarShown = GameManager.Instance.Score > 10000;
                _isRightStarShown = false;
                break;
            default:
                break;
        }

        starLeft.color = _isLeftStarShown ? _goldStarColor : Color.white;
        starCenter.color = _isCenterStarShown ? _goldStarColor : Color.white;
        starRight.color = _isRightStarShown ? _goldStarColor : Color.white;
    }
}
