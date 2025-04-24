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

        /// <summary>
        /// Transitions to the specified state. If the state is not already created, it will be created and added to the state machine.
        /// </summary>
        public void TransitionToState<State>() where State : MonoBehaviour, IState<T>
        {
            if (_activeState != null)
            {
                _activeState.enabled = false;
                ((IState<T>)_activeState).Exit();
            }

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