using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceIndicator : MonoBehaviour
{
    public static readonly HashSet<Type> ACTIVE_STATES = new HashSet<Type>(){
        typeof(ChaseState),
        typeof(AttackState)
    };

    private static List<PoliceIndicator> pool;

    private static Dictionary<int, PoliceIndicator> trackedPolice;

    [SerializeField]
    private Police police;

    private Renderer rend;

    public static void Track(Police p)
    {
        if (trackedPolice == null)
        {
            trackedPolice = new Dictionary<int, PoliceIndicator>();
        }

        int id = p.gameObject.GetInstanceID();
        if (trackedPolice.ContainsKey(id) == true)
        {
            return;
        }

        PoliceIndicator ind = GetIndicator();
        ind.police = p;
        ind.gameObject.SetActive(true);

        trackedPolice.Add(
            id,
            ind);
    }

    public static void Untrack(Police p)
    {
        if (trackedPolice == null)
        {
            trackedPolice = new Dictionary<int, PoliceIndicator>();
        }

        int id = p.gameObject.GetInstanceID();
        if (trackedPolice.ContainsKey(id) == false)
        {
            return;
        }

        PoliceIndicator ind = trackedPolice[id];
        ind.Disable();
    }

    public static void ClearAll()
    {
        pool?.Clear();
        trackedPolice?.Clear();
    }

    private static PoliceIndicator GetIndicator()
    {
        if (pool == null) {
            pool = new List<PoliceIndicator>();
        }

        foreach (PoliceIndicator ind in pool)
        {
            if (ind.gameObject.activeInHierarchy == false)
            {
                return ind;
            }
        }


        PoliceIndicator indicator = Instantiate(
            UIManager.instance.policeIndicatorPrefab,
            UIManager.instance.policeIndicators.transform);
        
        pool.Add(indicator);
        return indicator;
    }

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (police == null)
        {
            Disable();
            return;
        }

        if (ACTIVE_STATES.Contains(police.stateMachine.currentState.GetType()) == true)
        {
            PlayerController player = PlayerController.instance;

            // this is the vector from police to player car
            Vector3 distVector = police.transform.position - player.transform.position;

            // find angle between the police and forward direction to determine the indicator's rotation
            float angle = Vector3.SignedAngle(
                distVector,
                Vector3.forward,
                Vector3.up);

            
            // make the indicator rotate and point to police
            transform.rotation = Quaternion.Euler(
                90f,
                0f,
                angle
            );

            // calculate indicator offset from player car
            Vector3 indicatorOffset = new Vector3(
                5f * distVector.normalized.x,
                0.8f,
                5f * distVector.normalized.z
            );

            // update indicator's position
            transform.position = player.transform.position + indicatorOffset;
        }
        else {
            Disable();
        }
    }

    private void Disable()
    {
        if (police != null)
        {
            trackedPolice.Remove(police.gameObject.GetInstanceID());
        }

        police = null;
        this.gameObject.SetActive(false);
    }
}