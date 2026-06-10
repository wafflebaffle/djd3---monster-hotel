using System;
using LibGameAI.FSMs;

namespace LibGameAI.FSMs
{
    /// <summary>
    /// A finite state machine.
    /// </summary>
    public class StateMachine
    {
        private State currentState;

        /// Create a new FSM.
        /// </summary>
        /// <param name="initialState">Initial state.</param>
        public StateMachine(State initialState)
        {
            currentState = initialState;
            currentState.EntryActions?.Invoke();
        }

        public void ForceState(State newState)
        {
            currentState = newState;
            currentState.EntryActions?.Invoke();
        }

        /// <summary>
        /// Update the FSM and return the actions to perform.
        /// </summary>
        /// <returns>Actions to perform.</returns>
        public Action Update()
        {
            // Assume no transition is triggered
            Transition triggeredTransition = null;

            // Check through each transition and store the first one that
            // triggers
            foreach (Transition transition in currentState.Transitions)
            {
                if (transition.IsTriggered())
                {
                    triggeredTransition = transition;
                    break;
                }
            }

                // Check if we have a transition to fire
            if (triggeredTransition != null)
            {
                // Actions to perform when transitioning between states
                Action actions = null;

                // Find the target state
                State targetState = triggeredTransition.TargetState;

                // Add the exit action of the old state, the transition action
                // and the entry for the new state
                actions += currentState.ExitActions;
                actions += triggeredTransition.Actions;
                actions += targetState.EntryActions;

                // Complete the transition and return the action list
                currentState = targetState;
                return actions;
            }

            // If no transition was triggered, return the actions for the
            // current state
            UnityEngine.Debug.Log("Current State is: " + currentState.Name);
            return currentState.StateActions;
        }
    }
}