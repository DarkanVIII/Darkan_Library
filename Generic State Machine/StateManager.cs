namespace Darkan.StateMachine
{
    using Sirenix.OdinInspector;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// To use: <br/>
    /// 1. Create and derive a State Manager from this and create a State Enum in it <br/>
    /// 2. Create and derive States from <see cref="BaseState{TEnum, TManager}"/> <br/>
    /// 3. Add AssetMenu Attribute to each Base State and create a Scriptable Objects of each State
    /// 4. Add State Objects to Dictionary of State Manager
    /// 5. Set Active State in Awake -> Starting State (Will pick randomly if not set to avoid Errors) <br/>
    /// To use Unity Messages like Update and OnTriggerEnter -> For Example add in State Manager : Update { ActiveState.Update }
    /// </summary>
    public abstract class StateManager<TEnum, TManager> : SerializedMonoBehaviour where TEnum : Enum where TManager : StateManager<TEnum, TManager>
    {
        public static event Action<TEnum> OnGameStateChanged;

        [SerializeField] protected Dictionary<TEnum, BaseState<TEnum, TManager>> States = new();

        protected BaseState<TEnum, TManager> ActiveState;

        void Awake()
        {
            foreach (var state in States)
            {
                state.Value.Init((TManager)this);
                state.Value.OnAwake();
            }
        }

        void Start()
        {
            if (ActiveState == null)
                ActiveState = States.First().Value;

            ActiveState.Enter();
        }

        public void TransitionToState(TEnum nextState)
        {
            ActiveState.Exit();

            ActiveState = States[nextState];

            ActiveState.Enter();

            OnGameStateChanged?.Invoke(nextState);
        }

        void OnDestroy()
        {
            ActiveState.Unsubscribe();
        }
    }
}