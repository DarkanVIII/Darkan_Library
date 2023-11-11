namespace Darkan.StateMachine.Scriptable
{
    using Sirenix.OdinInspector;
    using System;
    using UnityEngine;

    public abstract class BaseState<TEnum, TMachine> : SerializedScriptableObject where TEnum : Enum where TMachine : StateMachine<TEnum, TMachine>
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

        public abstract void AwakeState();
        public abstract void EnterState();
        /// <summary>
        /// Will be called OnApplicationQuit too to make sure unsibscribing happens
        /// </summary>
        public abstract void ExitState();
        public virtual void UpdateState() { }
        public virtual void FixedUpdateState() { }
        public virtual void OnTriggerEnterState(Collider collider) { }
        public virtual void OnTriggerExitState(Collider collider) { }
        public virtual void OnTriggerStayState(Collider collider) { }
        public virtual void OnTriggerEnter2DState(Collider2D collider) { }
        public virtual void OnTriggerExit2DState(Collider2D collider) { }
        public virtual void OnTriggerStay2DState(Collider2D collider) { }
        public virtual void OnCollisionEnterState(Collision collision) { }
        public virtual void OnCollisionStayState(Collision collision) { }
        public virtual void OnCollisionExitState(Collision collision) { }
        public virtual void OnCollisionEnter2DState(Collision2D collision) { }
        public virtual void OnCollisionStay2DState(Collision2D collision) { }
        public virtual void OnCollisionExit2DState(Collision2D collision) { }
    }
}
