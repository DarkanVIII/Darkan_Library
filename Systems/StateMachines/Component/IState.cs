namespace Darkan.Systems.StateMachine.Component
{
    using System;
    using UnityEngine;

    public interface IState<TMachine, TEnum> where TMachine : StateMachine<TEnum, TMachine> where TEnum : Enum
    {
        public TEnum StateType { get; }
        void Enter();
        void Exit();
        void Initialize(TMachine stateMachine);
    }
}