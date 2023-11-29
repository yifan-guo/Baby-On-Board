using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using Cinemachine;

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
    public InputAction playerReset;
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
    /// Cinemachine camera reference.
    /// </summary>
    public Cinemachine.CinemachineFreeLook cmfl;

    /// <summary>
    /// Whether or not the car is upright.
    /// </summary>
    public bool isUpright { get; private set; }

    /// <summary>
    /// Is true when the player reset interpolation is happening
    /// </summary>
    public bool isResetting;

    /// <summary>
    /// Player reset interpolation speed
    /// </summary>
    public float resetSpeed;

    /// <summary>
    /// Force used for resetting the player vehicle
    /// </summary>
    public float upwardResetForce;

    /// <summary>
    /// The time set for the reset cooldown
    /// Can be configured in the editor
    /// </summary>
    public float cooldownTime;

    /// <summary>
    /// The timer for the cooldown variable.
    /// </summary>
    private float resetCooldownTimer;

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
    /// Look sensitivity maximum applied to camera.
    /// </summary>
    public float lookSensitivityMax = 3000f;


     /// <summary>
    /// Look sensitivity maximum applied to camera.
    /// </summary>
    public float lookSensitivityMin = 500f;


    protected Collider coll;
    
    public Vector3 dimensions {get; protected set;}

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

        accelerate.performed += CheckInputDevice;
        decelerate.performed += CheckInputDevice;
        turn.performed += CheckInputDevice;
        playerReset.performed += CheckInputDevice;
        quitGame.performed += CheckInputDevice;
        pauseGame.performed += CheckInputDevice;

        isResetting = false;

        coll = GetComponent<Collider>();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        Cursor.visible = false;
        OnLookSensitivityChanged(PlayerPrefs.GetFloat("lookSensitivity", 0.5f));
        dimensions = coll.bounds.size;
    }

    private float lastLog_s = -1f;
    private void Update()
    {
        if (Time.time - lastLog_s > AuditLogger.FREQ_S)
        {
            AuditLogger.instance.RecordPlayerData();
            lastLog_s = Time.time;
        }

        
        
    }

    /// <summary>
    /// Physics update loop.
    /// </summary>
    private void FixedUpdate()
    {
        //if enableControl == false, then pause control
        if (!enableControl)
        {
            return;
        }

        
        

        // New Input System
        InputSystemCalls();
        PlayerReset();

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

    // Resetting the player to the upright position
    private void PlayerReset()
    {

        //Clamp the range to check if car is upright
        float minRotationVal = 5f;
        float maxRotationVal = 355f;
        bool xComparison = gameObject.transform.rotation.eulerAngles.x < minRotationVal || gameObject.transform.rotation.eulerAngles.x > maxRotationVal;
        bool zComparison = gameObject.transform.rotation.eulerAngles.z < minRotationVal || gameObject.transform.rotation.eulerAngles.z > maxRotationVal;

        //Set is upright based on the clamp results
        if (xComparison && zComparison)
        {
            isUpright = true;
            //Done interpolating
            isResetting = false;
        } 
        else isUpright = false;

        //Cooldown timer
        resetCooldownTimer += Time.deltaTime;

        //Implement Reset Algorithm
        if (!isUpright && resetCooldownTimer > cooldownTime)
        {
            float resetCalled = playerReset.ReadValue<float>();
            
            // Start reset cool down timer
            if (resetCalled > 0f)
            {
                resetCooldownTimer = 0;
                // Add force in worldspace up direction
                rb.velocity += Vector3.up * upwardResetForce * Time.deltaTime;
                isResetting = true;
            }
        }

        //Interpolate current rotation with upright rotation (0, y, 0)
        if (isResetting)
        {
            // Handle player reset interpolation
            if (!isUpright)
            {
                Quaternion targetRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f); // Upright rotation
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, resetSpeed * Time.deltaTime);
            }
        }
        
    }

    // OnEnable/OnDisable for Unity's new input system
    private void OnEnable()
    {
        accelerate.Enable();
        decelerate.Enable();
        quitGame.Enable();
        pauseGame.Enable();
        turn.Enable();
        playerReset.Enable();
        UIManager.instance.settingsMenu.OnLookSensitivityChanged += OnLookSensitivityChanged;
    }

    private void OnDisable()
    {
        accelerate.Disable();
        decelerate.Disable();
        quitGame.Disable();
        pauseGame.Disable();
        turn.Disable();
        playerReset.Disable();
        UIManager.instance.settingsMenu.OnLookSensitivityChanged -= OnLookSensitivityChanged;
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

            AuditLogger.instance.ar.numCollisions++;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        AuditLogger.instance.ar.numCollisions++;
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

        // Disable input calls when vehicle is not in upright position
        if (isUpright == false) return;
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
        AuditLogger.instance.ar.numReclaimedPackages++;
    }

    void OnQuitGame(InputAction.CallbackContext context)
    {
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
        UIManager.instance.ToggleSettingsMenu();
    }

    public void OnLookSensitivityChanged(float value)
    {
        float lookSensitivity = lookSensitivityMin + (lookSensitivityMax - lookSensitivityMin) * value;
        PlayerPrefs.SetFloat("lookSensitivity", value);
        cmfl.m_XAxis.m_MaxSpeed = lookSensitivity;
        Debug.Log($"Look sensitivity changed to {lookSensitivity}");

    }
    void CheckInputDevice(InputAction.CallbackContext context){
        InputDevice device = context.control.device;
        // Assume keyboard by default
        // If we get any interaction with a gamepad, we'll set it to controller
        if (device is Gamepad)
        {
            AuditLogger.instance.ar.inputType = AttemptReport.InputType.controller;
        }
    }

    public float GetFOV(Vector3 target)
    {
        float dot = Vector3.Dot(
            (target - transform.position).normalized,
            transform.forward);
        
        return dot;
    }

    public bool CanSee(
        Vector3 target,
        float fovMin=0.5f,
        float visionRangeMin=Mathf.NegativeInfinity,
        float visionRangeMax=Mathf.Infinity,
        bool lineOfSight=false)
    {
        // Add half of our height so that this is from the center of the NPC
        Vector3 pos = transform.position + Vector3.up * (dimensions.y / 2f);
        Vector3 diff = target - pos;

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

        return true;
    }
}
