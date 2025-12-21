namespace Darkan.Systems.StateMachine.Hierarchical
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class StateMachine
    {
        public readonly State Root;
        public readonly TransitionSequencer TransitionSequencer;

        bool _isRunning;

        public StateMachine(State root)
        {
            Root = root;
            TransitionSequencer = new TransitionSequencer(this);
        }

        public void ChangeState(State from, State to)
        {
            if (from == null || to == null) return;

            State lca = TransitionSequencer.GetLowestCommonAncestor(from, to);

            // Exit current branch up to LCA child
            for (State s = from; s != lca; s = s.Parent)
                s.Exit();

            // kann ich nicht exit bei lca child callen und wenn lca null ist beim root?

            // Enter target branch from LCA down to target
            Stack<State> stack = new();

            for (State s = to; s != lca; s = s.Parent)
                stack.Push(s);

            while (stack.Count > 0)
                stack.Pop().Enter();
        }

        public string GetFullStateHierarchy()
        {
            return string.Join(">", Root.GetLeaf().PathToRoot().Reverse().Select(n => n.GetType().Name));
        }

        public void Start()
        {
            if (_isRunning) return;
            _isRunning = true;

            Root.Enter();
        }

        public void Tick()
        {
            if (!_isRunning) Start();

            InternalTick();
        }

        internal void InternalTick()
        {
            Root.Update();
        }
    }
}