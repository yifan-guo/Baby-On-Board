using System;
using System.Collections.Generic;
using UnityEngine;

public class Civilian : NPC
{
    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>
        {
            {typeof(IdleState), new IdleState(this)},
        };

        stateMachine.SetStates(states);
    }
}