using System;
using UnityEngine;

public class ChaseState : BaseState
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="npc"></param>
    public ChaseState(NPC npc) : base(npc)
    {
        me.stateMachine.OnStateChanged += Reset;
    }

    /// <summary>
    /// Reset Chase state.
    /// </summary>
    /// <param name="state"></param>
    private void Reset(BaseState state)
    {
        if (state is not ChaseState)
        {
            return;
        }

        me.stateSpeed = me.maxSpeed;
    }

    /// <summary>
    /// Update cycle.
    /// </summary>
    /// <returns></returns>
    public override Type Update()
    {
        // Relax until navigation is back up
        if (me.nav.enabled == false)
        {
            return null;
        }

        // Attack if we can
        float dist = Vector3.Distance(
            me.transform.position,
            player.transform.position);

        if (dist < me.attackRange)
        {
            return typeof(AttackState);
        }

        // Continuously update chase location
        if (me.Chase() == true)
        {
            // Debug.Log("setting destination to player");
            me.nav.SetDestination(player.transform.position);
        }

        // If we lost sight of the player and have reached the 
        // last known position, then give up
        if (me.nav.remainingDistance < 0.1f)
        {
            return typeof(IdleState);
        }

        return null;
    }
}