using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Static instance of player.
    /// </summary>
    public static PlayerController instance {get; private set;}

    [Header("Driving")]
    public float forwardSpeed;
    public float backwardsSpeed;
    public float turnSpeed;

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
    /// Time that the player can't endure another collision after one.
    /// </summary>
    private const float COLLISION_ENDURANCE_TIME_S = 0.5f;

    /// <summary>
    /// Time of last collision.
    /// </summary>
    private float lastCollisionTime_s;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }    

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        Cursor.visible = false;
        instance = this;
        packages = new List<Package>();
    }

    /// <summary>
    /// Every frame update loop.
    /// </summary>
    private void Update()
    {
        HandleInput();
    }

    /// <summary>
    /// Physics update loop.
    /// </summary>
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

    /// <summary>
    /// Interprets user input.
    /// </summary>
    private void HandleInput()
    {
        if (Input.GetKeyDown("q"))
        {
            Application.Quit();
        }
        if (Input.GetKey("w"))
        {
            rb.velocity += transform.forward * forwardSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s"))
        {
            rb.velocity -= transform.forward * backwardsSpeed * Time.deltaTime;
        }

        // Turning left
        if (Input.GetKey("a"))
        {
            transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
        }

        // Turning right
        if (Input.GetKey("d"))
        {
            transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
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
