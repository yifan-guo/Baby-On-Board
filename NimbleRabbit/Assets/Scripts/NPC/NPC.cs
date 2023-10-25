using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(StateMachine))]
[RequireComponent(typeof(HealthManager))]
public abstract class NPC : MonoBehaviour
{
    /// <summary>
    /// Enum for NPC roles.
    /// </summary>
    public enum Role {Civilian, Bandit, Police};

    [Header("Settings")]
    
    /// <summary>
    /// Type of NPC.
    /// </summary>
    public Role role;

    /// <summary>
    /// Absolute maximum move speed.
    /// </summary>
    public float maxSpeed;

    /// <summary>
    /// Range to perform attack.
    /// </summary>
    public float attackRange;

    /// <summary>
    /// Range we can see objects from (i.e. distance to see player).
    /// </summary>
    public float visionRange;

    [Header("Status")]

    /// <summary>
    /// Max move speed of the current state.
    /// </summary>
    public float stateSpeed;

    /// <summary>
    /// Whether this NPC has to wait before it can do anything.
    /// </summary>
    public bool inCooldown;

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
    /// Reference to this NPC's main body collider.
    /// </summary>
    protected Collider coll;

    /// <summary>
    /// Reference to this NPC's HealthManager component.
    /// </summary>
    protected HealthManager hp;

    /// <summary>
    /// Reference to this NPC's state machine.
    /// </summary>
    public StateMachine stateMachine {get; protected set;}

    /// <summary>
    /// Dimensions of the box collider used for this NPC.
    /// </summary>
    public Vector3 dimensions {get; protected set;}

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    protected virtual void Awake()
    {
        nav = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();
        hp = GetComponent<HealthManager>();
        stateMachine = GetComponent<StateMachine>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    protected virtual void Start()
    {
        dimensions = coll.bounds.size;
    }

    /// <summary>
    /// Every physics frame update loop.
    /// </summary>
    protected virtual void FixedUpdate()
    {
        // If turning on an OffMeshLink, let the turn control us
        if (isTurning == true)
        {
            return;
        }

        nav.speed = stateSpeed;

        // Check for a turn
        if (isTurning == false &&
            nav.isOnOffMeshLink == true)
        {
            StartCoroutine(Turn(nav.currentOffMeshLinkData));
        }

        // Only drive forward if the next location
        // is in front of you or if we're on top of it
        float fov = GetFOV(nav.steeringTarget);
        float dist_2 = (nav.steeringTarget - transform.position).sqrMagnitude;

        if (fov < 0.5f &&
            dist_2 > (dimensions.z * dimensions.z))
        {
            // Change nav velocity, not speed here
            nav.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Determines whether or not to maintain a chase.
    /// </summary>
    /// <returns>bool</returns>
    public virtual bool Chase() {return false;}

    /// <summary>
    /// Attack method to implement.
    /// </summary>
    public virtual void Attack() {}

    /// <summary>
    /// Whether or not the target destination is visible based on the NPC's FOV,
    /// with optional distance and LOS checks.
    /// 
    /// FOV interpretation:
    /// 1 = directly in front,
    /// 0 = directly to the left/right,
    /// -1 = directly behind.
    /// 
    /// Vision interpretation:
    /// Minimum is the radius where vision is guaranteed and ignores FOV/LOS,
    /// Maximum is the radius where vision ends at.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="fovMin"></param>
    /// <param name="visionRangeMax"></param>
    /// <param name="lineOfSight"></param>
    /// <returns></returns>
    public bool CanSee(
        Vector3 target,
        float fovMin=0.5f,
        float visionRangeMin=Mathf.NegativeInfinity,
        float visionRangeMax=Mathf.Infinity,
        bool lineOfSight=false)
    {
        Vector3 diff = target - transform.position;

        // If target is too close, then guaranteed to be seen
        if (visionRangeMin > Mathf.NegativeInfinity &&
            diff.sqrMagnitude < visionRangeMin * visionRangeMin)
        {
            return true;
        }

        // Check if target is too far
        if (visionRangeMax < Mathf.Infinity &&
            diff.sqrMagnitude > visionRangeMax * visionRangeMax)
        {
            return false;
        }

        // Check if in FOV
        float fov = GetFOV(target);
        fovMin = Mathf.Clamp(
            fovMin,
            -1f,
            1f);

        if (fov < fovMin)
        {
            return false;
        }

        // Check if we have a direct line of sight
        if (lineOfSight == true)
        {
            RaycastHit hit;
            bool result = Physics.Raycast(
                transform.position,
                diff,
                out hit,
                this.visionRange);

            // Debug.DrawRay(transform.position, diff.normalized * hit.distance, Color.yellow);

            return result ? 
                hit.collider.CompareTag("Player") :
                false;
        }

        return true;
    }

    /// <summary>
    /// Return FOV metric for position from NPC's perspective.
    /// 
    /// FOV interpretation:
    /// 1 = directly in front,
    /// 0 = directly to the left/right,
    /// -1 = directly behind,
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public float GetFOV(Vector3 target)
    {
        float dot = Vector3.Dot(
            (target - transform.position).normalized,
            transform.forward);
        
        return dot;
    }

    /// <summary>
    /// Get a random point on the NavMesh.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetRandomNavMeshPoint(float range = 100f)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 direction = UnityEngine.Random.insideUnitSphere * range;
            Vector3 pos = transform.position + direction;

            NavMeshHit hit;
            bool result = NavMesh.SamplePosition(
                pos,
                out hit,
                1.0f,
                NavMesh.AllAreas);
            
            if (result == true)
            {
                return hit.position;
            }
        }

        return transform.position;
    }

    /// <summary>
    /// Conduct the designated turn.
    /// </summary>
    /// <param name="linkData"></param>
    /// <returns></returns>
    public IEnumerator Turn(OffMeshLinkData linkData)
    {
        isTurning = true;

        // Add vehicle height to turn destination
        Vector3 endPos = linkData.endPos;
        endPos.y += (dimensions.y / 2f);

        // Estimate turn time
        float startTime = Time.time;
        float duration_s = (endPos - transform.position).magnitude / nav.speed;

        while (transform.position != endPos)
        {
            // Immediately stop if we crashed
            if (isCrashed == true)
            {
                break;
            }

            // Rotate continuously unless we are on top of the end position
            Vector3 diff = endPos - transform.position;

            if (diff.sqrMagnitude > dimensions.z * dimensions.z)
            {
                Quaternion lookRot = Quaternion.LookRotation(
                    diff,
                    Vector3.up);

                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    lookRot,
                    (Time.time - startTime) / (4 * duration_s));
            }

            if (CanSee(endPos) == true)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    endPos,
                    nav.speed * Time.deltaTime);
            }

            yield return null;
        }

        if (nav.enabled == true)
        {
            nav.CompleteOffMeshLink();
        }

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

        hp.Hit(
            rb,
            force);

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