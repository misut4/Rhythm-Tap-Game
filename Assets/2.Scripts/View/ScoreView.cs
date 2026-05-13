using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreView : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI txtScore;
    public TextMeshProUGUI txtCombo;
    public TextMeshProUGUI txtComboCounter;

    private int _currentScore = 0;
    private ComboType _currentComboType = ComboType.None;
    private int _comboCounter = 0;

    void Start()
    {
        GameManager.Instance.OnScoreChanged += OnScoreChanged;
    }

    void OnDestroy() {
        GameManager.Instance.OnScoreChanged -= OnScoreChanged;
    }

    private void OnScoreChanged(int score, ComboType comboType) 
    {
        // SCORE
        txtScore.DOCounter(_currentScore, score, 1f);
        _currentScore = score;

        // COMBO
        if (comboType == ComboType.None)
        {
            txtCombo.gameObject.SetActive(false);
        }else{
            txtCombo.gameObject.SetActive(true);
            txtCombo.text = comboType.ToString();
        }

        // COMBO COUNTER
        if (_currentComboType == comboType) {
            _comboCounter++;
        } else {
            _comboCounter = 0;
        }
        
        _currentComboType = comboType;


        if (_comboCounter > 0) {
            txtComboCounter.gameObject.SetActive(true);
            txtComboCounter.text = $"x{_comboCounter}";
        } else {
            txtComboCounter.gameObject.SetActive(false);
        }
    }

}
