using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;

public class AuditLogger : MonoBehaviour
{
    public static AuditLogger instance {get; private set;} // Singleton

    public LogMessenger lm {get; private set;}

    public AttemptReport ar {get; private set;}

    private List<float> speeds;

    private List<float> playerHealths;

    private bool reportSent;

    /// <summary>
    /// Recordings per second.
    /// Only needed for things where we record constantly (avg speed, hp, etc.)
    /// </summary>
    public const float FREQ_S = 1f;

    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        ar = new AttemptReport();
        lm = GetComponent<LogMessenger>();

        speeds = new List<float>();
        playerHealths = new List<float>();
    }

    public void RecordPlayerData()
    {
        PlayerController player = PlayerController.instance;
        float speed = player.rb.velocity.magnitude;
        float hp = player.hp.currentHealth;

        speeds.Add(speed);
        playerHealths.Add(hp);
    }

    public void RecordHeatMapValue(
        int row, 
        int col, 
        float time_s)
    {
        ar.playerHeatMap[row, col] += time_s;
    }

    void CalculatePackageAverageHealth()
    {
        List<Package> packages = FindObjectsOfType<Package>().ToList();
        float totalHealth = 0f;
        foreach (Package pkg in packages)
        {
            totalHealth += pkg.hp.currentHealth;
        }
        ar.finalPackageHealth = totalHealth / packages.Count;
    }


    /// <summary>
    /// Finalize metrics.
    /// </summary>
    public IEnumerator Finalize(AttemptReport.TerminatingState state)
    {
        ar.avgSpeed = speeds.Count > 0 ? (float) speeds.Average() : 0.0f;
        ar.avgPlayerHealth = playerHealths.Count > 0 ? (float) playerHealths.Average() : 0.0f;
        ar.finalMusicVolume = UIManager.instance.settingsMenu.musicSlider.value;
        ar.finalSFXVolume = UIManager.instance.settingsMenu.soundsSlider.value;
        ar.finalPlayerHealth = PlayerController.instance.hp.currentHealth;
        CalculatePackageAverageHealth();
        ar.attemptDurationSeconds = Time.time - GameState.instance.start_s;
        ar.terminatingState = state; 

        yield return StartCoroutine(lm.Post(ar));
    }
}