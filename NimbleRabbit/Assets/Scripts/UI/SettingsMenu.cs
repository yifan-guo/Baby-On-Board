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
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        // TODO:
        // Add listeners for restart/quit buttons.
        // Need to add a GameState class that manages pausing, quitting, etc.

        closeButton.onClick.AddListener(PlayerController.instance.TogglePause);
        resumeButton.onClick.AddListener(PlayerController.instance.TogglePause);
        musicSlider.onValueChanged.AddListener(AdjustMusicVolume);
        soundsSlider.onValueChanged.AddListener(AdjustSoundsVolume);
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