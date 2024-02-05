using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Darkan.StateMachine.Scriptable
{
    public abstract class StatesData<TEnum, TMachine> : SerializedScriptableObject where TEnum : Enum where TMachine : StateMachine<TEnum, TMachine>
    {
        [SerializeField]
        protected Dictionary<TEnum, BaseState<TEnum, TMachine>> _statesDictionary = new();

        public Dictionary<TEnum, BaseState<TEnum, TMachine>> StatesDictionary => _statesDictionary;
    }
}
