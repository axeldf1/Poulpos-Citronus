using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP
{
    public abstract class GAction : MonoBehaviour
    {
        public string actionName = "Action";
        public float cost = 1f;
        public GameObject target;
        public string targetTag;
        public float duration;
        public WorldState[] wsPreconditions;
        public WorldState[] wsAfterEffects;
        public NavMeshAgent agent;
        public GInventory Inventory;
        public Dictionary<ItemSo, int> resource;

        public bool running;

        public WorldStates agentBeliefs;
        public WorldStates beliefs;

        public Dictionary<string, int> effects;
        public Dictionary<string, int> preconditons;

        public GAction()
        {
            preconditons = new Dictionary<string, int>();
            effects = new Dictionary<string, int>();
        }

        public void Awake()
        {
            agent = gameObject.GetComponent<NavMeshAgent>();

            if (wsPreconditions != null)
                foreach (WorldState ws in wsPreconditions)
                    preconditons.Add(ws.key, ws.value);

            if (wsAfterEffects != null)
                foreach (WorldState ws in wsAfterEffects)
                    effects.Add(ws.key, ws.value);

            Inventory = GetComponent<GAgent>().Inventory;
            beliefs = GetComponent<GAgent>().beliefs;
            resource = GetComponent<GAgent>().resource;
        }

        public bool IsAchievable()
        {
            return true;
        }

        public bool IsAchievableGiven(Dictionary<string, int> conditions)
        {
            foreach (KeyValuePair<string, int> p in preconditons)
            {
                if (!conditions.ContainsKey(p.Key) || conditions[p.Key] < p.Value) return false;
            }

            return true;
        }
        
        public abstract bool PrePerform();
        public abstract bool PostPerform();
    }
}