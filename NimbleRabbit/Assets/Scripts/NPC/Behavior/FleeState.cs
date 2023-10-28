using System;
using UnityEngine;

public class FleeState : BaseState
{
    /// <summary>
    /// Chance from 0-1 of Bandit securing the package.
    /// </summary>
    private const float BANDIT_SECURE_PKG_CHANCE = 0.33f;

    /// <summary>
    /// Reference to this NPC's PackageCollector if it has one.
    /// </summary>
    private PackageCollector pc;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="npc"></param>
    public FleeState(NPC npc) : base(npc)
    {
        pc = npc.GetComponent<PackageCollector>();

        me.stateMachine.OnStateChanged += Reset;
    }

    /// <summary>
    /// Reset Chase state.
    /// </summary>
    /// <param name="state"></param>
    private void Reset(BaseState state)
    {
        if (state is not FleeState)
        {
            return;
        }

        me.stateSpeed = me.maxSpeed;

    }

    /// <summary>
    /// Update cycle.
    /// </summary>
    public override Type Update()
    {
        // Relax until navigation is back up
        if (me.nav.enabled == false)
        {
            return null;
        }

        // Do nothing until the path is calculated
        if (me.nav.pathPending == true)
        {
            return null;
        }

        // Destination has not been reached
        if (me.nav.remainingDistance > (me.dimensions.z / 2f))
        {
            return null;
        }

        // If we are not a PackageCollector, flee once and return to Idle
        // If we are a PackageCollector, return to Idle if we have no packages
        if (pc == null ||
            pc.packages.Count == 0)
        {
            return typeof(IdleState);
        }

        float roll = UnityEngine.Random.Range(
            0f,
            1f);

        // Secure package or continue fleeing
        if (roll < BANDIT_SECURE_PKG_CHANCE)
        {
            Package pkg = pc.packages[0];
            BanditHQ.Mark(pkg);
            
            Vector3 hqPos = BanditHQ.claimedPackages[pkg].transform.position;
            me.nav.SetDestination(hqPos);
        }
        else
        {
            Vector3 dest = GetFurtherFromPlayer();
            me.nav.SetDestination(dest);
        }

        return null;
    }

    /// <summary>
    /// Get a position that takes us further from the player.
    /// </summary>
    private Vector3 GetFurtherFromPlayer()
    {
        Vector3 playerPos = PlayerController.instance.transform.position;
        Vector3 pos = me.GetRandomNavMeshPoint();
        float myDist_2 = (me.transform.position - playerPos).sqrMagnitude;
        float newDist_2 = (pos - playerPos).sqrMagnitude;

        return (newDist_2 > myDist_2) ?
            pos :
            me.transform.position;
    }
}