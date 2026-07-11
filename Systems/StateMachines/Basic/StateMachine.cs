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

        public void Enter()
        {
            OnEnter();
        }

        public void Exit()
        {
            CancelAndDisposeCts();
            OnExit();
        }

        public void Dispose()
        {
            CancelAndDisposeCts();
            OnDispose();
        }

        protected abstract void OnEnter();
        protected abstract void OnExit();
        protected virtual void OnDispose() { }

        void CancelAndDisposeCts()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
        }
    }
}