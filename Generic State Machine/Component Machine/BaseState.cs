namespace Darkan.StateMachine.Component
{
    using UnityEngine;

    public abstract class BaseState<TEnum, TMachine> : MonoBehaviour where TEnum : System.Enum where TMachine : StateMachine<TEnum, TMachine>
    {
        [SerializeField]
        TEnum _state;

        public TEnum State => _state;

#if !RELEASE
        protected TMachine StateMachine { get; private set; }
#else
        protected TManager StateMachine;
#endif

        public virtual void Init(TMachine stateMachine)
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
