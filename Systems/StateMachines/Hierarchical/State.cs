namespace Darkan.Systems.StateMachine.Hierarchical
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class State
    {
        public readonly State Parent;
        public readonly StateMachine StateMachine;
        public State Child;

        protected State(StateMachine stateMachine, State parent = null)
        {
            StateMachine = stateMachine;
            Parent = parent;
        }

        /// <returns> The deepest descendant state of this state </returns>
        public State GetLeaf()
        {
            State leaf = this;

            while (leaf.Child != null)
                leaf = leaf.Child;

            return leaf;
        }

        /// <summary>
        ///     yields in turn (one after another): current → parent → ... → root
        /// </summary>
        public IEnumerable<State> PathToRoot()
        {
            for (State s = this; s != null; s = s.Parent)
                yield return s;
        }

        /// <summary>
        ///     Initial child to enter when this state starts. null = this is the leaf
        /// </summary>
        protected virtual State GetInitialState() => null;

        /// <summary>
        ///     Target state to switch to this frame. null = stay in current state
        /// </summary>
        protected virtual State GetTransition() => null;

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual void OnUpdate() { }

        internal void Enter()
        {
            if (Parent != null) Parent.Child = this;
            OnEnter();
            State initialState = GetInitialState();
            initialState?.Enter();
        }

        internal void Exit()
        {
            Child?.Exit();
            Child = null;
            OnExit();
        }

        internal void Update()
        {
            State t = GetTransition();

            if (t != null)
            {
                StateMachine.TransitionSequencer.RequestTransition(this, t);
                return;
            }

            Child?.Update();
            OnUpdate();
        }
    }
}