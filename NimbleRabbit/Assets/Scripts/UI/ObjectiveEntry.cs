using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ObjectiveEntry : MonoBehaviour
{
    /// <summary>
    /// Objective this entry is tracking.
    /// </summary>
    private IObjective obj;

    /// <summary>
    /// Profile picture ID icon.
    /// </summary>
    private Image id;

    private TMP_Text letterID;

    /// <summary>
    /// Text that tells the player what they need to do.
    /// </summary>
    private TextMeshProUGUI instruction;

    /// <summary>
    /// Object that displays a time limit if there is one for this objective.
    /// </summary>
    private Image timeBar;

    /// <summary>
    /// Background of time bar as it drains.
    /// </summary>
    private Image emptyTimeBar;

    /// <summary>
    /// Text that displays the amount of time remaining.
    /// </summary>
    private TextMeshProUGUI time;

    /// <summary>
    /// Stopwatch image.
    /// </summary>
    private Image clock;

    /// <summary>
    /// Reference to RectTransform.
    /// </summary>
    private RectTransform xform;

    private Image border;

    /// <summary>
    /// Color corresponding to this objective
    /// (Affects objective entry as well as indicator).
    /// </summary>
    public Color color {get; set;}
    private static readonly List<Color> colors = new List<Color>
    {
        Color.blue,
        Color.cyan,
        Color.green,
        Color.magenta,
        Color.red,
        Color.yellow
    };

    /// <summary>
    /// Letter ID.
    /// </summary>
    public char letter {get; set;}
    private const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        id = transform.Find("ID").GetComponent<Image>();
        letterID = transform.Find("ID/Letter").GetComponent<TMP_Text>();
        instruction = transform.Find("Instruction").GetComponent<TextMeshProUGUI>();
        timeBar = transform.Find("TimeBar").GetComponent<Image>();
        emptyTimeBar = transform.Find("EmptyTimeBar").GetComponent<Image>();
        time = transform.Find("Time").GetComponent<TextMeshProUGUI>();
        clock = transform.Find("Clock").GetComponent<Image>();
        xform = GetComponent<RectTransform>();
        border = transform.Find("Border").GetComponent<Image>();
        border.enabled = false;
    }

    /// <summary>
    /// Link this entry to an objective and update instruction.
    /// </summary>
    public void Link(IObjective obj)
    {
        color = colors[ObjectivesList.entries.Count % colors.Count];
        id.color = color;
        letterID.color = color;

        letter = letters[ObjectivesList.entries.Count % letters.Length];
        letterID.text = $"{letter}";
        
        this.obj = obj;
        this.obj.OnObjectiveUpdated += UpdateEntry;
        UpdateEntry();
    }

    /// <summary>
    /// Every frame update loop.
    /// </summary>
    private void Update()
    {
        if (obj == null)
        {
            return;
        }

        if (obj.ObjectiveStatus != IObjective.Status.InProgress)
        {
            return;
        }

        Timer();
    }

    /// <summary>
    /// Reduce timer (text and bar).
    /// </summary>
    private void Timer()
    {
        Package pkg = (Package) obj;
        float remainingTime_s = Mathf.Max(
            pkg.TTLAfterPickupSeconds - (Time.time - obj.StartTime),
            0f);
        time.text = $"{Mathf.Ceil(remainingTime_s)}s";
        timeBar.fillAmount = remainingTime_s / pkg.TTLAfterPickupSeconds;
    }

    /// <summary>
    /// Listener to objective update event that updates instruction entry.
    /// </summary>
    private void UpdateEntry()
    {
        string txt = "";
        switch (obj.ObjectiveStatus)        
        {
            case IObjective.Status.NotStarted:
                txt += $"Get my {obj.Name} please!";
                clock.enabled = false;
                time.enabled = false;
                timeBar.enabled = false;
                emptyTimeBar.enabled = false;
                border.enabled = false;
                break;

            case IObjective.Status.InProgress:
                if (obj.gameObject.transform.parent == null)
                {
                    txt += obj.Description;
                }
                else
                {
                    txt += (obj.gameObject.transform.parent.tag == "Player") ?
                        obj.Description :
                        "theyre STEALING my stuff!";
                }
                clock.enabled = true;
                time.enabled = true;
                timeBar.enabled = true;
                emptyTimeBar.enabled = true;
                border.enabled = true;
                StartCoroutine(BorderFlash());
                break;

            case IObjective.Status.Complete:
                txt += "--Delivered--";
                clock.enabled = false;
                time.enabled = false;
                timeBar.enabled = false;
                emptyTimeBar.enabled = false;
                border.enabled = false;
                break;

            case IObjective.Status.Failed:
                // TODO:
                // THIS NEEDS TO CHECK IF THE PKG WAS Destroyed, late, or stolen
                txt += "--Failed--";
                clock.enabled = false;
                time.enabled = false;
                timeBar.enabled = false;
                emptyTimeBar.enabled = false;
                border.enabled = false;
                break;

            default:
                Debug.LogWarning($"Unknown objective status: {obj.ObjectiveStatus}");
                clock.enabled = false;
                time.enabled = false;
                timeBar.enabled = false;
                emptyTimeBar.enabled = false;
                border.enabled = false;
                break;
        }

        instruction.text = txt;
    }

    private IEnumerator BorderFlash()
    {
        Color orange = new Color(
            255f / 255f, 
            116f/ 255f, 
            0);
            
        float time_s = Time.time;

        while (obj.ObjectiveStatus == IObjective.Status.InProgress)
        {
            if (Time.time - time_s > 0.5f)
            {
                border.color = (border.color == orange) ?
                    Color.white :
                    orange;

                time_s = Time.time;
            }

            yield return null;
        }

        yield break;
    }
}