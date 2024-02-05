namespace Darkan.StateMachine.Component
{
    using Sirenix.OdinInspector;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class StateMachine<TEnum, TMachine> : MonoBehaviour where TEnum : Enum where TMachine : StateMachine<TEnum, TMachine>
    {
        public event Action<TEnum> OnStateChanged;

        protected Dictionary<TEnum, BaseState<TEnum, TMachine>> StatesDictionary = new();

        [ShowInInspector]
        [ReadOnly]
        public TEnum ActiveState => ActiveStateComponent != null ? ActiveStateComponent.State : default;

        [ShowInInspector]
        [ReadOnly]
        protected BaseState<TEnum, TMachine> ActiveStateComponent;

        protected virtual void Awake()
        {
            BaseState<TEnum, TMachine>[] states = GetComponents<BaseState<TEnum, TMachine>>();

            foreach (BaseState<TEnum, TMachine> state in states)
            {
                StatesDictionary.Add(state.State, state);

                state.Init((TMachine)this);
            }
        }

        protected virtual void Start()
        {
            TEnum entryState = SetEntryState();

            ActiveStateComponent = StatesDictionary[entryState];
            ActiveStateComponent.EnterState();
            ActiveStateComponent.enabled = true;
            OnStateChanged?.Invoke(entryState);
        }

        public abstract TEnum SetEntryState();

        public void TransitionToState(TEnum nextState)
        {
            ActiveStateComponent.ExitState();
            ActiveStateComponent.enabled = false;

            ActiveStateComponent = StatesDictionary[nextState];

            ActiveStateComponent.EnterState();
            ActiveStateComponent.enabled = true;

            OnStateChanged?.Invoke(nextState);
        }

        void OnApplicationQuit()
        {
            ActiveStateComponent.ExitState();
        }
    }
}