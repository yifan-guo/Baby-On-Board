using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(StateMachine))]
public abstract class NPC : MonoBehaviour
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
    /// Reference to this NPC's rigidbody.
    /// </summary>
    protected Rigidbody rb;

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
        rb = GetComponent<Rigidbody>();
        stateMachine = GetComponent<StateMachine>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    protected virtual void Start()
    {
        nav.speed = moveSpeed;
    }

    /// <summary>
    /// Applies a force to the NPC and stops their nav system temporarily.
    /// NavMeshAgent movement will not take any AddForce() while active.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Crash(Vector3 force)
    {
        rb.velocity = nav.velocity;
        nav.enabled = false;
        rb.isKinematic = false;

        rb.AddForce(
            force,
            ForceMode.Impulse);

        // Regain control after coming to a stop
        while (rb.velocity.magnitude > 0.1f)
        {
            yield return null;
        }

        nav.enabled = true;
        rb.isKinematic = true;
    }
}