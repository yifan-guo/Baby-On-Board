using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{
    /// <summary>
    /// All existing indicators.
    /// </summary>
    private static List<Indicator> indicatorPool;

    /// <summary>
    /// Dictionary of unique tracked objects ids and the respective Indicator.
    /// </summary>
    private static Dictionary<int, Indicator> trackedObjectsMap;

    /// <summary>
    /// Reference to the object this is pointing towards.
    /// </summary>
    [SerializeField]
    private GameObject target;

    /// <summary>
    /// Reference to the Image component.
    /// </summary>
    private Image img;

    /// <summary>
    /// Arrow Image component when target is in view.
    /// </summary>
    private Image arrowImg;

    /// <summary>
    /// Provide new object to track with indicator.
    /// </summary>
    /// <param name="obj"></param>
    public static void Track(GameObject obj)
    {
        // Initialize object map
        if (trackedObjectsMap == null)
        {
            trackedObjectsMap = new Dictionary<int, Indicator>();
        }

        int id = obj.GetInstanceID();

        // Check if there's already an active indicator for the obj
        if (trackedObjectsMap.ContainsKey(id) == true)
        {
            return;
        }

        // Activate indicator
        Indicator indicator = GetIndicator();
        indicator.target = obj;
        indicator.gameObject.SetActive(true);

        trackedObjectsMap.Add(
            obj.GetInstanceID(),        
            indicator);
    }

    /// <summary>
    /// Stop tracking an object with an indicator.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static void Untrack(GameObject obj)
    {
        int id = obj.GetInstanceID();

        // Verify that it is actually being tracked
        if (trackedObjectsMap.ContainsKey(id) == false)
        {
            return;
        }

        Indicator indicator = trackedObjectsMap[id];
        indicator.Disable();

        trackedObjectsMap.Remove(id);
    }

    /// <summary>
    /// Clear static resources.
    /// </summary>
    public static void ClearAll()
    {
        indicatorPool.Clear();
        trackedObjectsMap.Clear();
    }

    /// <summary>
    /// Get an unused indicator or new one if all are busy.
    /// </summary>
    /// <returns>Indicator</returns>
    private static Indicator GetIndicator()
    {
        // Initialize pool if needed
        if (indicatorPool == null)
        {
            indicatorPool = new List<Indicator>();
        }

        // Check for inactive indicator
        foreach(Indicator existingIndicator in indicatorPool)
        {
            if (existingIndicator.gameObject.activeInHierarchy == false)
            {
                return existingIndicator;
            }
        }

        // Create a new one
        Indicator indicator = Instantiate(
            UIManager.instance.indicatorPrefab,
            UIManager.instance.indicators.transform);

        indicatorPool.Add(indicator);
        return indicator;
    }

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        img = GetComponent<Image>();
        arrowImg = transform.Find("Arrow").GetComponent<Image>();
    }

    /// <summary>
    /// Every frame update loop.
    /// </summary>
    private void Update()
    {
        if (target == null)
        {
            Disable();
            return;
        }

        Position();
        Resize();
    }

    /// <summary>
    /// Turn indicator off.
    /// </summary>
    private void Disable()
    {
        target = null;
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// Resize indicator depending on distance from player.
    /// </summary>
    private void Resize()
    {
        // Indicator target distance bounds for size
        const float MAX_DISTANCE_2 = 200f * 200f;
        const float MIN_DISTANCE_2 = 25f * 25f;

        // Get ratio for indicator size change depending on distance from camera
        float dist_2 = (target.transform.position - Camera.main.transform.position).sqrMagnitude;
        float ratio = (dist_2 - MIN_DISTANCE_2) / (MAX_DISTANCE_2 - MIN_DISTANCE_2);
        ratio = Mathf.Clamp(
            ratio,
            0f,
            1f);

        img.rectTransform.localScale = Vector2.one * (1f + ratio);

        // Minimum image alpha value
        const float MIN_ALPHA = 0.35f;

        // Adjust alpha to distance as well
        Color color = img.color;
        color.a = MIN_ALPHA + (1f - MIN_ALPHA) * (1f - ratio);
        img.color = color;
        arrowImg.color = color;
    }

    /// <summary>
    /// Place indicator on screen accordingly.
    /// </summary>
    private void Position()
    {
        // Get initial screen position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.transform.position);

        // If the target is behind the camera,
        // keep indicator at bottom of screen and flip the horizontal position
        float dot = Vector3.Dot(
            Camera.main.transform.forward,
            (target.transform.position - Camera.main.transform.position).normalized);
        
        if (dot < 0)
        {
            screenPos.x *= -1f;
            screenPos.y = 0f;
        }

        // Clamp indicator to edges of screen
        screenPos.x = Mathf.Clamp(
            screenPos.x,
            0f,
            Screen.width);

        screenPos.y = Mathf.Clamp(
            screenPos.y,
            0f,
            Screen.height);

        screenPos = PositionInView(screenPos);

        transform.position = screenPos;
    }

    /// <summary>
    /// Repositions indicator if it is in view
    /// AKA not on the edge of screen.
    /// </summary>
    /// <param name="screenPos"></param>
    /// <returns>Updated screen position</returns>
    private Vector3 PositionInView(Vector3 screenPos)
    {
        bool inView = 
            (screenPos.x > 0 &&
            screenPos.x < Screen.width &&
            screenPos.y > 0 &&
            screenPos.y < Screen.height);

        arrowImg.enabled = inView;

        if (inView == true)
        {
            screenPos.y += (img.rectTransform.sizeDelta.y * img.rectTransform.localScale.y);
        }

        return screenPos;
    }
}