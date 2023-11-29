using System;
using System.Collections.Generic;
using UnityEngine;

public class Civilian : NPC
{
    /// <summary>
    /// Initialization Pt II.
    /// </summary>
    /// 
    public override string IdleStatusText => "";
    public override string ChaseStatusText => "";
    public override string AttackStatusText => "";
    public override string FleeStatusText => "";
    public override string EngineFailureStatusText => "";
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