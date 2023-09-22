using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(StateMachine))]
public class NPC : MonoBehaviour
{
    /// <summary>
    /// Enum for NPC roles.
    /// </summary>
    public enum Role {Civilian, Bandit, Police};

    [Header("Info")]
    
    /// <summary>
    /// Type of NPC.
    /// </summary>
    public Role role;

    /// <summary>
    /// Move speed.
    /// </summary>
    public int moveSpeed;

    /// <summary>
    /// Reference to this NPC's nav component.
    /// </summary>
    public NavMeshAgent nav {get; protected set;}

    /// <summary>
    /// Reference to this NPC's state machine.
    /// </summary>
    protected StateMachine stateMachine;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    protected virtual void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        stateMachine = GetComponent<StateMachine>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    protected virtual void Start()
    {
        nav.speed = moveSpeed;
    }
}