using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

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
    public InputAction quitGame;
    public InputAction pauseGame;

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
    /// Audio manager for player sounds.
    /// </summary>
    public PlayerAudio pa {get; private set;}

    /// <summary>
    /// Whether or not the car is started.
    /// </summary>
    public bool started {get; private set;}

    /// <summary>
    /// Time that engine startup was initiated.
    /// </summary>
    private float startEngineTime = -1f;

    /// <summary>
    /// Time of last collision.
    /// </summary>
    private float lastCollisionTime_s;

    //bool value to enable or disable control
    public bool enableControl = true;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody>();
        hp = GetComponent<HealthManager>();
        pc = GetComponent<PackageCollector>();
        pa = GetComponent<PlayerAudio>();

        quitGame.performed += OnQuitGame;
        pauseGame.performed += OnPauseGame;
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        Cursor.visible = false;
    }

    /// <summary>
    /// Physics update loop.
    /// </summary>
    private void FixedUpdate()
    {
        //if enableControl == false, then pause control
        if(!enableControl)
        {
            return;
        }

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
        quitGame.Enable();
        pauseGame.Enable();
        turn.Enable();
    }

    private void OnDisable()
    {
        accelerate.Disable();
        decelerate.Disable();
        quitGame.Disable();
        pauseGame.Disable();
        turn.Disable();
    }

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
        float isAccelerating = accelerate.ReadValue<float>();
        float isDecelerating = decelerate.ReadValue<float>();
        float turnVal = turn.ReadValue<float>();

        // Startup mechanism
        if (started == false)
        {
            if (isAccelerating > 0f)
            {
                if (startEngineTime < 0f)
                {
                    pa.PlayStartup();
                    startEngineTime = Time.time;
                }

                started = (Time.time - startEngineTime) > PlayerAudio.STARTUP_TIME_S;
            }
            else
            {
                pa.StopEngine();
                startEngineTime = -1f;
            }

            if (started == true)
            {
                pa.PlayEngine();
            }

            return;
        }

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

    void OnQuitGame(InputAction.CallbackContext context)
    {

        Debug.Log("Quit action fired");
        Debug.Log($"context.Interaction: {context.interaction}");
        if (context.interaction is HoldInteraction)
        {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }
    }

    void OnPauseGame(InputAction.CallbackContext context)
    {
        Debug.Log("Pause action fired");
        UIManager.instance.ToggleSettingsMenu();
    }
}
