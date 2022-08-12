using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GOAP
{
    public class GInventory
    {
        public readonly List<GameObject> _items = new();

        public void AddItem(GameObject item)
        {
            _items.Add(item);
        }

        public void RemoveItem(GameObject item)
        {
            _items.Remove(item);
        }

        public GameObject FindItemWithTag(string tag)
        {
            return _items.FirstOrDefault(item => item.CompareTag(tag));
        }
    }
}