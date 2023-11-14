using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class AttemptReport
{

    /// <summary>
    /// The Unique ID for the session (spans multiple attempts)
    /// </summary>
    public string sessionID;

    /// <summary>
    /// The Unique ID for the attempt.
    /// </summary>
    public string attemptID;

    /// <summary>
    /// Average speed the player was moving during a single attempt (collected every two seconds)
    /// </summary>
    public float avgSpeed;

    /// <summary>
    /// Number of healing collectibles picked up
    /// </summary>
    public int numHealthCollectiblesConsumed;

    /// <summary>
    /// Total time the player spent pressing forward or right trigger.
    /// </summary>
    public float timeSpentAccelerating;

    /// <summary>
    /// Total time the player spent pressing backward or left trigger.
    /// </summary>
    public float timeSpentDecelerating;

    /// <summary>
    /// The type of controls the player used most.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InputType { keyboard, controller};

    public InputType inputType;

    /// <summary>
    /// The number of packages destroyed.
    /// </summary>
    public int numDestroyedPackages;

    /// <summary>
    /// The number of packages the player reclaimed.
    /// </summary>
    public int numReclaimedPackages;

    /// <summary>
    /// The final letter grade achieved in the attempt. Null if not a win.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FinalLetterGrade { S, A, B, C, D, F};

    public FinalLetterGrade finalLetterGrade;

    /// <summary>
    /// The final number grade 0-100 achieved by the player.
    /// </summary>
    public int finalNumberGrade;

    /// <summary>
    /// The health value of the package at the end of the attempt.
    /// </summary>
    public float finalPackageHealth;

    /// <summary>
    /// The health value of the player at the end of the attempt.
    /// </summary>
    public float finalPlayerHealth;

    /// <summary>
    /// The total duration of the attempt from game load to termination.
    /// </summary>
    public float attemptDurationSeconds;

    /// <summary>
    /// The total number of times the bandit stole the package.
    /// </summary>
    public int numBanditSteals;

    /// <summary>
    /// The total number of times the bandit chased the player.
    /// </summary>
    public int numBanditChases;

    /// <summary>
    /// The total number of times the police chased the player.
    /// </summary>
    public int numPoliceChases;

    /// <summary>
    /// The total number of times the player collided with a civilian NPC.
    /// </summary>
    public int numCivilianCollisions;

    /// <summary>
    /// The total number of times the player collided with a bandit NPC.
    /// </summary>
    public int numBanditCollisions;

    /// <summary>
    /// The total number of times the player was pulled over by a police NPC.
    /// </summary>
    public int numTimesPulledOver;

    /// <summary>
    /// The final volume setting for music.
    /// </summary>
    public float finalMusicVolume;

    /// <summary>
    /// The final volume setting for sound effects.
    /// </summary>
    public float finalSFXVolume;

    /// <summary>
    /// The final state of the game at termination: win, lose, quit, restart.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TerminatingState { win, lose, quit, restart };

    public TerminatingState terminatingState;

    /// <summary>
    /// The average health of the player over the duration of the attempt (collected every two seconds)
    /// </summary>
    public float avgPlayerHealth;

    /// <summary>
    /// The total number of collisions between the player and any object in the game.
    /// </summary>
    public int numCollisions;

    /// <summary>
    /// The total number of damage taken by the player during the attempt irrespective of any healing.
    /// </summary>
    public float totalDamageTaken;


    /// <summary>
    /// The total number of objects destroyed by the player during the attempt.
    /// </summary>
    public int numObjectsDestroyed;

    /// <summary>
    /// The total number of times the player viewed the controls page in the settings menu during the attempt.
    /// </summary>
    public int numControlViews;

    /// <summary>
    /// 2D array representing the number of time the player spent in each section of the map.
    /// </summary>
    public float[][] playerHeatMap;

    /// <summary>
    /// Example serialization method, can be removed later and just use JsonUtility.ToJson directly in caller class.
    /// </summary>
    /// <returns></returns>
    public string Serialize()
    {
        string jsonString = JsonConvert.SerializeObject(this);
        Debug.Log(jsonString);
        return jsonString;
    }

}