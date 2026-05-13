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

    void Awake()
    {
        btnPlay.onClick.AddListener(OnBtnPlayClick);
        btnQuit.onClick.AddListener(OnBtnQuitClick);
    }

    private void OnBtnPlayClick()
    {
        mainPanel.DOLocalMoveY(_mainPanelYHidden, _tweenDuration)
        .SetEase(Ease.OutQuad)
        .OnComplete(() => {
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
        mainPanel.DOLocalMoveY(_mainPanelYShown, _tweenDuration)
        .SetEase(Ease.OutBounce)
        .OnComplete(() => {
            txtResult.text = GameManager.Instance.CurrentState == GameState.Won ? "You Won!" : "You Lost!";
            txtScore.text = GameManager.Instance.Score.ToString();
            starLeft.gameObject.SetActive(GameManager.Instance.CurrentState == GameState.Won);
            starRight.gameObject.SetActive(GameManager.Instance.CurrentState == GameState.Won);
            starCenter.gameObject.SetActive(GameManager.Instance.CurrentState == GameState.Won);
        });
    }
}
