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

    /// <summary>
    /// Time that the player can't endure another collision after one.
    /// </summary>
    private const float COLLISION_ENDURANCE_TIME_S = 0.5f;

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
    /// Packages that player has collected.
    /// </summary>
    public List<Package> packages {get; private set;}

    /// <summary>
    /// Reference to Rigidbody.
    /// </summary>
    public Rigidbody rb {get; private set;}
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
    }    

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        Cursor.visible = false;
        packages = new List<Package>();
    }

    /// <summary>
    /// Every frame update loop.
    /// </summary>
    private void Update()
    {
        // Old Input System (remove when new input system is in place)
        HandleInput();

        // New Input System
        InputSystemCalls();
    }

    

    private void FixedUpdate()
    {
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
        if (Time.time - lastCollisionTime_s <= COLLISION_ENDURANCE_TIME_S)
        {
            return;
        }

        RaycastHit hit;
        bool result = rb.SweepTest(
            direction,
            out hit,
            distance);

        // TODO:
        // Update this to account for any highspeed collisions with non-static
        // objects
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

            float force = (myVelocityDot + theirVelocityDot) / 3f;

            rb.AddForce(
                hit.normal * force,
                ForceMode.Impulse);

            StartCoroutine(npc.Crash(-hit.normal * force));

            lastCollisionTime_s = Time.time;
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


        //////////////////////////////
        // VEHICLE WHEEL ANIMATIONS //
        //////////////////////////////

        // Rotate wheels while in motion
        float rotationSpeed = rb.velocity.magnitude * wheelRotationSpeed;
        foreach (GameObject wheel in frontWheels)
        {
            //wheel.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        }
        foreach (GameObject wheel in backWheels)
        {
            wheel.transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
        }

        // Turn the front wheels
        float rotationAngle = turnVal * wheelTurnSpeed * Time.deltaTime;
        foreach (GameObject wheel in frontWheels)
        {
            Vector3 currentRotation = wheel.transform.localRotation.eulerAngles;
            float currentRotationY = currentRotation.y;

            // Calculate the new rotation angle
            float newRotationY = currentRotationY + rotationAngle;

            // Adjust the range from 0-360 to -180-180
            // If not in place, issues occur when turning left
            if (newRotationY > 180)
            {
                newRotationY -= 360;
            }

            // Clamp the new rotation within the desired range
            newRotationY = Mathf.Clamp(newRotationY, -maxTurnAngle, maxTurnAngle);
            wheel.transform.localRotation = Quaternion.Euler(currentRotation.x, newRotationY, currentRotation.z);
        }

        // Return front wheels back to neutral position when not turning
        if (turnVal == 0)
        {
            foreach (GameObject wheel in frontWheels)
            {
                wheel.transform.localRotation = Quaternion.Lerp(wheel.transform.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
            }
        }

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
            packages.Count > 0)
        {
            DropPackage(packages[0]);
        }
    }

    /// <summary>
    /// Add package.
    /// </summary>
    /// <param name="pkg"></param>
    public void CollectPackage(Package pkg)
    {
        if (packages.Contains(pkg))
        {
            return;
        }

        packages.Add(pkg);
        pkg.Collect(transform);
    }

    /// <summary>
    /// Drop a package.
    /// </summary>
    /// <param name="pkg"></param>
    /// <param name="thief"></param>
    public void DropPackage(
        Package pkg,
        Transform thief=null)
    {
        if (packages.Contains(pkg) == false)
        {
            return;
        }

        packages.Remove(pkg);

        if (thief == null)
        {
            pkg.Drop();
        }
        else
        {
            pkg.Collect(thief);
        }
    }
}
