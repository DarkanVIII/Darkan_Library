namespace Darkan.StateMachine.Component
{
    using Sirenix.OdinInspector;
    using System;
    using System.Collections.Generic;
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
    public abstract class StateMachine<TEnum, TMachine> : SerializedMonoBehaviour where TEnum : Enum where TMachine : StateMachine<TEnum, TMachine>
    {
        public event Action<TEnum> OnStateChanged;

        [SerializeField]
        protected Dictionary<TEnum, BaseState<TEnum, TMachine>> StatesDictionary = new();

        [ShowInInspector]
        [ReadOnly]
        protected BaseState<TEnum, TMachine> ActiveState;

        protected virtual void Awake()
        {
            foreach (var keyValuePair in StatesDictionary)
            {
                keyValuePair.Value.Init((TMachine)this);
                keyValuePair.Value.enabled = false;
            }
        }

        protected virtual void Start()
        {
            ActiveState = StatesDictionary[SetEntryState()];
            ActiveState.EnterState();
            ActiveState.enabled = true;
        }

        public abstract TEnum SetEntryState();

        public void TransitionToState(TEnum nextState)
        {
            ActiveState.ExitState();
            ActiveState.enabled = false;

            ActiveState = StatesDictionary[nextState];

            ActiveState.EnterState();
            ActiveState.enabled = true;

            OnStateChanged?.Invoke(nextState);
        }

        void OnApplicationQuit()
        {
            ActiveState.enabled = false;
        }
    }
}