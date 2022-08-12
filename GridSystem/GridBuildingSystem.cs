using System;
using System.Collections.Generic;
using CodeMonkey.Utils;
using PlacedObjects;
using UnityEngine;

namespace Grid
{
    public class GridBuildingSystem : MonoBehaviour
    {
        [SerializeField] private List<PlacedObjectTypeSo> placedObjectTypeList;
        private PlacedObjectTypeSo.Dir _dir = PlacedObjectTypeSo.Dir.Down;
        public static GridBuildingSystem Instance { get; private set; }

        private GridXZ<GridObject> _grid;

        public GridXZ<GridObject> Grid => _grid;

        private PlacedObjectTypeSo _placedObjectType;

        public event EventHandler OnSelectedChanged;

        public PlacedObjectTypeSo PlacedObjectType
        {
            get => _placedObjectType;
            set
            {
                _placedObjectType = value;
                OnSelectedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void Awake()
        {
            Instance = this;
            int height = 19;
            int width = 19;
            float cellSize = 5f;

            _grid = new GridXZ<GridObject>(width, height, cellSize, Vector3.zero,
                (grid, x, z) => new GridObject(grid, x, z));

            _placedObjectType = placedObjectTypeList[0];
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _grid.GetXZ(Mouse3D.GetMouseWorldPosition(), out int x, out int z);
                GridObject gridObject = _grid.GetGridObject(x, z);
                List<Vector2Int> gridPositionList =
                    _placedObjectType.GetGridPositionList(new Vector2Int(x, z), _dir);
                bool canBuild = true;
                foreach (Vector2Int gridPosition in gridPositionList)
                    if (!_grid.GetGridObject(gridPosition.x, gridPosition.y).Canbuild())
                    {
                        canBuild = false;
                        break;
                    }

                if (canBuild)
                {
                    Vector2Int rotationOffset = _placedObjectType.GetRotationOffset(_dir);
                    Vector3 placedObjectWorldPosition = _grid.GetWorldPosition(x, z) +
                                                        new Vector3(rotationOffset.x, 0, rotationOffset.y) * _grid.CellSize;

                    PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), _dir,
                        _placedObjectType);

                    foreach (Vector2Int gridPosition in gridPositionList)
                        _grid.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                GridObject gridObject = _grid.GetGridObject(Mouse3D.GetMouseWorldPosition());
                PlacedObject placedObject = gridObject.PlacedObject;

                if (placedObject != null)
                {
                    placedObject.DestroySelf();
                    foreach (Vector2Int gridPosition in placedObject.GetGridPositionList())
                    {
                        _grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                _dir = PlacedObjectTypeSo.GetNextDir(_dir);
                UtilsClass.CreateWorldTextPopup(_dir.ToString(), Mouse3D.GetMouseWorldPosition());
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) PlacedObjectType = placedObjectTypeList[0];
            if (Input.GetKeyDown(KeyCode.Alpha2)) PlacedObjectType = placedObjectTypeList[1];
            if (Input.GetKeyDown(KeyCode.Alpha3)) PlacedObjectType = placedObjectTypeList[2];
            if (Input.GetKeyDown(KeyCode.Alpha4)) PlacedObjectType = placedObjectTypeList[3];
            if (Input.GetKeyDown(KeyCode.Alpha5)) PlacedObjectType = placedObjectTypeList[4];
            if (Input.GetKeyDown(KeyCode.Alpha6)) PlacedObjectType = placedObjectTypeList[5];
        }
    
        public Vector3 GetMouseWorldSnappedPosition() {
            Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
            _grid.GetXZ(mousePosition, out int x, out int z);

            if (_placedObjectType != null) {
                Vector2Int rotationOffset = _placedObjectType.GetRotationOffset(_dir);
                Vector3 placedObjectWorldPosition = _grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * _grid.CellSize;
                return placedObjectWorldPosition;
            } else {
                return mousePosition;
            }
        }
    
        public Quaternion GetPlacedObjectRotation() {
            if (_placedObjectType != null) {
                return Quaternion.Euler(0, _placedObjectType.GetRotationAngle(_dir), 0);
            } else {
                return Quaternion.identity;
            }
        }

        public class GridObject
        {
            private readonly GridXZ<GridObject> _grid;
            private readonly int _x;
            private readonly int _z;

            public GridObject(GridXZ<GridObject> grid, int x, int z)
            {
                _grid = grid;
                _x = x;
                _z = z;
            }

            public PlacedObject PlacedObject { get; private set; }

            public bool Canbuild()
            {
                return PlacedObject == null;
            }

            public void SetPlacedObject(PlacedObject placedObject)
            {
                PlacedObject = placedObject;
                _grid.TriggerGridObjectChanged(_x, _z);
            }

            public void ClearPlacedObject()
            {
                PlacedObject = null;
                _grid.TriggerGridObjectChanged(_x, _z);
            }

            public override string ToString()
            {
                return _x + " " + _z;
            }
        }
    }
}