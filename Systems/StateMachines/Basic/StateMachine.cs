namespace Darkan.Systems.StateMachine.Basic
{
    using System;

    public class StateMachine<T> : IDisposable where T : State<T>
    {
        public event Action<T> OnStateChanged;
        public T CurrentState { get; private set; }

        public void ChangeState(T to)
        {
            if (to == null) return;

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
        protected StateMachine<T> _stateMachine;

        public State(StateMachine<T> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public abstract void Enter();
        public abstract void Exit();
        public virtual void Dispose() { }
    }
}