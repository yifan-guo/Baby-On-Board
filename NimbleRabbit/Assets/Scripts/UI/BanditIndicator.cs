using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditIndicator : MonoBehaviour
{
    /// <summary>
    /// List of bandit states that the indicator should be active during.
    /// </summary>
    public static readonly HashSet<Type> ACTIVE_STATES = new HashSet<Type>(){
        typeof(ChaseState),
        typeof(AttackState)};

    /// <summary>
    /// All existing BanditIndicators.
    /// </summary>
    private static List<BanditIndicator> pool;

    /// <summary>
    /// Dictionary of unique tracked object ids and the respective indicator.
    /// </summary>
    private static Dictionary<int, BanditIndicator> trackedBandits;

    /// <summary>
    /// Reference to the bandit this indicator is pointing to.
    /// </summary>
    [SerializeField]
    private Bandit bandit;

    /// <summary>
    /// Reference to the Renderer component.
    /// </summary>
    private Renderer rend;

    /// <summary>
    /// Track this bandit with an indicator.
    /// </summary>
    /// <param name="b"></param>
    public static void Track(Bandit b)
    {
        // Initialize
        if (trackedBandits == null)
        {
            trackedBandits = new Dictionary<int, BanditIndicator>();
        }

        // Check if it's already tracked
        int id = b.gameObject.GetInstanceID();
        if (trackedBandits.ContainsKey(id) == true)
        {
            return;
        }

        BanditIndicator ind = GetIndicator();
        ind.bandit = b;
        ind.gameObject.SetActive(true);

        trackedBandits.Add(
            id,
            ind);
    }

    /// <summary>
    /// Stop tracking this bandit.
    /// </summary>
    /// <param name="b"></param>
    public static void Untrack(Bandit b)
    {
        // Initialize
        if (trackedBandits == null)
        {
            trackedBandits = new Dictionary<int, BanditIndicator>();
        }

        // Ensure it is being tracked at all
        int id = b.gameObject.GetInstanceID();
        if (trackedBandits.ContainsKey(id) == false)
        {
            return;
        }

        BanditIndicator ind = trackedBandits[id];    
        ind.Disable();
    }

    /// <summary>
    /// Clear static resources.
    /// </summary>
    public static void ClearAll()
    {
        pool.Clear();
        trackedBandits.Clear();
    }

    /// <summary>
    /// Get an unused indicator or make a new one.
    /// </summary>
    private static BanditIndicator GetIndicator()
    {
        if (pool == null)
        {
            pool = new List<BanditIndicator>();
        }

        foreach (BanditIndicator ind in pool)
        {
            if (ind.gameObject.activeInHierarchy == false)
            {
                return ind;
            }
        }

        BanditIndicator indicator = Instantiate(
            UIManager.instance.banditIndicatorPrefab,
            UIManager.instance.banditIndicators.transform);

        pool.Add(indicator);
        return indicator;
    }

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }    

    /// <summary>
    /// Every frame update loop.
    /// </summary>
    private void Update()
    {
        if (bandit == null)
        {
            Disable();
            return;
        }

        if (ACTIVE_STATES.Contains(bandit.stateMachine.currentState.GetType()) == true)
        {
            PlayerController player = PlayerController.instance;

            //this is the vector from bandit to player car
            Vector3 distVector = bandit.transform.position - player.transform.position;

            //find angle between the bandit and forward direction to determine the indicator's rotation
            float angle = Vector3.SignedAngle(
                distVector, 
                Vector3.forward, 
                Vector3.up);

            //make the indicator rotate and point to the bandit
            transform.rotation = Quaternion.Euler(
                90f, 
                0f, 
                angle);

            //calculate indicator offset from player car
            Vector3 indicatorOffset = new Vector3(
                5f * distVector.normalized.x, 
                0.8f, 
                5f * distVector.normalized.z);

            //update indicator's position
            transform.position = player.transform.position + indicatorOffset;
        }
        else
        {
            Disable();
        }
    }

    /// <summary>
    /// Turn this indicator off.
    /// </summary>
    private void Disable()
    {
        if (bandit != null)
        {
            trackedBandits.Remove(bandit.gameObject.GetInstanceID());
        }

        bandit = null;
        this.gameObject.SetActive(false);
    }
}
