namespace Darkan.Systems.StateMachine.Basic
{
    using System;
    using System.Collections.Generic;

    public abstract class StateMachine<TMachine> : IDisposable where TMachine : StateMachine<TMachine>
    {
        public event Action<State<TMachine>> OnStateChanged;
        public State<TMachine> CurrentState { get; private set; }

        readonly Dictionary<Type, State<TMachine>> _states = new();

        public void ChangeState<TState>() where TState : State<TMachine>
        {
            CurrentState?.Exit();

            if (!_states.TryGetValue(typeof(TState), out State<TMachine> to))
            {
                to = Activator.CreateInstance<TState>();
                _states[typeof(TState)] = to;
                to.Initialize((TMachine)this);
            }

            CurrentState = to;
            OnStateChanged?.Invoke(CurrentState);

            CurrentState.Enter();
        }

        public void Dispose()
        {
            foreach (var state in _states.Values)
                state?.Dispose();

            OnStateChanged = null;
        }
    }

    public abstract class State<TMachine> : IDisposable where TMachine : StateMachine<TMachine>
    {
        protected TMachine _stateMachine { get; private set; }

        public void Initialize(TMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public abstract void Enter();
        public abstract void Exit();
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void Dispose() { }
    }
}