using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    /// <summary>
    /// Reference to Restart button.
    /// </summary>
    public Button restartButton {get; private set;}

    /// <summary>
    /// Reference to Exit button.
    /// </summary>
    public Button exitButton {get; private set;}

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        restartButton = transform.Find("RestartButton").GetComponent<Button>();
        exitButton = transform.Find("ExitButton").GetComponent<Button>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        restartButton.onClick.AddListener(GameState.instance.Restart);
        exitButton.onClick.AddListener(GameState.instance.Quit);
    }
}