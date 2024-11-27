namespace Darkan.StateMachine.Component
{
    using Sirenix.OdinInspector;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class StateMachine<TEnum, TMachine> : MonoBehaviour where TEnum : Enum where TMachine : StateMachine<TEnum, TMachine>
    {
        public event Action<TEnum> OnStateChanged;

        protected Dictionary<TEnum, StateBase<TEnum, TMachine>> StatesDictionary = new();

        [ShowInInspector]
        [ReadOnly]
        public TEnum ActiveState => ActiveStateComponent != null ? ActiveStateComponent.State : default;

        [ShowInInspector]
        [ReadOnly]
        protected StateBase<TEnum, TMachine> ActiveStateComponent;

        protected virtual void Awake()
        {
            var states = GetComponents<StateBase<TEnum, TMachine>>();

            foreach (StateBase<TEnum, TMachine> state in states)
            {
                StatesDictionary.Add(state.State, state);

                state.Init((TMachine)this);
                state.enabled = false;
            }
        }

        protected virtual void Start()
        {
            TEnum entryState = SetEntryState();

            if (StatesDictionary.TryGetValue(entryState, out var state))
            {
                ActiveStateComponent = state;
                ActiveStateComponent.EnterState();
                ActiveStateComponent.enabled = true;

                OnStateChanged?.Invoke(entryState);
            }
        }

        public abstract TEnum SetEntryState();

        public void TransitionToState(TEnum nextState)
        {
            if (ActiveStateComponent != null)
            {
                ActiveStateComponent.ExitState();
                ActiveStateComponent.enabled = false;
            }

            if (StatesDictionary.TryGetValue(nextState, out var state))
            {
                ActiveStateComponent = state;
                ActiveStateComponent.EnterState();
                ActiveStateComponent.enabled = true;

                OnStateChanged?.Invoke(nextState);
            }
        }
    }
}