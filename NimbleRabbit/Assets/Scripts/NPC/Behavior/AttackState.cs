using System;
using UnityEngine;

public class AttackState : BaseState
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="npc"></param>
    public AttackState(NPC npc) : base(npc) {}

    /// <summary>
    /// Update cycle.
    /// </summary>
    /// <returns></returns>
    public override Type Update()
    {
        if (me.CanSee(
                player.transform.position,
                visionRangeMax: me.attackRange) == false)
        {
            return typeof(IdleState);
        }

        me.Attack();

        return null;
    }
}