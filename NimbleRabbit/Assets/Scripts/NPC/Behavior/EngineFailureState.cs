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
    /// Whether or not engine has been restarted.
    /// </summary>
    private bool engineStarted;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="npc"></param>
    public EngineFailureState(NPC npc) : base(npc) 
    {
        engineStarted = false;
        startTime = -1f;

        me.stateMachine.OnStateChanged += Reset;
    }

    /// <summary>
    /// Resets this state.
    /// </summary>
    /// <param name="state"></param>
    private void Reset(BaseState state)
    {
        if (state is not EngineFailureState)
        {
            return;
        }

        engineStarted = false;
        startTime = Time.time;
        me.na.StopEngine();
    }

    /// <summary>
    /// Update cycle.
    /// </summary>
    /// <returns></returns>
    public override Type Update()
    {
        float elapsed_s = Time.time - startTime;

        // Play startup sound when we're about to finish this state
        if (engineStarted == false &&
            DURATION_S - elapsed_s <= me.na.startup.length)
        {
            engineStarted = true;
            me.na.PlayStartup();
        }

        // relax until time resets
        if (elapsed_s > DURATION_S)
        {
            me.inCooldown = false; // reset the cooldown flag
            me.na.PlayEngine();
            return typeof(IdleState);
        }

        return null;
    }
}