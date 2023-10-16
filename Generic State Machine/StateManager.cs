namespace Darkan.StateMachine
{
    using Sirenix.OdinInspector;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public abstract class StateManager<TEnum, TManager> : SerializedMonoBehaviour where TEnum : Enum where TManager : StateManager<TEnum, TManager>
    {
        public static event Action<TEnum> OnGameStateChanged;

        [SerializeField] protected Dictionary<TEnum, BaseState<TEnum, TManager>> States = new();

        protected BaseState<TEnum, TManager> ActiveState;

        void Start()
        {
            if (ActiveState == null)
                ActiveState = States.First().Value;

            ActiveState.Enter();
        }

        void Update()
        {
            ActiveState.Update();
        }

        public void TransitionToState(TEnum nextState)
        {
            ActiveState.Exit();

            ActiveState = States[nextState];

            ActiveState.Enter();

            OnGameStateChanged?.Invoke(nextState);
        }
    }
}