using System;
using UnityEngine;

public class AttackState : BaseState
{
    /// <summary>
    /// Reference to Enemy NPC using this state.
    /// </summary>
    protected new Enemy me;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="npc"></param>
    public AttackState(Enemy npc) : base(npc)
    {
        me = npc;
    }

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

        if (me.KeepAttacking() == false)
        {
            return typeof(IdleState);
        }

        me.Attack();

        return null;
    }
}