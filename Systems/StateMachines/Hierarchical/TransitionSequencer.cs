namespace Darkan.Systems.StateMachine.Hierarchical
{
    using System.Collections.Generic;
    using UnityEngine;

    public class TransitionSequencer
    {
        public readonly StateMachine StateMachine;
        public TransitionSequencer(StateMachine stateMachine) => StateMachine = stateMachine;

        public static State GetLowestCommonAncestor(State a, State b)
        {
            HashSet<State> aParents = new(a.PathToRoot());

            foreach (State state in b.PathToRoot())
                if (aParents.Contains(state))
                    return state;

            return null;
        }

        public void RequestTransition(State from, State to)
        {
            StateMachine.ChangeState(from, to);
        }
    }
}