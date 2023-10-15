namespace Darkan.StateMachine
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// To use: Derive a StateManager from this -> Create a new enum for the states -> 
    /// Create the state classes and derive them from <see cref="BaseState{TEnum}"/>. <br/>
    /// Instantiate the states in the Manager on awake and add them into the <see cref="States"/> Dictionary -> <br/>
    /// Calling <see cref="TransitionToState(TEnum)"/> will automatically transition to the state passed in as an argument.<br/>
    /// To use Collision functions simply do: OnTriggerEnter(Collision collision) { ActiveState.OnTriggerEnter(collision) }
    /// </summary>
    public abstract class StateManager<TEnum, TManager> : MonoBehaviour where TEnum : Enum where TManager : StateManager<TEnum, TManager>
    {
        public static event Action<TEnum> OnGameStateChanged;

        protected Dictionary<TEnum, BaseState<TEnum, TManager>> States = new();

        protected BaseState<TEnum, TManager> ActiveState;

        void Start()
        {
            ActiveState.Enter();
        }

        void Update()
        {
            ActiveState.Update();
        }

        public void TransitionToState(TEnum nextState)
        {
            ActiveState.Exit();

            ActiveState = States[nextState];

            ActiveState.Enter();

            OnGameStateChanged?.Invoke(nextState);
        }
    }
}