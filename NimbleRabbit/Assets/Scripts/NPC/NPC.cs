using System.Collections;
using UnityEngine;
using UnityEngine.AI;

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
    /// Whether or not NPC is actively completing a turn.
    /// </summary>
    public bool isTurning;

    /// <summary>
    /// Whether or not NPC just suffered a crash.
    /// </summary>
    public bool isCrashed;

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
    /// Every physics frame update loop.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        // Only drive forward if the next location
        // is in front of you enough
        if (CanDriveToward(nav.steeringTarget) == false)
        {
            // TODO:
            // Slowly back up while rotating to look at next destination.
            nav.velocity = Vector3.zero;
        }

        // Check for a turn
        if (isTurning == false &&
            nav.isOnOffMeshLink == true)
        {
            StartCoroutine(Turn(nav.currentOffMeshLinkData));
        }
   }

    /// <summary>
    /// Whether or not the target destination is in front of the NPC enough.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected bool CanDriveToward(Vector3 target)
    {
        // Dot product interpretation:
        // 1 = directly in front
        // 0 = directly to the left/right
        // -1 = directly behind
        float dot = Vector3.Dot(
            (target - transform.position).normalized,
            transform.forward);

        return dot > 0.5f;
    }

    /// <summary>
    /// Conduct the designated turn.
    /// </summary>
    /// <param name="linkData"></param>
    /// <returns></returns>
    public IEnumerator Turn(OffMeshLinkData linkData)
    {
        isTurning = true;

        bool rotated = false;
        float startTime = Time.time;
        float duration_s = (linkData.endPos - transform.position).magnitude / nav.speed;
        while (transform.position != linkData.endPos)
        {
            // Immediately stop if we crashed
            if (isCrashed == true)
            {
                break;
            }

            Quaternion lookRot = Quaternion.LookRotation(
                linkData.endPos - transform.position,
                Vector3.up);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                lookRot,
                (Time.time - startTime) / (4 * duration_s));

            // Don't move toward the destination until we are rotated better
            if (rotated == true ||
                CanDriveToward(linkData.endPos) == true)
            {
                rotated = true;
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    linkData.endPos,
                    nav.speed * Time.deltaTime);
            }

            yield return null;
        }

        nav.CompleteOffMeshLink();
        isTurning = false;
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
        isCrashed = true;

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
        isCrashed = false;
    }
}