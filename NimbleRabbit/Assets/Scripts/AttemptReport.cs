using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Random=System.Random;

public class AttemptReport
{

    /// <summary>
    /// The Unique ID for the session (spans multiple attempts)
    /// </summary>
    [JsonProperty("entry.1074061825")]
    public string sessionID;

    /// <summary>
    /// The Unique ID for the attempt.
    /// </summary>
    [JsonProperty("entry.1706035046")]
    public string attemptID;

    /// <summary>
    /// Average speed the player was moving during a single attempt (collected every two seconds)
    /// </summary>
    [JsonProperty("entry.470235340")]
    public float avgSpeed;

    /// <summary>
    /// Number of healing collectibles picked up
    /// </summary>
    [JsonProperty("entry.1762866937")]
    public int numHealthCollectiblesConsumed;

    /// <summary>
    /// Total time the player spent pressing forward or right trigger.
    /// </summary>
    [JsonProperty("entry.1536362842")]
    public float timeSpentAccelerating;

    /// <summary>
    /// Total time the player spent pressing backward or left trigger.
    /// </summary>
    [JsonProperty("entry.1485903512")]
    public float timeSpentDecelerating;

    /// <summary>
    /// The type of controls the player used most.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InputType { keyboard, controller};

    [JsonProperty("entry.169029183")]
    public InputType inputType;

    /// <summary>
    /// The number of packages destroyed.
    /// </summary>
    [JsonProperty("entry.991958")]
    public int numDestroyedPackages;

    /// <summary>
    /// The number of packages the player reclaimed.
    /// </summary>
    [JsonProperty("entry.1556313048")]
    public int numReclaimedPackages;

    /// <summary>
    /// The final letter grade achieved in the attempt. Null if not a win.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FinalLetterGrade { S, A, B, C, D, F, I};

    [JsonProperty("entry.21026437")]
    public FinalLetterGrade finalLetterGrade = FinalLetterGrade.I;

    /// <summary>
    /// The final number grade 0-100 achieved by the player.
    /// </summary>
    [JsonProperty("entry.2083044711")]
    public int finalNumberGrade = -1;

    /// <summary>
    /// The health value of the package at the end of the attempt.
    /// </summary>
    [JsonProperty("entry.482325542")]
    public float finalPackageHealth;

    /// <summary>
    /// The health value of the player at the end of the attempt.
    /// </summary>
    [JsonProperty("entry.1167282296")]
    public float finalPlayerHealth;

    /// <summary>
    /// The total duration of the attempt from game load to termination.
    /// </summary>
    [JsonProperty("entry.172320985")]
    public float attemptDurationSeconds;

    /// <summary>
    /// The total number of times the bandit stole the package.
    /// </summary>
    [JsonProperty("entry.789894942")]
    public int numBanditSteals;

    /// <summary>
    /// The total number of times the bandit chased the player.
    /// </summary>
    [JsonProperty("entry.1691245107")]
    public int numBanditChases;

    /// <summary>
    /// The total number of times the police chased the player.
    /// </summary>
    [JsonProperty("entry.427089485")]
    public int numPoliceChases;

    /// <summary>
    /// The total number of times the player collided with a civilian NPC.
    /// </summary>
    [JsonProperty("entry.2092209994")]
    public int numCivilianCollisions;

    /// <summary>
    /// The total number of times the player collided with a bandit NPC.
    /// </summary>
    [JsonProperty("entry.1088033488")]
    public int numBanditCollisions;

    /// <summary>
    /// The total number of times the player was pulled over by a police NPC.
    /// </summary>
    [JsonProperty("entry.900801655")]
    public int numTimesPulledOver;

    /// <summary>
    /// The final volume setting for music.
    /// </summary>
    [JsonProperty("entry.1403845558")]
    public float finalMusicVolume;

    /// <summary>
    /// The final volume setting for sound effects.
    /// </summary>
    [JsonProperty("entry.79484810")]
    public float finalSFXVolume;

    /// <summary>
    /// The final state of the game at termination: win, lose, quit, restart.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TerminatingState { win, lose, quit, restart };

    [JsonProperty("entry.1872382336")]
    public TerminatingState terminatingState;

    /// <summary>
    /// The average health of the player over the duration of the attempt (collected every two seconds)
    /// </summary>
    [JsonProperty("entry.1317096085")]
    public float avgPlayerHealth;

    /// <summary>
    /// The total number of collisions between the player and any object in the game.
    /// </summary>
    [JsonProperty("entry.2028536741")]
    public int numCollisions;

    /// <summary>
    /// The total number of damage taken by the player during the attempt irrespective of any healing.
    /// </summary>
    [JsonProperty("entry.690325059")]
    public float totalDamageTaken;

    /// <summary>
    /// The total number of objects destroyed by the player during the attempt.
    /// </summary>
    [JsonProperty("entry.1378110127")]
    public int numObjectsDestroyed;

    /// <summary>
    /// The total number of times the player viewed the controls page in the settings menu during the attempt.
    /// </summary>
    [JsonProperty("entry.753705781")]
    public int numControlViews;

    /// <summary>
    /// 2D array representing the number of time the player spent in each section of the map.
    /// </summary>
    [JsonProperty("entry.655076955")]
    public float[,] playerHeatMap = new float[6,6]
    {
        {0f, 0f, 0f, 0f, 0f, 0f},
        {0f, 0f, 0f, 0f, 0f, 0f},
        {0f, 0f, 0f, 0f, 0f, 0f},
        {0f, 0f, 0f, 0f, 0f, 0f},
        {0f, 0f, 0f, 0f, 0f, 0f},
        {0f, 0f, 0f, 0f, 0f, 0f}
    };

    public AttemptReport()
    {
        Random rng = new Random();

        // See if they already had a cached sessionID
        sessionID = PlayerPrefs.GetString(
            "sessionID",
            "");
        
        // If not, generate one and cache it
        if (sessionID == "")
        {
            sessionID = $"{rng.Next()}";
            PlayerPrefs.SetString(
                "sessionID",
                sessionID);
        }

        attemptID = $"{rng.Next()}";
    }

    /// <summary>
    /// Example serialization method, can be removed later and just use JsonUtility.ToJson directly in caller class.
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, dynamic> ToDict()
    {
        string jsonString = JsonConvert.SerializeObject(this);
        Debug.Log(jsonString);

        Dictionary<string, dynamic> obj = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonString);

        return obj;
    }

}