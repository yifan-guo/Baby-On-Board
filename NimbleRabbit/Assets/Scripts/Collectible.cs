using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Collectible : MonoBehaviour
{
    /// <summary>
    /// Reference to collider.
    /// </summary>
    protected Collider coll;

    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    protected virtual void Awake()
    {
        coll = GetComponent<Collider>();
    }

    /// <summary>
    /// Trigger handler that decides what happens.
    /// </summary>
    /// <param name="other"></param>
    protected abstract void OnTriggerEnter(Collider other);
}