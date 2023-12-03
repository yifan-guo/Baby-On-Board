using UnityEngine;

/// <summary>
/// Represents a scoring category, its target value, and whether the goal is to get a low or high value in this category
/// </summary>
public class ScoreCategory
{
    /// <summary>
    /// The display name to be shown for this score category in UI elements.
    /// </summary>
    public string displayName;

    /// <summary>
    /// The percent of the grade determined by the score category
    /// </summary>
    public int weightingPercent;

    /// <summary>
    /// The target value for this score category.
    /// </summary>
    public float targetValue;

    /// <summary>
    /// The current value for this score category
    /// </summary>
    public float currentValue = 0;

    /// <summary>
    /// Bool to decide if the target value should be high or low.
    /// When targetHigh is true (the default setting), a higher score is better.
    /// When targetHigh is false, a lower score is better.
    /// </summary>
    public bool targetHigh = true;

    /// <summary>
    /// This number is a penalty that is incurred for value above a non-zero score when the category wants a low score with a zero target.
    // Lower penalty number actually results in higher penalty since this number is multiplied by itself for each hit over the target.
    /// </summary>
    public float missZeroTargetPenaltyExponent = 0.95f;


    public int CalculateScore()
    {
        int score = 0;
        if (targetHigh)
        {
            if (currentValue >= targetValue)
            {
                score = weightingPercent;
            }
            else
            {
                score = Mathf.RoundToInt(weightingPercent * (currentValue / targetValue));
            }
        }
        else
        {
            if (currentValue <= targetValue)
            {
                score = weightingPercent;
            }
            else
            {
                // need to be careful of dividing by zero, so we need to do some funny math to handle when the target value is zero
                if (targetValue == 0)
                {
                    // we'll raise a decimal to the power of the currentValue to simulate a penalty incurred for each hit
                    // when the target was to get 0 hits. Then multiply by the weight for the category.
                    // For example, with a penalty of 0.95 and a total number of 1 hits, and a category weight of 25%
                    // score = ( 0.95 ^ 1) * 25;
                    // score = 23.75 (24 in integer)
                    // Now if the player fails more times, the incur the penalty by raising the fraction to a power
                    // Penalty is 0.95, total hits is 4, category weight is 25
                    // score = (0.95 ^ 4) * 25;
                    // score = 20.36 (21 in integer)
                    score = Mathf.RoundToInt(weightingPercent * Mathf.Pow(missZeroTargetPenaltyExponent, currentValue));
                }
                else
                {
                    // Otherwise just divide the target by the current value
                    // E.g. when target is 20, current value is 23, and category weight is 25:
                    // score =  25 * (20/23)
                    // score = 21.73 (22 in integer)
                    // this doesn't work great for goals that are very low values like 1-5 since 1/2 == 50% and incurs a steep penalty
                    // potentially we can soften this in the future with a modifier value
                    score = Mathf.RoundToInt(weightingPercent * (targetValue) / currentValue);
                }

            }
        }
        return score;
    }

}
