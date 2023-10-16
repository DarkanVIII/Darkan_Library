namespace Darkan.StateMachine
{
    using System;
    using UnityEngine;

    public abstract class BaseState<TEnum, TManager> : ScriptableObject where TEnum : Enum where TManager : StateManager<TEnum, TManager>
    {
        public BaseState(TEnum key, TManager stateManager)
        {
            Key = key;
            StateManager = stateManager;
        }

#if RELEASE
        public TEnum Key;
        public TManager StateManager;
#else
        public TEnum Key { get; private set; }
        public TManager StateManager { get; private set; }
#endif
        public abstract void Enter();
        public abstract void Exit();
        public abstract void Update();
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
