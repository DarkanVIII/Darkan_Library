namespace Darkan.Systems.StateMachine.Basic
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

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

            CurrentState.OnEnter();
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
        protected CancellationToken _cancellationToken
        {
            get
            {
                _cts ??= new CancellationTokenSource();
                return _cts.Token;
            }
        }

        CancellationTokenSource _cts;

        public void Initialize(TMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public abstract void OnEnter();

        public void Exit()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }

            OnExit();
        }

        public void Dispose()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }

            OnDispose();
        }

        protected abstract void OnExit();
        protected abstract void OnDispose();
    }
}