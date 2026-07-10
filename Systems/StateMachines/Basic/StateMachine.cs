namespace Darkan.Systems.StateMachine.Basic
{
    using System;
    using System.Collections.Generic;

    public abstract class StateMachine : IDisposable
    {
        public event Action<State<StateMachine>> OnStateChanged;
        public State<StateMachine> CurrentState { get; private set; }

        readonly Dictionary<Type, State<StateMachine>> _states = new();

        public void ChangeState(State<StateMachine> to)
        {
            if (to == null) return;

            CurrentState?.Exit();

            CurrentState = to;
            OnStateChanged?.Invoke(CurrentState);

            CurrentState.Enter();
        }

        public void ChangeState<State>() where State : State<StateMachine>
        {
            CurrentState?.Exit();

            if (!_states.TryGetValue(typeof(State), out State<StateMachine> to))
            {
                to = Activator.CreateInstance<State>();
                _states[typeof(State)] = to;
                to.Initialize(this);
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

    public abstract class State<T> : IDisposable where T : StateMachine
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