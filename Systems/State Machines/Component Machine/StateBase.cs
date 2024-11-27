namespace Darkan.StateMachine.Component
{
    using UnityEngine;

    public abstract class StateBase<TEnum, TMachine> : MonoBehaviour where TEnum : System.Enum where TMachine : StateMachine<TEnum, TMachine>
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

        public abstract void EnterState();

        public abstract void ExitState();
    }
}
