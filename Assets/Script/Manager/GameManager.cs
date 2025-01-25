using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private TextMeshProUGUI _textHeight;
    private int _currentHeight;
    [SerializeField] private int _currentLevel = 1;
    [SerializeField] private List<Stage> _listStage; 
    private void Update()
    {
        SetHeight();
    }

    private void SetHeight()
    {
        _currentHeight = (int)_playerTransform.position.y;
        _textHeight.text = _currentHeight.ToString() + "M";

        CheckingHeight();
    }

    private void CheckingHeight()
    {
        if (_currentLevel >= _listStage.Count)
            return;

        if (_currentHeight >= _listStage[_currentLevel].heightRequiriment)
        {

            _currentLevel++;
            ChangeBackground(true);
        }else if(_currentLevel > 1 && _currentHeight <= _listStage[_currentLevel - 1].heightRequiriment)
        {
            _currentLevel--;
            ChangeBackground(false);
        }
    }

    private void ChangeBackground(bool isLevelUp)
    {

        if (isLevelUp)
        {
            _listStage[_currentLevel - 2].background.DOFade(0, 3f);
            _listStage[_currentLevel - 1].background.DOFade(1, 3f);
        }
        else
        {
            _listStage[_currentLevel].background.DOFade(0, 3f);
            _listStage[_currentLevel - 1].background.DOFade(1, 3f);
        }
    }

}

[System.Serializable]
public class Stage
{
    public int heightRequiriment;
    public CanvasGroup background;
}
