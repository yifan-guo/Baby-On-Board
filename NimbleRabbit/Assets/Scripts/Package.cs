using UnityEngine;

public class Package : Collectible
{
    /// <summary>
    /// Whether the package has been picked up or not.
    /// </summary>
    public bool isCollected {get; private set;}

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    protected void Start()
    {
        isCollected = false;
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

        PlayerController.instance.CollectPackage(this);
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

        transform.parent = owner;

        // TODO:
        // Update as placeholders for player and NPCs change.
        // Don't even know if we want it to sit on top of the vehicles.
        transform.localPosition = (owner.tag == "Player") ?
            new Vector3(0f, 0.35f, 0f) :
            Vector3.up * ((owner.GetComponent<Collider>().bounds.size.y) + (transform.localScale.y / 2f));
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
    }
}