using System;
using UnityEngine;

public abstract class BaseState
{
    /// <summary>
    /// Reference to NPC using this state.
    /// </summary>
    protected NPC me;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public BaseState(NPC npc)
    {
        me = npc;
    }

    /// <summary>
    /// Update fnc that needs to be implemented by children.
    /// </summary>
    /// <returns></returns>
    public abstract Type Update();
}