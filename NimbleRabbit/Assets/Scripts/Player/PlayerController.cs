using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Static instance of player.
    /// </summary>
    public static PlayerController instance {get; private set;}

    /// <summary>
    /// Packages that player has collected.
    /// </summary>
    private List<Package> packages;

    /// <summary>
    /// Reference to Rigidbody.
    /// </summary>
    private Rigidbody rb;

    [Header("Driving")]
    public float forwardSpeed;
    public float backwardsSpeed;
    public float turnSpeed;

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
        pkg.Collect();
    }

    /// <summary>
    /// Drop a package.
    /// </summary>
    /// <param name="pkg"></param>
    public void DropPackage(Package pkg)
    {
        if (packages.Contains(pkg) == false)
        {
            return;
        }

        packages.Remove(pkg);
        pkg.Drop();
    }
}
