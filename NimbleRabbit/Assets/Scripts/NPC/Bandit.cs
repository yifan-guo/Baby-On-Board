using System;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : NPC 
{
    /// <summary>
    /// Initialization Pt I.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    protected void Start()
    {
        base.Awake();

        Dictionary<Type, BaseState> states = new Dictionary<Type, BaseState>
        {
            {typeof(ChaseState), new ChaseState(this)}
        };

        stateMachine.SetStates(states);
    }
}