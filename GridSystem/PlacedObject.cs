using System.Collections.Generic;
using UnityEngine;

namespace PlacedObjects
{
    public class PlacedObject : MonoBehaviour
    {
        private PlacedObjectTypeSo.Dir _dir;
        private Vector2Int _origin;
        private PlacedObjectTypeSo _placedObjectType;

        public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSo.Dir direction,
            PlacedObjectTypeSo placedObjectTypeSo)
        {
            Transform placedObjectTransform = Instantiate(
                placedObjectTypeSo.prefab,
                worldPosition,
                Quaternion.Euler(0, placedObjectTypeSo.GetRotationAngle(direction), 0));

            PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

            placedObject._placedObjectType = placedObjectTypeSo;
            placedObject._origin = origin;
            placedObject._dir = direction;

            return placedObject;
        }

        public List<Vector2Int> GetGridPositionList()
        {
            return _placedObjectType.GetGridPositionList(_origin, _dir);
        } 
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}