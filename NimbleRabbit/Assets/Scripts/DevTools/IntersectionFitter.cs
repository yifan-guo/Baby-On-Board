using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class IntersectionFitter : MonoBehaviour
{
    #if UNITY_EDITOR

    // NOTES:
    // The terms left/right lanes generally refers to incoming/outgoing
    // lanes respectively. In hindsight these terms would be updated in variable
    // names to prevent confusion, but I'm too lazy at the moment.

    /// <summary>
    /// Left and right lane widths for vertical roads (top and bottom).
    /// </summary>
    [Min(0)]
    public Vector2 verticalLaneWidths;

    /// <summary>
    /// Left and right lane widths for horizontal roads (left and right).
    /// </summary>
    [Min(0)]
    public Vector2 horizontalLaneWidths;

    /// <summary>
    /// Length of TopRoad
    /// </summary>
    [Min(0)]
    public float topRoadLength;

    /// <summary>
    /// Length of RightRoad
    /// </summary>
    [Min(0)]
    public float rightRoadLength;

    /// <summary>
    /// Length of BottomRoad
    /// </summary>
    [Min(0)]
    public float bottomRoadLength;

    /// <summary>
    /// Length of LeftRoad
    /// </summary>
    [Min(0)]
    public float leftRoadLength;

    /// <summary>
    /// Internal flag to know when to fit object and dirty objects.
    /// </summary>
    private bool fit;

    /// <summary>
    /// Top road of 4 way intersection.
    /// </summary>
    private Transform topRoad;

    /// <summary>
    /// Right road of 4 way intersection.
    /// </summary>
    private Transform rightRoad;

    /// <summary>
    /// Bottom road of 4 way intersection.
    /// </summary>
    private Transform bottomRoad;

    /// <summary>
    /// Left road of 4 way intersection.
    /// </summary>
    private Transform leftRoad;

    /// <summary>
    /// Center of 4 way intersection.
    /// </summary>
    private Transform center;

    /// <summary>
    /// Thickness of lane line.
    /// </summary>
    private const float laneLineThickness = 1f;

    /// <summary>
    /// Thickness of stop line.
    /// </summary>
    private const float stopLineThickness = 1f;

    /// <summary>
    /// How close to the edge the link is.
    /// Sometimes they aren't reachable if it's the exact edge.
    /// </summary>
    private const float linkOffset = 1f;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        // Only do this in Editor when not in PlayMode
        if (Application.isPlaying == true)
        {
            this.enabled = false;
            return;
        }
    }

    /// <summary>
    /// Called when the script is loaded or a value changes in the Inspector.
    /// </summary>
    private void OnValidate()
    {
        fit = true;
    }

    /// <summary>
    /// Every frame update loop.
    /// </summary>
    private void Update()
    {
        // Only do this in Editor when not in PlayMode
        if (Application.isPlaying == true)
        {
            this.enabled = false;
            return;
        }

        // Only fit and dirty objects when there's a change
        if (fit == true)
        {
            Fit();

            // Let's us save the changes
            EditorUtility.SetDirty(this);
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            fit = false;
        }
    }

    /// <summary>
    /// Get references to roads as their true direction (not relative).
    /// </summary>
    private void GetCardinalRoads()
    {
        if (topRoad != null &&
            rightRoad != null &&
            bottomRoad != null &&
            leftRoad != null &&
            center != null)
        {
            return;
        }

        topRoad = transform.Find("TopRoad");
        rightRoad = transform.Find("RightRoad");
        bottomRoad = transform.Find("BottomRoad");
        leftRoad = transform.Find("LeftRoad");
        center = transform.Find("Center");
    }

    /// <summary>
    /// Main method that calls all helper methods to update intersection.
    /// </summary>
    private void Fit()
    {
        GetCardinalRoads();

        // Do not modify Intersection's scale,
        // use public settings to change size
        transform.localScale = Vector3.one;

        FitCenter();

        FitRoad(
            topRoad,
            topRoadLength);

        FitRoad(
            rightRoad,
            rightRoadLength);

        FitRoad(
            bottomRoad,
            bottomRoadLength);

        FitRoad(
            leftRoad,
            leftRoadLength);
        
        // Top road as top
        LinkRoads(
            topRoad,
            rightRoad,
            bottomRoad,
            leftRoad);

        // Right road as top
        LinkRoads(
            rightRoad,
            bottomRoad,
            leftRoad,
            topRoad);

        // Bottom road as top
        LinkRoads(
            bottomRoad,
            leftRoad,
            topRoad,
            rightRoad);

        // Left road as top
        LinkRoads(
            leftRoad,
            topRoad,
            rightRoad,
            bottomRoad);
    }

    /// <summary>
    /// Fit center according to lane widths.
    /// </summary>
    private void FitCenter()
    {
        if (center == null)
        {
            return;
        }

        center.gameObject.SetActive(true);
        center.localPosition = Vector3.zero;
        center.localScale = new Vector3(
            verticalLaneWidths[0] + verticalLaneWidths[1],
            1f,
            horizontalLaneWidths[0] + horizontalLaneWidths[1]);
    }

    /// <summary>
    /// Fit road to length and lane widths.
    /// </summary>
    private void FitRoad(
        Transform road,
        float length)
    {
        if (road == null)
        {
            return;
        }

        float centerLength; // Dimension of center that is parallel with road
        Vector3 dir;        // Relative direction from center
        Vector2 laneWidths; // Relavent pair of lane widths

        if (road == topRoad)
        {
            centerLength = center.localScale.z;
            dir = Vector3.forward;
            laneWidths = verticalLaneWidths;
        }
        else if (road == rightRoad)
        {
            centerLength = center.localScale.x;
            dir = Vector3.right;
            laneWidths = horizontalLaneWidths;
        }
        else if (road == bottomRoad)
        {
            dir = Vector3.back;
            centerLength = center.localScale.z;
            laneWidths = new Vector2(
                verticalLaneWidths[1],
                verticalLaneWidths[0]);
        }
        else
        {
            centerLength = center.localScale.x;
            dir = Vector3.left;
            laneWidths = new Vector2(
                horizontalLaneWidths[1],
                horizontalLaneWidths[0]);
        }

        road.gameObject.SetActive(length > 0f);
        road.localPosition = dir * (length + centerLength) / 2f;
        road.localScale = new Vector3(
            length,
            1f,
            1f);

        if (length > 0f)
        {
            FitLanes(
                road,
                laneWidths);
        }
    }

    /// <summary>
    /// Fit lanes to lane widths.
    /// </summary>
    private void FitLanes(
        Transform road,
        Vector2 laneWidths)
    {
        Transform leftLane = road.Find("LeftLane");
        Transform rightLane = road.Find("RightLane");
        float roadWidth = laneWidths[0] + laneWidths[1];

        Vector3 scale = Vector3.one;
        scale.z = laneWidths[0];
        leftLane.localScale = scale;
        scale.z = laneWidths[1];
        rightLane.localScale = scale;

        Vector3 pos = Vector3.zero;
        pos.z = (roadWidth / 2f) - (laneWidths[0] / 2f);
        leftLane.localPosition = pos;
        pos.z = -(roadWidth / 2f) + (laneWidths[1] / 2f);
        rightLane.localPosition = pos;

        leftLane.gameObject.SetActive(laneWidths[0] > 0);
        rightLane.gameObject.SetActive(laneWidths[1] > 0);

        FitStopLine(road);
        FitLaneLine(
            road,
            laneWidths);
    }

    /// <summary>
    /// Fit lane line to lane widths.
    /// </summary>
    private void FitLaneLine(
        Transform road,
        Vector2 laneWidths)
    {
        // Only active if there are 2 lanes
        Transform laneLine = road.Find("LaneLine");
        laneLine.gameObject.SetActive(
            laneWidths[0] > 0 && 
            laneWidths[1] > 0);

        float roadWidth = laneWidths[0] + laneWidths[1];

        Vector3 scale = Vector3.one;
        scale.z = laneLineThickness; 
        laneLine.localScale = scale;

        Vector3 pos = new Vector3(
            0f,                                 // Default to 0 to stay same length with road
            0.01f,                              // Places lane line above road (0f)
            (roadWidth / 2f) - laneWidths[0]);  // Line goes on the right side of the left lane

        laneLine.localPosition = pos;
    }

    /// <summary>
    /// Fit stop line to left lane width.
    /// </summary>
    private void FitStopLine(Transform road)
    {
        Transform laneLine = road.Find("LaneLine");
        Transform stopLine = road.Find("LeftLane/StopLine");
        Transform leftLane = stopLine.parent;

        // If left lane is not active, then this child won't be active
        if (stopLine.gameObject.activeInHierarchy == false)
        {
            return;
        }

        float leftLaneLength = leftLane.localScale.x * road.localScale.x;
        float leftLaneWidth = leftLane.localScale.z * road.localScale.z;

        Vector3 scale = new Vector3(
            stopLineThickness / leftLaneLength,                             // Set to user specified thickness
            1f,                                                             // Keep defaulted to 1 to avoid factoring in height positioning 
            1f + laneLineThickness / (2f * leftLaneWidth));                 // Always be long enough to cover the lane and lane line

        Vector3 pos = new Vector3(
            (leftLaneLength - stopLineThickness) / (2f * leftLaneLength),   // Half of road length but pushed back by half of stop line thickness
            0.011f,                                                         // Places it above lane line (0.01f)
            laneLineThickness / (4f * leftLaneWidth));                      // Move over 1/4 of the lane line's thickness 

        stopLine.localPosition = pos;
        stopLine.localScale = scale;
    }

    /// <summary>
    /// Toggle NavMeshLinks depending on top road's status.
    /// Pass roads in via relative directions.
    /// </summary>
    private void LinkRoads(
        Transform top,
        Transform right,
        Transform bottom,
        Transform left)
    {
        // Lanes in the top road
        Transform rightLane = top?.Find("RightLane");
        Transform leftLane = top?.Find("LeftLane");

        // Turns from other roads that turn onto the top road
        NavMeshLink straightIntoMe = bottom?.Find("LeftLane/StraightLink").GetComponent<NavMeshLink>();
        NavMeshLink rightTurnIntoMe = right?.Find("LeftLane/RightTurnLink").GetComponent<NavMeshLink>();
        NavMeshLink leftTurnIntoMe = left?.Find("LeftLane/LeftTurnLink").GetComponent<NavMeshLink>();

        // Other roads can only turn into top road if right lane is active
        bool status = (top == null) ? 
            false :
            rightLane.gameObject.activeInHierarchy;

        straightIntoMe.gameObject.SetActive(status);
        rightTurnIntoMe.gameObject.SetActive(status);
        leftTurnIntoMe.gameObject.SetActive(status);

        // Dirty the NavMeshLink components
        EditorUtility.SetDirty(straightIntoMe);
        EditorUtility.SetDirty(rightTurnIntoMe);
        EditorUtility.SetDirty(leftTurnIntoMe);

        // If entire top road is disabled then we're done
        if (top.gameObject.activeInHierarchy == false)
        {
            return;
        }

        // Turns that a car in the top road can do
        NavMeshLink rightLaneUTurn = rightLane.Find("UTurnLink").GetComponent<NavMeshLink>();
        NavMeshLink leftLaneUTurn = leftLane.Find("UTurnLink").GetComponent<NavMeshLink>();
        NavMeshLink leftLaneStraight = leftLane.Find("StraightLink").GetComponent<NavMeshLink>();
        NavMeshLink leftLaneRightTurn = leftLane.Find("RightTurnLink").GetComponent<NavMeshLink>();
        NavMeshLink leftLaneLeftTurn = leftLane.Find("LeftTurnLink").GetComponent<NavMeshLink>();

        // Road lanes that a car in the top road can turn into
        Transform iGoStraightHere = bottom.Find("RightLane");
        Transform iRightTurnHere = left.Find("RightLane");
        Transform iLeftTurnHere = right.Find("RightLane");

        float roadLength = leftLane.localScale.x * top.localScale.x;
        float rightLaneWidth = rightLane.localScale.z * top.localScale.z;
        float leftLaneWidth = leftLane.localScale.z * top.localScale.z;
        float centerHeight = (iRightTurnHere.localScale.z * left.localScale.z) + (iLeftTurnHere.localScale.z * right.localScale.z);

        // Save link status and make link active so Unity knows which 
        // link is updating. Helps prevent error logs.
        bool rightLaneStatus = rightLane.gameObject.activeSelf;
        bool leftLaneStatus = leftLane.gameObject.activeSelf;
        bool leftLaneStraightStatus = leftLaneStraight.gameObject.activeSelf;
        bool leftLaneRightTurnStatus = leftLaneRightTurn.gameObject.activeSelf;
        bool leftLaneLeftTurnStatus = leftLaneLeftTurn.gameObject.activeSelf;
        rightLane.gameObject.SetActive(true);
        leftLane.gameObject.SetActive(true);
        rightLaneUTurn.gameObject.SetActive(true);
        leftLaneUTurn.gameObject.SetActive(true);
        leftLaneStraight.gameObject.SetActive(true);
        leftLaneRightTurn.gameObject.SetActive(true);
        leftLaneLeftTurn.gameObject.SetActive(true);

        // Position top road's link's starting points
        Vector3 pos = Vector3.right * (((roadLength / 2f) - linkOffset) / roadLength);
        rightLaneUTurn.transform.localPosition = pos;
        leftLaneUTurn.transform.localPosition = pos;
        leftLaneStraight.transform.localPosition = pos;
        leftLaneRightTurn.transform.localPosition = pos;
        leftLaneLeftTurn.transform.localPosition = pos;

        rightLaneUTurn.startPoint = Vector3.zero;
        leftLaneUTurn.startPoint = Vector3.zero;
        leftLaneStraight.startPoint = Vector3.zero;
        leftLaneRightTurn.startPoint = Vector3.zero;
        leftLaneLeftTurn.startPoint = Vector3.zero;

        // Position top road's link's ending points
        rightLaneUTurn.endPoint = Vector3.forward * (leftLaneWidth + rightLaneWidth) / 2f;
        leftLaneUTurn.endPoint = rightLaneUTurn.endPoint;
        leftLaneStraight.endPoint = Vector3.right * (centerHeight + 2 * linkOffset);

        leftLaneRightTurn.endPoint = new Vector3(
            (iRightTurnHere.localScale.z * left.localScale.z / 2f) + linkOffset,                                                        // Go forward half the width of the destination lane
            0f,                                                                                                                         // By default the link does not have any verticality yet
            -(leftLaneWidth / 2f) - linkOffset);                                                                                        // Go right by half the width of our lane

        leftLaneLeftTurn.endPoint = new Vector3(
            (iRightTurnHere.localScale.z * right.localScale.z) + (iLeftTurnHere.localScale.z * right.localScale.z / 2f) + linkOffset,   // Go forward by the full width of the left lane and half the right lane of the destination road
            0f,                                                                                                                         // By default the link does not have any verticality yet
            (leftLaneWidth / 2f) + rightLaneWidth + linkOffset);                                                                        // Go left by half the width of our lane and full width of our right lane

        // Restore saved link statuses after endpoints are updated.
        // U-turns are only available if both lanes are active.
        rightLane.gameObject.SetActive(rightLaneStatus);
        leftLane.gameObject.SetActive(leftLaneStatus);
        rightLaneUTurn.gameObject.SetActive(leftLane.gameObject.activeInHierarchy);
        leftLaneUTurn.gameObject.SetActive(rightLane.gameObject.activeInHierarchy);
        leftLaneStraight.gameObject.SetActive(leftLaneStraightStatus);
        leftLaneRightTurn.gameObject.SetActive(leftLaneRightTurnStatus);
        leftLaneLeftTurn.gameObject.SetActive(leftLaneLeftTurnStatus);

        // Dirty the NavMeshLink components
        EditorUtility.SetDirty(rightLaneUTurn);
        EditorUtility.SetDirty(leftLaneUTurn);
        EditorUtility.SetDirty(leftLaneStraight);
        EditorUtility.SetDirty(leftLaneRightTurn);
        EditorUtility.SetDirty(leftLaneLeftTurn);
    }

    #endif
}