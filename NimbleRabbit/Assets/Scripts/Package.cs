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
    public void Collect()
    {
        isCollected = true;
        coll.enabled = false;

        transform.parent = PlayerController.instance.transform;
        transform.localPosition = new Vector3(0f, 0.35f, 0f);
    }

    /// <summary>
    /// Alter state to be dropped.
    /// </summary>
    public void Drop()
    {
        coll.enabled = true;
        transform.parent = null;

        Vector3 pos = PlayerController.instance.transform.position;
        pos.y = transform.localScale.y / 2f;
        transform.position = pos;
    }
}