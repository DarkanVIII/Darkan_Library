namespace Darkan.Systems.StateMachine.Basic
{
    using System;
    using System.Collections.Generic;

    public class StateMachine<T> : IDisposable where T : State<T>
    {
        public event Action<T> OnStateChanged;
        public T CurrentState { get; private set; }

        readonly Dictionary<Type, T> _states = new();

        public void ChangeState(T to)
        {
            if (to == null) return;

            CurrentState?.Exit();

            CurrentState = to;
            OnStateChanged?.Invoke(CurrentState);

            CurrentState.Enter();
        }

        public void ChangeState<State>() where State : T
        {
            if (!_states.TryGetValue(typeof(State), out T to))
            {
                to = Activator.CreateInstance<State>();
                _states[typeof(State)] = to;
                to.Initialize(this);
            }

            CurrentState?.Exit();

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

    public abstract class State<T> : IDisposable where T : State<T>
    {
        protected StateMachine<T> _stateMachine { get; private set; }

        public void Initialize(StateMachine<T> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public abstract void Enter();
        public abstract void Exit();
        public virtual void Dispose() { }
    }
}