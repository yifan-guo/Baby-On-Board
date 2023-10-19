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
}