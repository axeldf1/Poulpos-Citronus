using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP
{
    public class SubGoal
    {
        public bool remove;
        public Dictionary<string, int> sgoals;

        public SubGoal(string subGoal, int value, bool remove)
        {
            sgoals = new Dictionary<string, int>();
            sgoals.Add(subGoal, value);
            this.remove = remove;
        }
    }

    public class GAgent : MonoBehaviour
    {
        public List<GAction> actions = new();
        public GAction currentAction;
        private Queue<GAction> _actionQueue;
        private SubGoal _currentGoal;

        private bool _invoked;

        private GPlanner _planner;
        public Dictionary<SubGoal, int> goals = new();
        public GInventory Inventory = new();
        public WorldStates beliefs = new();
        public Dictionary<ItemSo, int> resource = new();

        public void Start()
        {
            GAction[] acts = GetComponents<GAction>();
            foreach (GAction act in acts) actions.Add(act);
        }

        private void LateUpdate()
        {
            if (currentAction != null && currentAction.running)
            {
                float distanceToTarget = Vector3.Distance(transform.position, currentAction.target.transform.position);
                if (currentAction.agent.hasPath && distanceToTarget < 1f)//currentAction.agent.remainingDistance < 1f)
                    if (!_invoked)
                    {
                        Invoke("CompleteAction", currentAction.duration);
                        _invoked = true;
                    }

                return;
            }

            if (_planner == null || _actionQueue == null)
            {
                _planner = new GPlanner();
                IOrderedEnumerable<KeyValuePair<SubGoal, int>> sortedGoals =
                    from entry in goals orderby entry.Value descending select entry;

                foreach (KeyValuePair<SubGoal, int> goal in sortedGoals)
                {
                    _actionQueue = _planner.Plan(actions, goal.Key.sgoals, beliefs);
                    if (_actionQueue != null)
                    {
                        _currentGoal = goal.Key;
                        break;
                    }
                }
            }

            if (_actionQueue != null && _actionQueue.Count == 0)
            {
                if (_currentGoal.remove)
                    goals.Remove(_currentGoal);

                _planner = null;
            }

            if (_actionQueue != null && _actionQueue.Count > 0)
            {
                currentAction = _actionQueue.Dequeue();
                if (currentAction.PrePerform())
                {
                    if (currentAction.target == null && currentAction.targetTag != "")
                        currentAction.target = GameObject.FindWithTag(currentAction.targetTag);

                    if (currentAction.target != null)
                    {
                        currentAction.running = true;
                        currentAction.agent.SetDestination(currentAction.target.transform.position);
                    }
                }
                else
                {
                    _actionQueue = null;
                }
            }
        }

        private void CompleteAction()
        {
            currentAction.running = false;
            currentAction.PostPerform();
            _invoked = false;
        }
    }
}