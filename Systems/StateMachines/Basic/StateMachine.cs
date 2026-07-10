namespace Darkan.Systems.StateMachine.Basic
{
    using System;
    using System.Collections.Generic;

    public abstract class StateMachine<T> : IDisposable where T : StateMachine<T>
    {
        public event Action<State<T>> OnStateChanged;
        public State<T> CurrentState { get; private set; }

        readonly Dictionary<Type, State<T>> _states = new();

        public void ChangeState(State<T> to)
        {
            if (to == null) return;

            CurrentState?.Exit();

            CurrentState = to;
            OnStateChanged?.Invoke(CurrentState);

            CurrentState.Enter();
        }

        public void ChangeState<State>() where State : State<T>
        {
            CurrentState?.Exit();

            if (!_states.TryGetValue(typeof(State), out State<T> to))
            {
                to = Activator.CreateInstance<State>();
                _states[typeof(State)] = to;
                to.Initialize((T)this);
            }

            CurrentState = to;
            OnStateChanged?.Invoke(CurrentState);

            CurrentState.Enter();
        }

        public void Dispose()
        {
            CurrentState?.Dispose();
            OnStateChanged = null;
        }
    }

    public abstract class State<T> : IDisposable where T : StateMachine<T>
    {
        protected T _stateMachine { get; private set; }

        public void Initialize(T stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public abstract void Enter();
        public abstract void Exit();
        public virtual void Dispose() { }
    }
}