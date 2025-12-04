namespace Darkan.Systems.StateMachine.Component
{
    using System;
    using System.Collections.Generic;
    using Sirenix.OdinInspector;
    using UnityEngine;

    public abstract class StateMachine<TEnum, TMachine> : MonoBehaviour where TEnum : Enum where TMachine : StateMachine<TEnum, TMachine>
    {
        #region Serialized Fields

        [ShowInInspector, ReadOnly]
        public MonoBehaviour ActiveStateComponent;

        #endregion

        public event Action<TEnum> OnStateChanged;

        [ShowInInspector, ReadOnly]
        public TEnum ActiveStateType => _activeState != null ? _activeState.StateType : default;

        public abstract TEnum EntryState { get; }
        public TEnum LastStateType { get; private set; }

        protected Dictionary<TEnum, MonoBehaviour> _componentDictionary = new();
        IState<TMachine, TEnum> _activeState;

        #region Unity Methods

        protected virtual void Awake()
        {
            MonoBehaviour[] components = GetComponents<MonoBehaviour>();

            foreach (MonoBehaviour component in components)
                if (component is IState<TMachine, TEnum> iState)
                {
                    component.enabled = false;
                    _componentDictionary.Add(iState.StateType, component);
                    iState.Initialize((TMachine)this);
                }
        }

        #endregion

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