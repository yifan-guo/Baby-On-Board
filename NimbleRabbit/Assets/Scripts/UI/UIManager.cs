using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of UIManager.
    /// </summary>
    public static UIManager instance { get; private set; }

    [Header("Canvas References")]

    /// <summary>
    /// Reference to the player's health display.
    /// </summary>
    public HPDisplayUpdater playerHP;

    /// <summary>
    /// Reference to the package health display.
    /// </summary>
    public PackageHPDisplayUpdater packageHP;

    private IObjective level;

    public Transform ControllerLevel;

    /// <summary>
    /// Indicator pool parent object.
    /// </summary>
    public GameObject indicators;

    /// <summary>
    /// Reference to parent object for the Settings menu.
    /// </summary>
    public GameObject settingsMenu;

    /// <summary>
    /// Reference to the parent object for the Win Screen.
    /// </summary>
    public GameObject winPopup;

    /// <summary>
    /// Reference to the parent object for the Lose Screen.
    /// </summary>
    public GameObject losePopup;

    [Header("Non-Canvas References")]

    /// <summary>
    /// Parent object for bandit indicators.
    /// </summary>
    public GameObject banditIndicators;

    [Header("Prefabs")]

    /// <summary>
    /// Reference to Indicator Prefab that will be cloned.
    /// </summary>
    public Indicator indicatorPrefab;

    /// <summary>
    /// Reference to BanditIndicator Prefab that will be cloned.
    /// </summary>
    public BanditIndicator banditIndicatorPrefab;

    /// <summary>
    /// Reference to Canvas component.
    /// </summary>
    public Canvas canvas { get; private set; }

    public ScoreManager sm;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        instance = this;
        canvas = GetComponent<Canvas>();
        sm = new ScoreManager();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        settingsMenu.SetActive(false);
        winPopup.SetActive(false);

        playerHP.Link(PlayerController.instance.hp);
        packageHP.Link(hp: PlayerController.instance.hp, pc: PlayerController.instance.pc);

        PlayerController.instance.pc.OnInventoryChange += SubscribeToPackages;

        PlayerController.instance.hp.OnHealthChange += UpdateWinLoseDisplay;

        // interface objects are not visible in the Unity Editor, so the workaround is to
        // get the level from a Transform and assign it to the interface object
        level = (IObjective)ControllerLevel.GetComponent(typeof(IObjective));
        level.StartObjective();
    }

    /// <summary>
    /// Clean up UI for a fresh start.
    /// </summary>
    public void Restart()
    {
        Indicator.ClearAll();
        BanditIndicator.ClearAll();
    }

    /// <summary>
    /// Enable and disable the settings menu.
    /// </summary>
    public void ToggleSettingsMenu()
    {
        GameState.instance.TogglePause();
        settingsMenu.SetActive(GameState.instance.isPaused);
    }

    /// <summary>
    /// Show the win screen.
    /// </summary>
    public void DisplayWinScreen()
    {
        SetWinText();
        winPopup.SetActive(true);
    }

    /// <summary>
    /// Show the lose screen.
    /// </summary>
    public void DisplayLoseScreen()
    {
        losePopup.SetActive(true);
    }

    public void SubscribeToPackages()
    {
        foreach (Package pkg in PlayerController.instance.pc.packages)
        {
            pkg.OnObjectiveUpdated += UpdateWinLoseDisplay;
        }
    }

    /// <summary>
    /// UI Manager or GameState maintains a Level (Objective implementation)
    /// and listens to its failure and completion events.
    /// When a level is completed, it should display the win screen.
    /// When a level is failed, it should display the lose screen.
    /// </summary>
    public void UpdateWinLoseDisplay()
    {
        level.CheckCompletion();
        if (level.ObjectiveStatus == IObjective.Status.Complete)
        {
            DisplayWinScreen();
            GameState.instance.TogglePause();
            return;
        }
        level.CheckFailure();
        if (level.ObjectiveStatus == IObjective.Status.Failed) {
            DisplayLoseScreen();
            GameState.instance.TogglePause();
            return;
        }
    }

    public void SetWinText()
    {
        if (level.ObjectiveStatus != IObjective.Status.Complete)
        {
            throw new InvalidOperationException("Cannot get score for an incomplete objective.");
        }
        // Calculate score
        // I wanted to do this in the level class, but since it's an IObjective here I can't access the non-interface methods
        // TODO() refactor this later to make score calculation part of the level class
        List<ScoreCategory> scoringCategories = new List<ScoreCategory>() {
        new ScoreCategory() { displayName = "Package Health", weightingPercent = 40, targetValue = 100, currentValue=(int)Math.Round(PlayerController.instance.pc.packages[0].hp.currentHealth)},
        new ScoreCategory() { displayName = "Player Health", weightingPercent = 20, targetValue = 100, currentValue=(int)Math.Round(PlayerController.instance.hp.currentHealth)},
        new ScoreCategory() { displayName = "Completion Time", weightingPercent = 40, targetValue = 0, targetHigh = false, missZeroTargetPenaltyExponent=0.999f, currentValue=(int)Math.Round(level.EndTime - level.StartTime)},
        };
        sm.scoringCategories = scoringCategories;
        string scoreSummaryText = sm.GetScoreSummaryText();
        
        // Get the TextMeshPro component with the name "WinText" in the children of WinPopup
        Component[] textMeshes = winPopup.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        TMPro.TextMeshProUGUI winTextMesh = (TMPro.TextMeshProUGUI)Array.Find(textMeshes, tm => tm.name == "WinText");
        winTextMesh.text = scoreSummaryText;
    }
}
