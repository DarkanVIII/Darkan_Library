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

        [ShowInInspector]
        [ReadOnly]
        public TEnum ActiveStateType => _activeState != null ? _activeState.StateType : default;

        [ShowInInspector]
        [ReadOnly]
        public MonoBehaviour ActiveStateComponent;

        IState<TMachine, TEnum> _activeState;

        protected virtual void Awake()
        {
            MonoBehaviour[] components = GetComponents<MonoBehaviour>();


            foreach (MonoBehaviour component in components)
            {
                if (component is IState<TMachine, TEnum> iState)
                {
                    component.enabled = false;
                    _componentDictionary.Add(iState.StateType, component);
                    iState.Initialize((TMachine)this);
                }
            }
        }

        public void SetEntryState(TEnum state)
        {
            if (ActiveStateComponent != null) return;

            if (_componentDictionary.TryGetValue(state, out MonoBehaviour component))
            {
                ActiveStateComponent = component;
                _activeState = (IState<TMachine, TEnum>)component;
                ActiveStateComponent.enabled = true;
                _activeState.Enter();

                OnStateChanged?.Invoke(state);
            }
        }

        public void TransitionToState(TEnum nextState)
        {
            if (_componentDictionary.TryGetValue(nextState, out MonoBehaviour component))
            {

                _activeState.Exit();
                ActiveStateComponent.enabled = false;

                ActiveStateComponent = component;

                ActiveStateComponent = component;
                _activeState = (IState<TMachine, TEnum>)component;
                ActiveStateComponent.enabled = true;
                _activeState.Enter();

                OnStateChanged?.Invoke(nextState);
            }
        }
    }
}