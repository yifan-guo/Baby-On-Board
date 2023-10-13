using System;
using UnityEngine;

public class ApprehendedState : BaseState
{
    /// <summary>
    /// Reference to Enemy NPC using this state.
    /// </summary>
    protected new Enemy me;

    private float startTime;

    public float timeUntilAgitatedResets = 10f;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="npc"></param>
    public ApprehendedState(Enemy npc) : base(npc)
    {
        me = npc;
    }

    /// <summary>
    /// Update cycle.
    /// </summary>
    /// <returns></returns>
    public override Type Update()
    {
        if (startTime == 0f) {
            startTime = Time.time;
        }

        // relax until time resets
        if (Time.time > (startTime + timeUntilAgitatedResets)) {
            me.inCooldown = false; // reset the cooldown flag
            startTime = 0f;
            return typeof(IdleState);
        }

        return null;
    }
}