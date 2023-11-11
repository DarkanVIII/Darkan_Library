namespace Darkan.StateMachine.Component
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    public abstract class BaseState<TEnum, TMachine> : SerializedMonoBehaviour where TEnum : System.Enum where TMachine : StateMachine<TEnum, TMachine>
    {
#if !RELEASE
        protected TMachine StateMachine { get; private set; }
#else
        protected TManager StateMachine;
#endif

        public void Init(TMachine stateMachine)
        {
            StateMachine = stateMachine;
        }

        protected virtual void Awake()
        {
            enabled = false;
        }

        public abstract void EnterState();
        /// <summary>
        /// Called OnApplicationQuit too to unsubscribe from possible events
        /// </summary>
        public abstract void ExitState();
    }
}
