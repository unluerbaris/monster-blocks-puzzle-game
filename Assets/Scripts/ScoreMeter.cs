using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ScoreMeter : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] ScoreStar[] scoreStars = new ScoreStar[3];
    LevelGoal levelGoal;
    int maxScore;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void SetupStars(LevelGoal levelGoal)
    {
        if (levelGoal == null)
        {
            Debug.LogWarning("Score meter invalid level goal!!!");
            return;
        }

        this.levelGoal = levelGoal;

        maxScore = this.levelGoal.scoreGoals[this.levelGoal.scoreGoals.Length - 1];
        float sliderWidth = slider.GetComponent<RectTransform>().rect.width;

        if (maxScore > 0)
        {
            for (int i = 0; i < levelGoal.scoreGoals.Length; i++)
            {
                if (scoreStars[i] != null)
                {
                    float newX = (sliderWidth * this.levelGoal.scoreGoals[i] / maxScore) - (sliderWidth * 0.5f);
                    RectTransform starRectTransform = scoreStars[i].GetComponent<RectTransform>();
                    if (starRectTransform != null)
                    {
                        starRectTransform.anchoredPosition = new Vector2(newX, starRectTransform.anchoredPosition.y);
                    }
                }
            }
        }
    }

    public void UpdateScoreMeter(int score, int starCount)
    {
        if (levelGoal != null)
        {
            slider.value = (float)score / (float)maxScore;
        }

        for (int i = 0; i < starCount; i++)
        {
            if (scoreStars[i] != null)
            {
                scoreStars[i].Activate();
            }
        }
    }
}
