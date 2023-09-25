using System;
using UnityEngine;

public class ChaseState : BaseState
{
    /// <summary>
    /// Reference to Enemy NPC using this state.
    /// </summary>
    protected new Enemy me;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="npc"></param>
    public ChaseState(Enemy npc) : base(npc)
    {
        me = npc;
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

        // If there's no player, just idle
        if (PlayerController.instance == null)
        {
            return typeof(IdleState);
        }

        // Attack if we can
        float dist = Vector3.Distance(
            me.transform.position,
            PlayerController.instance.transform.position);

        if (dist < me.attackRange)
        {
            return typeof(AttackState);
        }

        // Do we still want to chase
        if (me.KeepChasing() == false)
        {
            return typeof(IdleState);
        }

        // TODO:
        // speed should dynamically change depending on stuff probably
        // maybe like turns etc. need to see how much is handled by
        // the navmeshagent component
        me.nav.speed = me.moveSpeed;
        me.nav.SetDestination(PlayerController.instance.transform.position);

        return null;
    }
}