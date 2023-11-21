using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of GameState.
    /// </summary>
    public static GameState instance {get; private set;}

    /// <summary>
    /// Flag for if game is paused.
    /// </summary>
    public bool isPaused {get; private set;}

    /// <summary>
    /// Level objectives.
    /// </summary>
    public Level level {get; private set;}

    public float start_s {get; private set;}

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        instance = this;
        level = GetComponent<Level>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        start_s = Time.time;
        ((IObjective)level).StartObjective();
    }

    /// <summary>
    /// Add a prerequisite objective at runtime.
    /// </summary>
    /// <param name="obj"></param>
    public void AddObjective(IObjective obj)
    {
        level.AddPrereq(obj);
        UIManager.instance.objList.AddEntry(obj);
    }

    /// <summary>
    /// Pause or resume the game.
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ?
            0f :
            1f;
    }

    /// <summary>
    /// Reloads current level.
    /// </summary>
    public void Restart() {StartCoroutine(_Restart());}
    private IEnumerator _Restart()
    {
        yield return StartCoroutine(
            AuditLogger.instance.Finalize(AttemptReport.TerminatingState.restart));

        UIManager.instance.Restart();
        BanditHQ.Restart();

        DeliveryLocation.ClearAll();

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (isPaused)
        {
            TogglePause();
        }
    }


    /// <summary>
    /// Quit the application.
    /// </summary>
    public void Quit() {StartCoroutine(_Quit());}
    private IEnumerator _Quit()
    {
        yield return StartCoroutine(
            AuditLogger.instance.Finalize(AttemptReport.TerminatingState.quit));
            
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}