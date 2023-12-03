using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public SettingsMenu settingsMenu {get; private set;}

    /// <summary>
    /// Reference to the parent object for the Lose Screen.
    /// </summary>
    private GameObject losePopup;

    /// <summary>
    /// Reference to the parent object for the End Screen.
    /// </summary>
    public GameObject endScreen {get; private set;}


    /// <summary>
    /// Reference to the parent object for the Pull Over Screen.
    /// </summary>
    public GameObject pullOverScreen {get; private set;}

    /// <summary>
    /// Parent object for bandit indicators.
    /// </summary>
    public GameObject banditIndicators {get; private set;}

    public GameObject policeIndicators {get; private set;}

    /// <summary>
    /// Audio source for music.
    /// </summary>
    private AudioSource musicSource;

    public bool mainMusicPlaying = false;

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
    ///  <Reference to PoliceIndicator Prefab that will be cloned.
    /// </summary>
    public PoliceIndicator policeIndicatorPrefab;

    /// <summary>
    /// Reference to objective list entry Prefab that will be cloned.
    /// </summary>
    public ObjectiveEntry entryPrefab;

    /// <summary>
    /// Reference to Canvas component.
    /// </summary>
    public Canvas canvas { get; private set; }

    public ScoreManager sm {get; private set;}

    private GameObject hostileVehicleWarning;

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
        settingsMenu = transform.Find("SettingsMenu").GetComponent<SettingsMenu>();
        losePopup = transform.Find("LosePopup").gameObject;
        endScreen = transform.Find("EndScreen").gameObject;
        pullOverScreen = transform.Find("PulledOverScreen").gameObject;
        hostileVehicleWarning = transform.Find("HostileVehicleWarning").gameObject;

        musicSource = GetComponent<AudioSource>();
        mainMusicPlaying = true;

        sm = ScriptableObject.CreateInstance<ScoreManager>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        // Get this reference in Start because PlayerController instance is set in Awake
        banditIndicators = PlayerController.instance.transform.Find("BanditIndicators").gameObject;
        policeIndicators = PlayerController.instance.transform.Find("PoliceIndicators").gameObject;

        musicSource.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;

        settingsMenu.gameObject.SetActive(false);
        endScreen.SetActive(false);

        playerHP.Link(PlayerController.instance.hp);
        packageHP.Link(hp: PlayerController.instance.hp, pc: PlayerController.instance.pc);

        settingsMenu.OnMusicVolumeChanged += SetMusicVolume;
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
        PoliceIndicator.ClearAll();
    }

    /// <summary>
    /// Enable and disable the settings menu.
    /// </summary>
    public void ToggleSettingsMenu()
    {
        //disable this menu when game is over
        if(!PlayerController.instance.enableControl)
        {
            return;
        }

        GameState.instance.TogglePause();
        settingsMenu.gameObject.SetActive(GameState.instance.isPaused);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingsMenu.resumeButton.gameObject);

        if (settingsMenu.gameObject.activeInHierarchy)
        {
            settingsMenu.settings.SetActive(true);
            settingsMenu.controls.SetActive(false);
        }

        Cursor.visible = settingsMenu.gameObject.activeInHierarchy;
    }

    /// <summary>
    /// Show the win screen.
    /// </summary>
    public void DisplayWinScreen(){StartCoroutine(_DisplayWinScreen());}
    private IEnumerator _DisplayWinScreen()
    {
        yield return StartCoroutine(
            AuditLogger.instance.Finalize(AttemptReport.TerminatingState.win));

        Cursor.visible = true;
        SetWinText();
        endScreen.SetActive(true);
    }

    /// <summary>
    /// Show the lose screen.
    /// </summary>
    public void DisplayLoseScreen(){StartCoroutine(_DisplayLoseScreen());}
    private IEnumerator _DisplayLoseScreen()
    {
        yield return StartCoroutine(
            AuditLogger.instance.Finalize(AttemptReport.TerminatingState.lose));

        Cursor.visible = true;
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
            //disable player control so the player car can't move anymore because game is over
            PlayerController.instance.enableControl = false;
            //GameState.instance.TogglePause();
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
        sm.pkgHealthCategory,
        new ScoreCategory() { displayName = "Player Health", weightingPercent = 20, targetValue = 100, currentValue=(int)Math.Round(PlayerController.instance.hp.currentHealth)},
        new ScoreCategory() { displayName = "Completion Time", weightingPercent = 10, targetValue = 0, targetHigh = false, missZeroTargetPenaltyExponent=0.999f, currentValue=(int)Math.Round(levelObj.EndTime - levelObj.StartTime)}};

        sm.scoringCategories = scoringCategories;
        string scoreSummaryText = sm.GetScoreSummaryText();

        AuditLogger.instance.ar.finalLetterGrade = Enum.Parse<AttemptReport.FinalLetterGrade>(sm.grade);
        AuditLogger.instance.ar.finalNumberGrade = sm.score;
        
        // Get the TextMeshPro component with the name "WinText" in the children of EndScreen 
        Component[] textMeshes = endScreen.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        TMPro.TextMeshProUGUI winTextMesh = (TMPro.TextMeshProUGUI)Array.Find(textMeshes, tm => tm.name == "WinText");
        winTextMesh.text = scoreSummaryText;
    }
    
    /// <summary>
    /// Settings music audio bar slider listener.
    /// </summary>
    /// <param name="value"></param>
    private void SetMusicVolume(float value)
    {
        musicSource.volume = value;
    }

    public void PlayMusic()
    {
        if (mainMusicPlaying == false) {
            musicSource.Play();
        }
        mainMusicPlaying = true;
    }

    public void StopMusic()
    {
        musicSource.Stop();
        mainMusicPlaying = false;
    }

    public void ShowHostileVehicleWarning()
    {
        hostileVehicleWarning.SetActive(true);
    }

    public void HideHostileVehicleWarning()
    {
        hostileVehicleWarning.SetActive(false);
    }
}
