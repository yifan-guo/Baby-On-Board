using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    /// <summary>
    /// All states included in this state machine.
    /// </summary>
    public Dictionary<Type, BaseState> states {get; private set;}
    
    /// <summary>
    /// Current active state.
    /// </summary>
    public BaseState currentState {get; private set;}

    /// <summary>
    /// Event for when state changes.
    /// </summary>
    public event Action<BaseState> OnStateChanged;

    /// <summary>
    /// Initialize state machine and optional start state.
    /// </summary>
    /// <param name="states"></param>
    /// <param name="startState"></param>
    public void SetStates(
        Dictionary<Type, BaseState> states,
        BaseState startState = null)
    {
        this.states = states;
        currentState = startState;
    }

    /// <summary>
    /// Update loop.
    /// </summary>
    private void Update()
    {
        // Default to the first
        if (currentState == null)
        {
            currentState = states.Values.First();
        }

        // Check if current state wants to change
        Type nextStateType = currentState?.Update();
        if (nextStateType != null &&
            nextStateType != currentState?.GetType())
        {
            SwitchState(nextStateType);
        }
    }

    /// <summary>
    /// Change state.
    /// </summary>
    /// <param name="nextStateType"></param>
    private void SwitchState(Type nextStateType)
    {
        currentState = states[nextStateType];
        OnStateChanged?.Invoke(currentState);
    }
}