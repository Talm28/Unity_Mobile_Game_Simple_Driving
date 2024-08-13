using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private float _scorePerSecond;

    public const string HighScoreKey = "HighScore";

    private float _score = 0;


    // Update is called once per frame
    void Update()
    {
        _score += _scorePerSecond * Time.deltaTime;
        _scoreText.text = Mathf.FloorToInt(_score).ToString();
    }

    void OnDestroy()
    {
        int currentHighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        if(_score > currentHighScore)
            PlayerPrefs.SetInt(HighScoreKey, Mathf.FloorToInt(_score));
    }
}
