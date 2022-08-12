using System.Collections.Generic;
using System.Linq;

namespace GOAP
{
    public class Node
    {
        public GAction action;
        public float cost;
        public Node parent;
        public Dictionary<string, int> state;

        public Node(Node parent, float cost, Dictionary<string, int> allStates, GAction action)
        {
            this.parent = parent;
            this.cost = cost;
            state = new Dictionary<string, int>(allStates);
            this.action = action;
        }

        public Node(Node parent, float cost, Dictionary<string, int> allStates, Dictionary<string, int> beliefStates,
            GAction action)
        {
            this.parent = parent;
            this.cost = cost;
            state = new Dictionary<string, int>(allStates);
            foreach (KeyValuePair<string, int> belief in beliefStates.Where(belief => !state.ContainsKey(belief.Key)))
                state.Add(belief.Key, belief.Value);
            this.action = action;
        }
    }

    public class GPlanner
    {
        public Queue<GAction> Plan(List<GAction> actions, Dictionary<string, int> goal, WorldStates beliefStates)
        {
            List<GAction> usableActions = new();
            foreach (GAction action in actions)
                if (action.IsAchievable())
                    usableActions.Add(action);

            List<Node> leaves = new();
            Node start = new(null, 0, GWorld.Instance.GetWorld().GetStates(), beliefStates.States, null);

            bool success = BuildGraph(start, leaves, usableActions, goal);

            if (!success)
                // Debug.Log("No plan found");
                return null;

            Node cheapest = null;
            foreach (Node leaf in leaves)
                if (cheapest == null || leaf.cost < cheapest.cost)
                    cheapest = leaf;

            List<GAction> result = new();
            Node n = cheapest;
            while (n != null)
            {
                if (n.action != null)
                    result.Insert(0, n.action);
                n = n.parent;
            }

            Queue<GAction> queue = new();
            foreach (GAction action in result)
                queue.Enqueue(action);

            // Debug.Log("Plan is : ");
            // foreach (GAction action in queue) Debug.Log("Q: " + action.actionName);

            return queue;
        }

        private bool BuildGraph(Node parent, List<Node> leaves, List<GAction> usableActions,
            Dictionary<string, int> goal)
        {
            bool foundPlan = false;

            foreach (GAction action in usableActions)
                if (action.IsAchievableGiven(parent.state))
                {
                    Dictionary<string, int> currentState = new(parent.state);
                    foreach (KeyValuePair<string, int> effect in action.effects)
                        if (!currentState.ContainsKey(effect.Key))
                            currentState.Add(effect.Key, effect.Value);

                    Node node = new(parent, parent.cost + action.cost, currentState, action);
                    if (GoalAchieved(goal, currentState))
                    {
                        leaves.Add(node);
                        foundPlan = true;
                    }
                    else
                    {
                        List<GAction> subset = ActionSubset(usableActions, action);
                        bool found = BuildGraph(node, leaves, subset, goal);
                        if (found)
                            foundPlan = true;
                    }
                }

            return foundPlan;
        }

        private bool GoalAchieved(Dictionary<string, int> goal, Dictionary<string, int> state)
        {
            foreach (KeyValuePair<string, int> goalState in goal)
                if (!state.ContainsKey(goalState.Key))
                    return false;
            return true;
        }

        private List<GAction> ActionSubset(List<GAction> actions, GAction remove)
        {
            return actions.Where(action => !action.Equals(remove)).ToList();
        }
    }
}