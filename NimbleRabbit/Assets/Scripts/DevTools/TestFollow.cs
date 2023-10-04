using UnityEngine;
using UnityEngine.AI;

public class TestFollow : MonoBehaviour
{
    #if UNITY_EDITOR

    /// <summary>
    /// Target object to follow.
    /// </summary>
    public GameObject target;

    /// <summary>
    /// NavMeshAgent reference.
    /// </summary>
    private NavMeshAgent nav;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
    }    

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        if (nav != null)
        {
            nav.speed = 20f;
        }
    }

    /// <summary>
    /// Every frame update loop.
    /// </summary>
    private void Update()
    {
        nav?.SetDestination(target.transform.position);
    }

    #endif
}