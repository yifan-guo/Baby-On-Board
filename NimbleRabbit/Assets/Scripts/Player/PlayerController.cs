using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of player.
    /// </summary>
    public static PlayerController instance {get; private set;}

    [Header("Input")]
    public InputAction accelerate;
    public InputAction decelerate;
    public InputAction turn;

    [Header("Driving")]
    public float forwardSpeed;
    public float backwardsSpeed;
    public float turnSpeed;
    public float maxSpeed;

    [Header("Animation Objects")]
    public List<GameObject> frontWheels;
    public List<GameObject> backWheels;

    [Header("Animation Controls")]
    public float wheelRotationSpeed;
    public float wheelTurnSpeed;
    public float returnSpeed;
    public float maxTurnAngle;

    /// <summary>
    /// Flag for if player is stable on the ground.
    /// </summary>
    public bool isGrounded {get; private set;}

    /// <summary>
    /// Reference to Rigidbody.
    /// </summary>
    public Rigidbody rb {get; private set;}
    /// <summary>
    /// Reference to HealthManager.
    /// </summary>
    public HealthManager hp {get; private set;}

    /// <summary>
    /// Packages that player has collected.
    /// </summary>
    public PackageCollector pc {get; private set;}

    /// <summary>
    /// Time of last collision.
    /// </summary>
    private float lastCollisionTime_s;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        hp = GetComponent<HealthManager>();
        pc = GetComponent<PackageCollector>();
    }    

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        Cursor.visible = false;
    }

    /// <summary>
    /// Every frame update loop.
    /// </summary>
    private void Update()
    {
        // Old Input System (remove when new input system is in place)
        HandleInput();
    }



    private void FixedUpdate()
    {
        // New Input System
        InputSystemCalls();

        // Forward
        SweepTest(
            transform.forward,
            (transform.localScale.z / 2f) + 0.1f);

        // Backward
        SweepTest(
            -transform.forward,
            (transform.localScale.z / 2f) + 0.1f);

        // Right
        SweepTest(
            transform.right,
            (transform.localScale.x / 2f) + 0.1f);

        // Left
        SweepTest(
            -transform.right,
            (transform.localScale.x / 2f) + 0.1f);
    }


    // OnEnable/OnDisable for Unity's new input system
    private void OnEnable()
    {
        accelerate.Enable();
        decelerate.Enable();
        turn.Enable();
    }

    private void OnDisable()
    {
        accelerate.Disable();
        decelerate.Disable();
        turn.Disable();
    }

    /// <summary>
    /// Physics update loop.
    /// </summary>


    /// <summary>
    /// Use sweep test for fast collisions.
    /// </summary>
    /// <param name="direction"></param>
    private void SweepTest(
        Vector3 direction,
        float distance)
    {
        if (Time.time - lastCollisionTime_s <= HealthManager.PHYSICAL_DAMAGE_DEBOUNCE_S)
        {
            return;
        }

        RaycastHit hit;
        bool result = rb.SweepTest(
            direction,
            out hit,
            distance);

        if (result == true &&
            hit.collider.gameObject.tag == "NPC")
        {
            NPC npc = hit.transform.GetComponent<NPC>();

            float myVelocityDot = Vector3.Dot(
                rb.velocity,
                hit.transform.position - transform.position);

            float theirVelocityDot = Vector3.Dot(
                npc.nav.velocity,
                transform.position - hit.transform.position);

            float forceMagnitude = (myVelocityDot + theirVelocityDot) / 3f;

            hp.Hit(
                rb,
                hit.normal * forceMagnitude);

            StartCoroutine(npc.Crash(-hit.normal * forceMagnitude));

            lastCollisionTime_s = Time.time;

            ReclaimPackage(npc);
        }
    }

    private void InputSystemCalls()
    {
        // WORK IN PROGRESS
        // STILL NEEDS POLISH
        float isAccelerating = accelerate.ReadValue<float>();
        float isDecelerating = decelerate.ReadValue<float>();
        float turnVal = turn.ReadValue<float>();

        ///////////////////////////////////////
        // VEHICLE ACCELERATION AND TURNING //
        //////////////////////////////////////


        // Adjust the velocity based on acceleration and deceleration
        rb.velocity += transform.forward * forwardSpeed * (isAccelerating - isDecelerating) * Time.deltaTime;

        // Calculate a normalized value based on current speed and modify turn speed based on the inverse of the normalized speed
        float speedNormalized = rb.velocity.magnitude / maxSpeed;
        float modifiedTurnSpeed = turnSpeed * (1.0f - speedNormalized);

        // Apply turning only when moving forward or backward
        if (turnVal != 0 && rb.velocity.magnitude > 0.1f)
        {

            if (isAccelerating == 0 && isDecelerating > 0)
            {
                transform.Rotate(Vector3.up, -turnVal * modifiedTurnSpeed * Time.deltaTime);
            }
            else
            {
                transform.Rotate(Vector3.up, turnVal * modifiedTurnSpeed * Time.deltaTime);
            }
        }

    }

    /// <summary>
    /// Reclaim the package from the colliding object if it has a PackageManager
    /// </summary>
    private void ReclaimPackage(NPC npc) 
    {
        // get the package from the Bandit
        PackageCollector otherPc = npc.GetComponent<PackageCollector>();

        if (otherPc == null) 
        {
            return;
        }

        List<Package> pkgs = otherPc.packages;

        if (pkgs.Count == 0)
        {
            return;
        }

        npc.inCooldown = true;

        // Randomly pick any package the colliding object has
        int pkgIdx = UnityEngine.Random.Range(
            0, 
            pkgs.Count);

        Package stolenPackage = pkgs[pkgIdx];

        Indicator.Untrack(npc.gameObject);
        
        // make the NPC give the package to the Player
        otherPc.DropPackage(
            stolenPackage,
            this.pc);
    }

    /// <summary>
    /// Interprets user input.
    /// REMOVE ONCE NEW INPUT SYSTEM IS FUNCTIONING
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetKeyDown("q"))
        {
            Application.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.ToggleSettingsMenu();
        }

        // TODO:
        // This is test code to drop the package.
        // Ideally DropPackage() is called elsewhere by something like an enemy.
        if (Input.GetKeyDown(KeyCode.Space) &&
            this.pc.packages.Count > 0)
        {
            this.pc.DropPackage(this.pc.packages[0]);
        }
    }
}
