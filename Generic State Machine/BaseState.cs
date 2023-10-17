namespace Darkan.StateMachine
{
    using Sirenix.OdinInspector;
    using System;
    using UnityEngine;

    public abstract class BaseState<TEnum, TManager> : SerializedScriptableObject where TEnum : Enum where TManager : StateManager<TEnum, TManager>
    {
#if !RELEASE
        public TManager StateManager { get; private set; }
#else
        public TManager StateManager;
#endif

        public void Init(TManager stateManager)
        {
            StateManager = stateManager;
        }

        public abstract void OnAwake();
        public abstract void Enter();
        public abstract void Exit();
        /// <summary>
        /// Unsubscribe from events and delegates here and call this in <see cref="Exit"/>.
        /// Will make sure the state unsubscribes if the state wasn't exited (game closed)
        /// </summary>
        public abstract void Unsubscribe();
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnTriggerEnter(Collider collider) { }
        public virtual void OnTriggerExit(Collider collider) { }
        public virtual void OnTriggerStay(Collider collider) { }
        public virtual void OnTriggerEnter2D(Collider2D collider) { }
        public virtual void OnTriggerExit2D(Collider2D collider) { }
        public virtual void OnTriggerStay2D(Collider2D collider) { }
        public virtual void OnCollisionEnter(Collision collision) { }
        public virtual void OnCollisionStay(Collision collision) { }
        public virtual void OnCollisionExit(Collision collision) { }
        public virtual void OnCollisionEnter2D(Collision2D collision) { }
        public virtual void OnCollisionStay2D(Collision2D collision) { }
        public virtual void OnCollisionExit2D(Collision2D collision) { }
    }
}
