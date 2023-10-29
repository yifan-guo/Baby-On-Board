using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of UIManager.
    /// </summary>
    public static UIManager instance { get; private set; }

    /// <summary>
    /// Reference to the player's health display.
    /// </summary>
    private HPDisplayUpdater playerHP;

    /// <summary>
    /// Reference to the package health display.
    /// </summary>
    private PackageHPDisplayUpdater packageHP;

    /// <summary>
    /// Reference to the objective list.
    /// </summary>
    public ObjectivesList objList {get; private set;}

    /// <summary>
    /// Indicator pool parent object.
    /// </summary>
    public GameObject indicators {get; private set;}

    /// <summary>
    /// Reference to parent object for the Settings menu.
    /// </summary>
    public GameObject settingsMenu {get; private set;}
    /// <summary>
    /// Reference to the parent object for the Lose Screen.
    /// </summary>
    private GameObject losePopup;

    /// <summary>
    /// Reference to the parent object for the End Screen.
    /// </summary>
    public GameObject endScreen {get; private set;}

    /// <summary>
    /// Parent object for bandit indicators.
    /// </summary>
    public GameObject banditIndicators {get; private set;}

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
    /// Reference to objective list entry Prefab that will be cloned.
    /// </summary>
    public ObjectiveEntry entryPrefab;

    /// <summary>
    /// Reference to Canvas component.
    /// </summary>
    public Canvas canvas { get; private set; }

    public ScoreManager sm {get; private set;}

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        instance = this;
        canvas = GetComponent<Canvas>();
        playerHP = transform.Find("PlayerHP").GetComponent<HPDisplayUpdater>();
        packageHP = transform.Find("PackageHPAnchor").GetComponent<PackageHPDisplayUpdater>();
        objList = transform.Find("ObjectivesList").GetComponent<ObjectivesList>();
        indicators = transform.Find("Indicators").gameObject;
        settingsMenu = transform.Find("SettingsMenu").gameObject;
        losePopup = transform.Find("LosePopup").gameObject;
        endScreen = transform.Find("EndScreen").gameObject;

        sm = ScriptableObject.CreateInstance<ScoreManager>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        // Get this reference in Start because PlayerController instance is set in Awake
        banditIndicators = PlayerController.instance.transform.Find("BanditIndicators").gameObject;

        settingsMenu.SetActive(false);
        endScreen.SetActive(false);

        playerHP.Link(PlayerController.instance.hp);
        packageHP.Link(hp: PlayerController.instance.hp, pc: PlayerController.instance.pc);

        PlayerController.instance.pc.OnInventoryChange += SubscribeToPackages;
        PlayerController.instance.hp.OnHealthChange += UpdateWinLoseDisplay;
    }

    /// <summary>
    /// Clean up UI for a fresh start.
    /// </summary>
    public void Restart()
    {
        Indicator.ClearAll();
        BanditIndicator.ClearAll();
        ObjectivesList.ClearAll();
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
        endScreen.SetActive(true);
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
        // TODO:
        // Pretty sure this will add repeats of the listener if the player loses
        // and then regains the package
        foreach (Package pkg in PlayerController.instance.pc.packages)
        {
            pkg.OnObjectiveUpdated += UpdateWinLoseDisplay;
        }
    }

    /// <summary>
    /// GameState maintains a Level (Objective implementation)
    /// and listens to its failure and completion events.
    /// When a level is completed, it should display the win screen.
    /// When a level is failed, it should display the lose screen.
    /// </summary>
    public void UpdateWinLoseDisplay()
    {
        // TODO:
        // Move some of this code to GameState, leave only UI code here

        IObjective levelObj = (IObjective) GameState.instance.level;
        levelObj.CheckCompletion();

        if (levelObj.ObjectiveStatus == IObjective.Status.Complete) 
        {
            DisplayWinScreen();
            GameState.instance.TogglePause();
            return;
        } 

        levelObj.CheckFailure();

        if (levelObj.ObjectiveStatus == IObjective.Status.Failed) 
        {
            DisplayLoseScreen();
            GameState.instance.TogglePause();
            return;
        }
    }

    public void SetWinText()
    {
        IObjective levelObj = (IObjective) GameState.instance.level;

        if (levelObj.ObjectiveStatus != IObjective.Status.Complete)
        {
            throw new InvalidOperationException("Cannot get score for an incomplete objective.");
        }
        // Calculate score
        // I wanted to do this in the level class, but since it's an IObjective here I can't access the non-interface methods
        // TODO() refactor this later to make score calculation part of the level class
        List<ScoreCategory> scoringCategories = new List<ScoreCategory>() {
        new ScoreCategory() { displayName = "Package Health", weightingPercent = 40, targetValue = 100, currentValue=(int)Math.Round(PlayerController.instance.pc.packages[0].hp.currentHealth)},
        new ScoreCategory() { displayName = "Player Health", weightingPercent = 20, targetValue = 100, currentValue=(int)Math.Round(PlayerController.instance.hp.currentHealth)},
        new ScoreCategory() { displayName = "Completion Time", weightingPercent = 40, targetValue = 0, targetHigh = false, missZeroTargetPenaltyExponent=0.999f, currentValue=(int)Math.Round(levelObj.EndTime - levelObj.StartTime)}};

        sm.scoringCategories = scoringCategories;
        string scoreSummaryText = sm.GetScoreSummaryText();
        
        // Get the TextMeshPro component with the name "WinText" in the children of EndScreen 
        Component[] textMeshes = endScreen.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        TMPro.TextMeshProUGUI winTextMesh = (TMPro.TextMeshProUGUI)Array.Find(textMeshes, tm => tm.name == "WinText");
        winTextMesh.text = scoreSummaryText;
    }
}
