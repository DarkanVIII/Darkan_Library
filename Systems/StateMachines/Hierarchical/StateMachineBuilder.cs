namespace Darkan.Systems.StateMachine.Hierarchical
{
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    public class StateMachineBuilder
    {
        readonly State _root;

        public StateMachineBuilder(State root) => _root = root;

        public StateMachine Build()
        {
            StateMachine stateMachine = new(_root);
            Wire(_root, stateMachine, new HashSet<State>());
            return stateMachine;
        }

        static void Wire(State state, StateMachine machine, HashSet<State> visited)
        {
            if (state == null) return;
            if (!visited.Add(state)) return; // state already wired

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;
            FieldInfo machineField = typeof(State).GetField("StateMachine", flags);

            if (machineField != null)
                machineField.SetValue(state, machine);

            foreach (FieldInfo field in state.GetType().GetFields(flags))
            {
                if (!typeof(State).IsAssignableFrom(field.FieldType)) continue; // only fields of type State
                if (field.Name == "Parent") continue;

                State child = (State)field.GetValue(state);
                if (child == null) continue;
                if (!ReferenceEquals(child.Parent, state)) continue;

                Wire(child, machine, visited);
            }
        }
    }
}