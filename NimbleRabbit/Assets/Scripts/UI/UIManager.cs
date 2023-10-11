using UnityEngine;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of UIManager.
    /// </summary>
    public static UIManager instance {get; private set;}

    /// <summary>
    /// Reference to parent object for the Settings menu.
    /// </summary>
    public GameObject settingsMenu;

    /// <summary>
    /// Reference to the parent object for the Win Screen.
    /// </summary>
    public GameObject winPopup;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        settingsMenu.SetActive(false);
        winPopup.SetActive(false);
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