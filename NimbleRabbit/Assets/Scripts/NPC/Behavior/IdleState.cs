using System;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : BaseState
{
    /// <summary>
    /// Number of paths that will be tried before agent won't follow traffic flow.
    /// </summary>
    private const int PATIENCE_LIMIT = 5;

    private const float IDLE_MOVE_SPEED = 10f;

    /// <summary>
    /// Whether or not the NPC will follow traffic flow.
    /// </summary>
    private bool isTrafficCompliant;

    /// <summary>
    /// Number of consecutively tested paths that don't follow traffic.
    /// </summary>
    private int failedPathCount;

    /// <summary>
    /// When a destination has been chosen, but path is still being calculated.
    /// </summary>
    private bool waitingOnPath;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="npc"></param>
    public IdleState(NPC npc) : base(npc) 
    {
        npc.stateMachine.OnStateChanged += Reset;
    }

    /// <summary>
    /// Reset Idle state.
    /// </summary>
    /// <param name="state"></param>
    private void Reset(BaseState state)
    {
        if (state is not IdleState)
        {
            return;
        }

        isTrafficCompliant = true;
        waitingOnPath = true;
        failedPathCount = 0;

        me.stateSpeed = IDLE_MOVE_SPEED;
    }

    /// <summary>
    /// Update cycle.
    /// </summary>
    /// <returns></returns>
    public override Type Update()
    {
        // If we're in cooldown, engine has failed for whatever reason
        if (me.inCooldown == true)
        {
            return typeof(EngineFailureState);
        }

        if (me.role == NPC.Role.Bandit)
        {
            Bandit bandit = (Bandit) me;
            if (bandit.pc.packages.Count > 0)
            {
                return typeof(FleeState);
            }
        }

        // See if we want to chase
        if (me.Chase() == true)
        {
            Debug.Log("going to Chase state");
            return typeof(ChaseState);
        }

        // Do nothing if navigation is disabled
        if (me.nav.enabled == false)
        {
            return null;
        }

        // Do nothing until the path is calculated
        if (me.nav.pathPending == true)
        {
            return null;
        }

        // When a path has just been calculated,
        // waitingOnPath will still be true
        if (waitingOnPath == true)
        {
            // Check how many paths drive against the traffic flow by not
            // containing an OffMeshLink
            failedPathCount = me.nav.nextOffMeshLinkData.valid ?
                0 :
                Math.Min(
                    failedPathCount + 1,
                    PATIENCE_LIMIT + 1);

            // We will stop obeying traffic if our patience has run out
            isTrafficCompliant = failedPathCount <= PATIENCE_LIMIT;

            // Wait for a new path if we are following traffic
            waitingOnPath = (
                me.nav.nextOffMeshLinkData.valid == false &&
                isTrafficCompliant == true);
        }

        // Get a new destination if new path doesn't have an OffMeshLink or
        // if we reach the destination
        if (waitingOnPath == true ||
            me.nav.remainingDistance < (me.dimensions.z / 2f))
        {
            Vector3 pos = me.GetRandomNavMeshPoint();
            if (pos != me.transform.position)
            {
                waitingOnPath = true;
            }

            me.nav.SetDestination(pos);
        }

        return null;
    }
}