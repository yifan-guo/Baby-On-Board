using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading;
using System.Linq;
/// <summary>
/// The ScoreManager class calculates the player's score from S (highest) to F (fail, lowest).
/// Upon level completion, invoke the ScoreManager with the scoring categories for the level to calculate the letter grade.
/// In the future the ScoreManager could subscribe to events from other components to keep a running tab of certain events.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>
    /// Performance ranked from highest to lowest. S-tier performance is above or equal to 100 points.
    /// F is any score between 0-60.
    /// </summary>
    public enum ScoringThresholds : int
    {
        S = 100,
        A = 90,
        B = 80,
        C = 70,
        D = 60,
        F = 0,
    }

    public int score { get; private set; }

    public string grade { get; private set; }

    public List<ScoreCategory> scoringCategories = new List<ScoreCategory>() {
        new ScoreCategory() { displayName = "Package Health", weightingPercent = 25, targetValue = 100},
        new ScoreCategory() { displayName = "Player Health", weightingPercent = 25, targetValue = 100},
        // Time-based zero-target score categories need a softer miss zero penalty, hence overriding the default to 0.999 here.
        new ScoreCategory() { displayName = "Completion Time", weightingPercent = 25, targetValue = 0, targetHigh = false, missZeroTargetPenaltyExponent=0.999f},
        new ScoreCategory() { displayName = "Police Pullovers", weightingPercent = 25, targetValue = 0, targetHigh = false},
    };

    public (int, string) CalculateGrade()
    {
        // Calculate the score by summing the scores of each category
        score = 0;
        int totalPercent = 0;
        foreach (ScoreCategory category in scoringCategories)
        {
            score += category.CalculateScore();
            // Reusing the same loop to check if our weight percents are valid
            totalPercent += category.weightingPercent;
        }

        // Check if weight percents are valid
        if (totalPercent != 100)
        {
            throw new ArgumentException("Category weighting percents do not add up to 100. Make sure to initialize scoringCategories with a total weightingPercent of 100.");
        }

        // Loop over the scoring thresholds and set grade and exit on the first case where the score is greater than the threshold
        foreach (string thresholdName in Enum.GetNames(typeof(ScoringThresholds)).Reverse())
        {
            int thresholdValue = (int)Enum.Parse(typeof(ScoringThresholds), thresholdName);
            if (score >= thresholdValue)
            {
                grade = thresholdName;
                break;
            }
        }

        return (score, grade);
    }
}
