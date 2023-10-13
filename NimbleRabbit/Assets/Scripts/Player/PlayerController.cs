using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of player.
    /// </summary>
    public static PlayerController instance {get; private set;}

    /// <summary>
    /// Packages that player has collected.
    /// </summary>
    public PackageCollector pc {get; private set;}

    /// Time that the player can't endure another collision after one.
    /// </summary>
    private const float COLLISION_ENDURANCE_TIME_S = 0.5f;


    [Header("Driving")]
    public float forwardSpeed;
    public float backwardsSpeed;
    public float turnSpeed;

    /// <summary>
    /// Flag for if player is stable on the ground.
    /// </summary>
    public bool isGrounded {get; private set;}

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
        pc = GetComponent<PackageCollector>();
    }    

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    private void Start()
    {
        Cursor.visible = false;
        instance = this;
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

            ReclaimPackage(hit);
        }
    }

    /// <summary>
    /// Reclaim the package from the colliding object if it has a PackageManager
    /// </summary>
    private void ReclaimPackage(RaycastHit hit) {

        // get the package from the Bandit
        GameObject other = hit.collider.gameObject;
        PackageCollector otherPc = other.GetComponent<PackageCollector>();
        if (otherPc != null) {
            List<Package> pkgs = otherPc.packages;

            if (pkgs.Count == 0)
            {
                return;
            }

            // enable the enemy to transition to cooldown state 
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) {
                enemy.inCooldown = true;
            }

            // Randomly pick any package the colliding object has
            int pkgIdx = UnityEngine.Random.Range(0, pkgs.Count);

            Package stolenPackage = pkgs[pkgIdx];
            
            // make the NPC give the package to the Player
            otherPc.DropPackage(
                stolenPackage,
                this.pc);
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.ToggleSettingsMenu();    
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
            this.pc.packages.Count > 0)
        {
            this.pc.DropPackage(this.pc.packages[0]);
        }
    }
}
