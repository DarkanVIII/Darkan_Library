namespace Darkan.Systems.StateMachine.Component.New
{
    public interface IState<T> where T : StateMachine<T>
    {
        void Initialize(T stateMachine);
        void Enter();
        void Exit();
    }
}