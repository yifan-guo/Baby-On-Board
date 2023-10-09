using System;
using UnityEngine;
using UnityEngine.AI;

public class IdleState : BaseState
{
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
        if (enemy != null && enemy.inCooldown) {
            return typeof(ApprehendedState);
        }

        if (PlayerController.instance != null)
        {
            // TODO:
            // provide transitions to other states
            if (me.role == NPC.Role.Bandit &&
                PlayerController.instance.pc.packages.Count > 0)
            {
                return typeof(ChaseState);
            }
        }

        if (me.nav.remainingDistance < 2f)
        {
            Vector3 pos = GetRandomNavMeshPoint();
            me.nav.speed = 10f;
            me.nav.SetDestination(pos);
        }

        return null;
    }

    /// <summary>
    /// Get a random point on the NavMesh.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomNavMeshPoint()
    {
        float wanderRange = 50f;

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
                return hit.position;
            }
        }

        return Vector3.zero;
    }
}