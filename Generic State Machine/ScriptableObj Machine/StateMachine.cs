namespace Darkan.StateMachine.Scriptable
{
    using Sirenix.OdinInspector;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class StateMachine<TEnum, TMachine> : MonoBehaviour where TEnum : Enum where TMachine : StateMachine<TEnum, TMachine>
    {
        [Required]
        [SerializeField]
        StatesData<TEnum, TMachine> _statesData;

        public event Action<TEnum> OnStateChanged;

        protected Dictionary<TEnum, BaseState<TEnum, TMachine>> StatesDictionary = new();

        [ShowInInspector]
        [ReadOnly]
        protected BaseState<TEnum, TMachine> ActiveStateScriptable;

        protected virtual void Awake()
        {
            if (_statesData == null)
            {
                enabled = false;
                return;
            }

            StatesDictionary = _statesData.StatesDictionary;
        }

        protected virtual void Start()
        {
            ActiveStateScriptable = StatesDictionary[SetEntryState()];
            ActiveStateScriptable.EnterState();
        }

        public abstract TEnum SetEntryState();

        public void TransitionToState(TEnum nextState)
        {
            ActiveStateScriptable.ExitState();

            ActiveStateScriptable = StatesDictionary[nextState];

            OnStateChanged?.Invoke(nextState);

            ActiveStateScriptable.EnterState();
        }

        void OnApplicationQuit()
        {
            ActiveStateScriptable.ExitState();
        }
    }
}