using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsMenu : MonoBehaviour
{
    /// <summary>
    /// Parent object of main settings menu.
    /// </summary>
    public GameObject settings {get; private set;}

    /// <summary>
    /// Parent object of controls screen.
    /// </summary>
    public GameObject controls {get; private set;}

    /// <summary>
    /// Reference to Close "X" button.
    /// </summary>
    private Button closeButton;

    /// <summary>
    /// Reference to Music volume slider.
    /// </summary>
    public Slider musicSlider {get; private set;}

    /// <summary>
    /// Reference to Sounds volume slider.
    /// </summary>
    public Slider soundsSlider {get; private set;}

    /// <summary>
    /// Reference to Resume button.
    /// </summary>
    public Button resumeButton;

    /// <summary>
    /// Reference to Restart button.
    /// </summary>
    private Button restartButton;

    /// <summary>
    /// Reference to Quit button.
    /// </summary>
    private Button quitButton;

    /// <summary>
    /// Reference to the Controls button.
    ///</summary
    private Button controlsButton;

    /// <summary>
    /// Reference to the Controls Back button.
    ///</summary
    private Button controlsBackButton;

    /// <summary>
    /// Event for when music volume is changed.
    /// </summary>
    public event Action<float> OnMusicVolumeChanged;

    /// <summary>
    /// Event for when sound effects volume is changed.
    /// </summary>
    public event Action<float> OnSoundVolumeChanged;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        settings = transform.Find("Panel/Settings").gameObject;
        controls = transform.Find("Panel/Controls").gameObject;
        closeButton = transform.Find("Panel/TitleBar/Close").GetComponent<Button>();
        musicSlider = transform.Find("Panel/Settings/Music/Slider").GetComponent<Slider>();
        soundsSlider = transform.Find("Panel/Settings/Sound/Slider").GetComponent<Slider>();
        resumeButton = transform.Find("Panel/Settings/Resume").GetComponent<Button>();
        controlsButton = transform.Find("Panel/Settings/Controls").GetComponent<Button>();
        restartButton = transform.Find("Panel/Settings/Restart").GetComponent<Button>();
        quitButton = transform.Find("Panel/Settings/Quit").GetComponent<Button>();
        controlsBackButton = transform.Find("Panel/Controls/Back").GetComponent<Button>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        musicSlider.onValueChanged.AddListener(AdjustMusicVolume);
        soundsSlider.onValueChanged.AddListener(AdjustSoundsVolume);
        closeButton.onClick.AddListener(UIManager.instance.ToggleSettingsMenu);
        resumeButton.onClick.AddListener(UIManager.instance.ToggleSettingsMenu);
        restartButton.onClick.AddListener(GameState.instance.Restart);
        quitButton.onClick.AddListener(GameState.instance.Quit);
        controlsButton.onClick.AddListener(ToggleControlsMenu);
        controlsBackButton.onClick.AddListener(ToggleControlsMenu);
    }

    /// <summary>
    /// Changes music volume accordingly.
    /// </summary>
    private void AdjustMusicVolume(float value)
    {
        OnMusicVolumeChanged?.Invoke(value / 100f);
    }

    /// <summary>
    /// Changes sounds volume accordingly.
    /// </summary>
    private void AdjustSoundsVolume(float value)
    {
        OnSoundVolumeChanged?.Invoke(value / 100f);
    }

    /// <summary>
    /// Open/close the controls screen.
    /// </summary>
    private void ToggleControlsMenu()
    {
        AuditLogger.instance.ar.numControlViews++;
        bool ControlsActive = controls.activeSelf;
        if (ControlsActive)
        {
            settings.SetActive(true);
            controls.SetActive(false);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(resumeButton.gameObject);
        }
        else
        {
            settings.SetActive(false);
            controls.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(controlsBackButton.gameObject);
        }
    }
}