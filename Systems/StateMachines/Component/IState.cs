using System;

namespace Darkan.Systems.StateMachine.Component.New
{
    public interface IState<TMachine, TEnum> where TMachine : StateMachine<TEnum, TMachine> where TEnum : Enum
    {
        public TEnum StateType { get; }
        void Initialize(TMachine stateMachine);
        void Start();
        void Enter();
        void Exit();
    }
}