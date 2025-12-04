using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Darkan.Systems.StateMachine.Component.New
{
    public abstract class StateMachine<TEnum, TMachine> : MonoBehaviour where TEnum : Enum where TMachine : StateMachine<TEnum, TMachine>
    {
        public event Action<TEnum> OnStateChanged;

        protected Dictionary<TEnum, MonoBehaviour> _componentDictionary = new();

        [ShowInInspector, ReadOnly]
        public TEnum ActiveStateType => _activeState != null ? _activeState.StateType : default;

        [ShowInInspector, ReadOnly]
        public MonoBehaviour ActiveStateComponent;
        public abstract TEnum EntryState { get; }
        public TEnum LastStateType { get; private set; }

        readonly List<IState<TMachine, TEnum>> _states = new();
        IState<TMachine, TEnum> _activeState;

        protected virtual void Awake()
        {
            var components = GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour component in components)
            {
                if (component is IState<TMachine, TEnum> iState)
                {
                    _states.Add(iState);
                    component.enabled = false;
                    _componentDictionary.Add(iState.StateType, component);
                    iState.Initialize((TMachine)this);
                }
            }
        }

        protected virtual void Start()
        {
            foreach (var state in _states)
                state.Start();
        }

        public void TransitionToState(TEnum nextState)
        {
            if (_componentDictionary.TryGetValue(nextState, out MonoBehaviour component))
            {
                if (_activeState != null)
                {
                    _activeState.Exit();
                    ActiveStateComponent.enabled = false;
                    LastStateType = _activeState.StateType;
                }

                ActiveStateComponent = component;

                _activeState = (IState<TMachine, TEnum>)component;
                ActiveStateComponent.enabled = true;
                _activeState.Enter();

                OnStateChanged?.Invoke(nextState);
            }
        }
    }
}