using System;
using UnityEngine;

public abstract class BaseState
{
    /// <summary>
    /// Reference to NPC using this state.
    /// </summary>
    protected NPC me;

    /// <summary>
    /// Reference to PlayerController singleton instance.
    /// </summary>
    protected PlayerController player;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public BaseState(NPC npc)
    {
        me = npc;
        player = PlayerController.instance;
    }

    /// <summary>
    /// Update fnc that needs to be implemented by children.
    /// </summary>
    /// <returns></returns>
    public abstract Type Update();
}