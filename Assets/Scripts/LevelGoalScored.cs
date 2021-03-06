﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalScored : LevelGoal
{
    public override bool IsWinner()
    {
        if (ScoreManager.Instance != null)
        {
            return ScoreManager.Instance.CurrentScore >= scoreGoals[0];
        }
        return false;
    }

    public override bool IsGameOver()
    {
        return movesLeft <= 0;
    }
}
