using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of UIManager.
    /// </summary>
    public static UIManager instance {get; private set;}

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

    [Header("Prefabs")]

    /// <summary>
    /// Reference to Indicator Prefab that will be cloned.
    /// </summary>
    public Indicator indicatorPrefab;

    /// <summary>
    /// Reference to Canvas component.
    /// </summary>
    public Canvas canvas {get; private set;}

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        instance = this;
        canvas = GetComponent<Canvas>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        settingsMenu.SetActive(false);
        winPopup.SetActive(false);

        playerHP.Link(PlayerController.instance.hp);
        packageHP.Link(hp:PlayerController.instance.hp, pc:PlayerController.instance.pc);

        PlayerController.instance.pc.OnInventoryChange += SubscribeToPackages;

        // interface objects are not visible in the Unity Editor, so the workaround is to
        // get the level from a Transform and assign it to the interface object
        level = (IObjective) ControllerLevel.GetComponent(typeof(IObjective));
        level.StartObjective();
    }

    /// <summary>
    /// Clean up UI for a fresh start.
    /// </summary>
    public void Restart()
    {
        Indicator.ClearAll();
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
        winPopup.SetActive(true);
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
        if (level.ObjectiveStatus == IObjective.Status.Complete) {
            DisplayWinScreen();
            GameState.instance.TogglePause();
            return;
        } 
        level.CheckFailure();
        if (level.ObjectiveStatus == IObjective.Status.Failed) {
            // TODO() Display lose screen.
            Debug.Log("Lose screen placeholder.");
            return;
        }
    }
}
