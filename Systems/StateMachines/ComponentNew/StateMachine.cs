using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Darkan.Systems.StateMachine.Component.New
{
    public abstract class StateMachine<T> : MonoBehaviour where T : StateMachine<T>
    {
        public event Action<MonoBehaviour> OnStateChanged;

        readonly Dictionary<Type, MonoBehaviour> _states = new();

        [ShowInInspector, ReadOnly]
        protected MonoBehaviour _activeState;

        public void SetEntryState<State>() where State : MonoBehaviour, IState<T>
        {
            if (_activeState != null) return;

            if (!_states.TryGetValue(typeof(State), out MonoBehaviour state))
                state = CreateNewStateInstance<State>();

            _activeState = state;

            ((IState<T>)_activeState).Enter();
            _activeState.enabled = true;

            OnStateChanged?.Invoke(_activeState);
        }

        /// <summary>
        /// Transitions from the current state to the specified state. The current State must not be null.
        /// </summary>
        public void TransitionToState<State>() where State : MonoBehaviour, IState<T>
        {
            _activeState.enabled = false;
            ((IState<T>)_activeState).Exit();

            if (!_states.TryGetValue(typeof(State), out MonoBehaviour state))
                state = CreateNewStateInstance<State>();

            _activeState = state;

            ((IState<T>)_activeState).Enter();
            _activeState.enabled = true;

            OnStateChanged?.Invoke(_activeState);
        }

        State CreateNewStateInstance<State>() where State : MonoBehaviour, IState<T>
        {
            State state = gameObject.AddComponent<State>();
            (state as IState<T>)?.Initialize((T)this);

            _states[typeof(State)] = state;
            return state;
        }
    }
}