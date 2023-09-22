using System;
using UnityEngine;

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
        // TODO:
        // provide transitions to other states
        return null;
    }
}