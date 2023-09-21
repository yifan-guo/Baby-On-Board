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
        // TODO:
        // replace with a static instance reference to player
        if (me.target == null)
        {
            return null;
        }

        me.nav.SetDestination(me.target.transform.position);

        return null;
    }
}