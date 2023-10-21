using System;
using UnityEngine;

public class EngineFailureState : BaseState
{
    /// <summary>
    /// Duration of state.
    /// </summary>
    private const float DURATION_S = 10f;

    /// <summary>
    /// Time this state began.
    /// </summary>
    private float startTime;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="npc"></param>
    public EngineFailureState(NPC npc) : base(npc) {}

    /// <summary>
    /// Update cycle.
    /// </summary>
    /// <returns></returns>
    public override Type Update()
    {
        if (startTime == 0f) 
        {
            startTime = Time.time;
        }

        // relax until time resets
        if (Time.time > (startTime + DURATION_S)) 
        {
            me.inCooldown = false; // reset the cooldown flag
            startTime = 0f;
            return typeof(IdleState);
        }

        return null;
    }
}