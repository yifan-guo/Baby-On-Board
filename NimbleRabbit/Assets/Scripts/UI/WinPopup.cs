using UnityEngine;
using UnityEngine.UI;

public class WinPopup : MonoBehaviour
{
    /// <summary>
    /// Reference to Restart button.
    /// </summary>
    public Button restartButton;

    /// <summary>
    /// Reference to Exit button.
    /// </summary>
    public Button exitButton;

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        restartButton.onClick.AddListener(GameState.instance.Restart);
        exitButton.onClick.AddListener(GameState.instance.Quit);
    }
}