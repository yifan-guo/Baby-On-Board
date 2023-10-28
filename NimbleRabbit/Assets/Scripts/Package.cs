using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(HealthManager))]
public class Package : Collectible, IObjective
{
    /// <summary>
    /// Whether the package has been picked up or not.
    /// </summary>
    public bool isCollected {get; private set;}

    /// <summary>
    /// Reference to HealthManager component.
    /// </summary>
    public HealthManager hp {get; private set;}

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        hp = GetComponent<HealthManager>();
    }

    public GameObject deliveryLocation;

    public const float DELIVERY_RADIUS = 10f;

    public float TTLAfterPickupSeconds = 12f;

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    protected void Start()
    {
        isCollected = false;
        Indicator.Track(gameObject);
        // Subscribe to health change events to check if package is destroyed.
        hp.OnHealthChange += ((IObjective)this).CheckFailure;
    }

    /// <summary>
    /// Trigger handler that decides what happens.
    /// </summary>
    /// <param name="other"></param>
    protected override void OnTriggerEnter(Collider other)
    {
        if (isCollected == true ||
            other.gameObject.tag != "Player")
        {
            return;
        }

        PlayerController.instance.pc.CollectPackage(this);
    }

    /// <summary>
    /// What happens when something leaves the trigger.
    /// </summary>
    /// <param name="other"></param>
    protected void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player")
        {
            return;
        }

        // Package is only collectable once it is no
        // longer underneath the player
        isCollected = false;
    }

    /// <summary>
    /// Alter state to be collected.
    /// </summary>
    public void Collect(Transform owner)
    {
        isCollected = true;
        coll.enabled = false;

        ((IObjective)this).StartObjective();
        // Deferred check to see if the package is delivered by TTL
        Invoke("CheckCompletionOnTTL", TTLAfterPickupSeconds + 0.01f);

        transform.parent = owner;

        // TODO:
        // Update as placeholders for player and NPCs change.
        // Don't even know if we want it to sit on top of the vehicles.

        if (owner.tag == "Player")
        {
            Indicator.Untrack(gameObject);
            transform.localPosition = new Vector3(0f, 0.35f, 0f);
        }
        else
        {
            Indicator.Track(owner.gameObject);
            transform.localPosition = Vector3.up * ((owner.GetComponent<Collider>().bounds.size.y + transform.localScale.y) / 2f);
        }
    }

    /// <summary>
    /// Alter state to be dropped.
    /// </summary>
    public void Drop()
    {
        coll.enabled = true;

        // Place on floor below owner
        Vector3 pos = transform.parent.position;
        pos.y = transform.localScale.y / 2f;
        transform.position = pos;
        transform.parent = null;

        Indicator.Track(gameObject);
    }

    public string _name = "Package";
    public string Name
    {
        get { return _name; }
    }

    public string _description;
    public string Description
    {
        get { return _description; }
    }

    private IObjective.Status _status = IObjective.Status.NotStarted;
    public IObjective.Status ObjectiveStatus
    {
        get { return _status; }
        set { _status = value; }
    }

    private float _startTime;
    public float StartTime
    {
        get { return _startTime; }
        set { _startTime = value; }
    }

    private float _endTime;
    public float EndTime
    {
        get { return _endTime; }
        set { _endTime = value; }
    }

    private List<IObjective> _prereqs = new List<IObjective>();
    public List<IObjective> prereqs
    {
        get { return _prereqs; }
    }

    private IObjective.PrereqOperator _prereqCompletionOperator = IObjective.PrereqOperator.AND;
    public IObjective.PrereqOperator prereqCompletionOperator
    {
        get { return _prereqCompletionOperator; }
    }

    private IObjective.PrereqOperator _prereqFailureOperator = IObjective.PrereqOperator.AND;
    public IObjective.PrereqOperator prereqFailureOperator
    {
        get { return _prereqFailureOperator; }
    }

    public event Action OnObjectiveUpdated;

    public bool PrimaryCompletionCondition()
    {
        if (deliveryLocation == null)
        {
            Debug.Log("No delivery location set for package.");
            return false;
        }
        else
        {
            Debug.Log("Checking distance to delivery location.");
            float distance = Vector3.Distance(transform.position, deliveryLocation.transform.position);
            Debug.Log("Distance: " + distance);
            return Vector3.Distance(transform.position, deliveryLocation.transform.position) < DELIVERY_RADIUS;
        }
    }
    public bool PrimaryFailureCondition()
    {
        bool healthFailed = false;
        Debug.Log("Checking if package is destroyed.");
        Debug.Log("Current health: " + hp.currentHealth);
        healthFailed = hp.currentHealth <= 0f;
        if (healthFailed) {
            Debug.Log("FAIL: package health below zero");
        }

        Debug.Log("Checking TTL");
        Debug.Log("TTL: " + TTLAfterPickupSeconds);
        Debug.Log("Time elapsed since start: " + ((IObjective)(this)).ElapsedDuration);
        Debug.Log("Start time: " + ((IObjective)(this)).StartTime);
        Debug.Log("Current time: " + Time.time);
        bool ttlFailed = false;
        if (isCollected)
        {
             ttlFailed = ((IObjective)(this)).ElapsedDuration > TTLAfterPickupSeconds;
        }
        if (ttlFailed) {
            Debug.Log("FAIL: package TTL exceeded");
        }

        return healthFailed || ttlFailed;

    }

    public void RaiseObjectiveUpdated()
    {
        OnObjectiveUpdated?.Invoke();
    }

    public void CheckCompletionOnTTL()
    {
        Debug.Log("Checking failure on TTL");
        ((IObjective)this).CheckFailure();
    }

}