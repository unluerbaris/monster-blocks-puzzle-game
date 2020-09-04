﻿using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ScoreManager : Singleton<ScoreManager>
{
    int currentScore = 0;
    public int CurrentScore
    {
        get
        {
            return currentScore;
        }
    }

    int counterValue = 0;
    int increment = 5;

    public Text scoreText;

    void Start()
    {
        UpdateScoreText(currentScore);
    }

    public void UpdateScoreText(int scoreValue)
    {
        if (scoreText != null)
        {
            scoreText.text = scoreValue.ToString();
        }
    }

    public void AddScore(int value)
    {
        currentScore += value;
        StartCoroutine(CountScoreRoutine());
    }

    IEnumerator CountScoreRoutine()
    {
        int iterations = 0;

        while (counterValue < currentScore && iterations < 100000)
        {
            counterValue += increment;
            UpdateScoreText(counterValue);
            iterations++;
            yield return null;
        }

        counterValue = currentScore;
        UpdateScoreText(currentScore);
    }
}
