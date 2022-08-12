using System;
using System.Collections.Generic;

namespace GOAP
{
    [Serializable]
    public class WorldState
    {
        public string key;
        public int value;
    }

    public class WorldStates
    {
        public Dictionary<string, int> States;

        public WorldStates()
        {
            States = new Dictionary<string, int>();
        }

        public bool HasState(string key)
        {
            return States.ContainsKey(key);
        }

        public void AddState(string key, int value)
        {
            States.Add(key, value);
        }

        public void ModifyState(string key, int value)
        {
            if (HasState(key))
            {
                States[key] += value;
                if (States[key] <= 0) RemoveState(key);
            }
            else
            {
                States.Add(key, value);
            }
        }

        public void RemoveState(string key)
        {
            if (HasState(key)) States.Remove(key);
        }

        public void SetState(string key, int value)
        {
            if (HasState(key))
                States[key] = value;
            else
                States.Add(key, value);
        }

        public Dictionary<string, int> GetStates()
        {
            return States;
        }
    }
}