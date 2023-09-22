using System;
using UnityEngine;

public class ChaseState : BaseState
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="npc"></param>
    public ChaseState(NPC npc) : base(npc) {}

    /// <summary>
    /// Update cycle.
    /// </summary>
    /// <returns></returns>
    public override Type Update()
    {
        if (PlayerController.instance == null)
        {
            return null;
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