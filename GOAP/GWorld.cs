using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public sealed class GWorld
    {
        private static readonly WorldStates world;
        private static Queue<GameObject> _plantations, _silos;
        static GWorld()
        {
            world = new WorldStates();
            _plantations = new Queue<GameObject>();
            _silos = new Queue<GameObject>();
        }

        private GWorld()
        {
        }

        public static GWorld Instance { get; } = new();

        public WorldStates GetWorld()
        {
            return world;
        }
        public void AddPlantation(GameObject plantation)
        {
            _plantations.Enqueue(plantation);
        }
        public GameObject RemovePlantation()
        {
            return _plantations.Dequeue();
        }
        public void AddSilo(GameObject silo)
        {
            _silos.Enqueue(silo);
        }
        public void RemoveSilo()
        {
            _silos.Dequeue();
        }
        public GameObject GetSilo()
        {
            if(_silos.Count == 0) return null;
            
            return _silos.Peek();
        }
    }
}