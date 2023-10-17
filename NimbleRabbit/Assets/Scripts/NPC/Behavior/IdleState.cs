using System;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : BaseState
{
    /// <summary>
    /// Number of paths that will be tried before agent won't follow traffic flow.
    /// </summary>
    private const int PATIENCE_LIMIT = 5;

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
    public IdleState(NPC npc) : base(npc) {}

    /// <summary>
    /// Update cycle.
    /// </summary>
    /// <returns></returns>
    public override Type Update()
    {
        if (me.nav.enabled == false)
        {
            return null;
        }
        
        Enemy enemy = me.GetComponent<Enemy>();
        if (enemy != null && 
            enemy.inCooldown) 
        {
            return typeof(ApprehendedState);
        }

        if (PlayerController.instance != null)
        {
            if (me.role == NPC.Role.Bandit &&
                PlayerController.instance.pc.packages.Count > 0)
            {
                return typeof(ChaseState);
            }
        }

        if (me.nav.pathPending == false)
        {
            // Check how many paths drive against the traffic flow by not
            // containing an OffMeshLink
            failedPathCount = me.nav.nextOffMeshLinkData.valid ?
                0 :
                Math.Min(
                    failedPathCount + 1,
                    PATIENCE_LIMIT + 1);

            // We will stop obeying traffic if our patience has run out
            me.followsTraffic = failedPathCount <= PATIENCE_LIMIT;

            // We will only wait for a new path if we are following traffic
            waitingOnPath = (
                me.nav.nextOffMeshLinkData.valid == false &&
                me.followsTraffic == true);

            // Get a new destination if new path doesn't have an OffMeshLink or
            // if we reach the destination
            if (waitingOnPath == true ||
                me.nav.remainingDistance < 2f)
            {
                Vector3 pos = GetRandomNavMeshPoint();
                me.nav.speed = 10f;
                me.nav.SetDestination(pos);
            }
        }

        return null;
    }

    /// <summary>
    /// Get a random point on the NavMesh.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomNavMeshPoint()
    {
        const float wanderRange = 100f;

        for (int i = 0; i < 30; i++)
        {
            Vector3 direction = UnityEngine.Random.insideUnitSphere * wanderRange;
            Vector3 pos = me.transform.position + direction;

            NavMeshHit hit;
            bool result = NavMesh.SamplePosition(
                pos,
                out hit,
                1.0f,
                NavMesh.AllAreas);
            
            if (result == true)
            {
                waitingOnPath = true;
                return hit.position;
            }
        }

        return me.transform.position;
    }
}