namespace Darkan.StateMachine
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// To use: Derive a StateManager from this -> Create a new enum for the states -> 
    /// Create the state classes and derive them from <see cref="BaseState{TEnum}"/>. <br/>
    /// Instantiate the states in the Manager on awake and add them into the <see cref="States"/> Dictionary -> <br/>
    /// Invoking the <see cref="BaseState{TEnum}.OnNextState"/> event will automatically transition to the state passed in as an argument.<br/>
    /// To use Collision functions simply do: OnTriggerEnter(Collision collision) { ActiveState.OnTriggerEnter(collision) }
    /// </summary>
    public abstract class StateManager<TEnum> : MonoBehaviour where TEnum : Enum
    {
        protected Dictionary<TEnum, BaseState<TEnum>> States = new();

        protected BaseState<TEnum> ActiveState;

        void Start()
        {
            ActiveState.OnNextState += TransitionToState;
            ActiveState.Enter();
        }

        void OnDestroy()
        {
            ActiveState.OnNextState -= TransitionToState;
        }

        void Update()
        {
            ActiveState.Update();
        }

        void TransitionToState(TEnum nextState)
        {
            ActiveState.OnNextState -= TransitionToState;
            ActiveState.Exit();

            ActiveState = States[nextState];

            ActiveState.OnNextState += TransitionToState;
            ActiveState.Enter();
        }
    }
}