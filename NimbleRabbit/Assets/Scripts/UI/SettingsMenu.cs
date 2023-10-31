using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    /// <summary>
    /// Reference to Close "X" button.
    /// </summary>
    public Button closeButton;

    /// <summary>
    /// Reference to Music volume slider.
    /// </summary>
    public Slider musicSlider;

    /// <summary>
    /// Reference to Sounds volume slider.
    /// </summary>
    public Slider soundsSlider;

    /// <summary>
    /// Reference to Resume button.
    /// </summary>
    public Button resumeButton;

    /// <summary>
    /// Reference to Restart button.
    /// </summary>
    public Button restartButton;

    /// <summary>
    /// Reference to Quit button.
    /// </summary>
    public Button quitButton;

    /// <summary>
    /// Reference to the Controls button.
    ///</summary
    public Button controlsButton;

    /// <summary>
    /// Reference to the Controls Back button.
    ///</summary
    public Button controlsBackButton;

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        // TODO:
        // Add listeners for restart/quit buttons.
        // Need to add a GameState class that manages pausing, quitting, etc.

        musicSlider.onValueChanged.AddListener(AdjustMusicVolume);
        soundsSlider.onValueChanged.AddListener(AdjustSoundsVolume);
        closeButton.onClick.AddListener(UIManager.instance.ToggleSettingsMenu);
        resumeButton.onClick.AddListener(UIManager.instance.ToggleSettingsMenu);
        restartButton.onClick.AddListener(GameState.instance.Restart);
        quitButton.onClick.AddListener(GameState.instance.Quit);
        controlsButton.onClick.AddListener(GameState.instance.Controls);
        controlsBackButton.onClick.AddListener(GameState.instance.Controls);
    }

    /// <summary>
    /// Changes music volume accordingly.
    /// </summary>
    private void AdjustMusicVolume(float value)
    {
        // TODO:
        // Implement and maybe save PlayerPrefs data
    }    

    /// <summary>
    /// Changes sounds volume accordingly.
    /// </summary>
    private void AdjustSoundsVolume(float value)
    {
        // TODO:
        // Implement and maybe save PlayerPrefs data
    }
}