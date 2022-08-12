using System;
using CodeMonkey.Utils;
using UnityEngine;

namespace Grid
{
    public class GridXZ<TGridObject>
    {
        private readonly TextMesh[,] _debugTextArray;
        private readonly TGridObject[,] _gridArray;
        private readonly int _height;
        private readonly Vector3 _originPosition;
        private readonly int _width;

        public int Height => _height;

        public int Width => _width;

        public GridXZ(int width, int height, float cellSize, Vector3 originPosition,
            Func<GridXZ<TGridObject>, int, int, TGridObject> createGridObject)
        {
            _width = width;
            _height = height;
            CellSize = cellSize;
            _originPosition = originPosition;

            _gridArray = new TGridObject[width, height];
            _debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
            {
                _gridArray[x, z] = createGridObject(this, x, z);
                _debugTextArray[x, z] = UtilsClass.CreateWorldText("", null,
                    GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * .5f, 30, Color.white,
                    TextAnchor.MiddleCenter);
                _debugTextArray[x, z].transform.Rotate(Vector3.right, 90);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);

                OnGridValueChanged += (object sender, OnGridXZValueChangedEventArgs eventArgs) =>
                {
                    _debugTextArray[eventArgs.X, eventArgs.Z].text = _gridArray[eventArgs.X, eventArgs.Z]?.ToString();
                };
            }

            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }

        public float CellSize { get; }

        public event EventHandler<OnGridXZValueChangedEventArgs> OnGridValueChanged;

        public Vector3 GetWorldPosition(int x, int z)
        {
            return new Vector3(x, 0, z) * CellSize + _originPosition;
        }

        public void GetXZ(Vector3 worldPosition, out int x, out int z)
        {
            x = Mathf.FloorToInt((worldPosition - _originPosition).x / CellSize);
            z = Mathf.FloorToInt((worldPosition - _originPosition).z / CellSize);
        }

        public void SetGridObject(int x, int z, TGridObject value)
        {
            if (x >= 0 && x < _width && z >= 0 && z < _height)
            {
                _gridArray[x, z] = value;
                TriggerGridObjectChanged(x, z);
            }
        }

        public void TriggerGridObjectChanged(int x, int z)
        {
            OnGridValueChanged?.Invoke(this, new OnGridXZValueChangedEventArgs { X = x, Z = z });
        }

        public void SetGridObject(Vector3 worldPosition, TGridObject value)
        {
            int x = Mathf.FloorToInt(worldPosition.x / CellSize);
            int z = Mathf.FloorToInt(worldPosition.z / CellSize);
            SetGridObject(x, z, value);
        }

        public TGridObject GetGridObject(int x, int z)
        {
            if (x >= 0 && x < _width && z >= 0 && z < _height) return _gridArray[x, z];
            return default;
        }

        public TGridObject GetGridObject(Vector3 worldPosition)
        {
            int x, z;
            GetXZ(worldPosition, out x, out z);
            return GetGridObject(x, z);
        }

        public class OnGridXZValueChangedEventArgs : EventArgs
        {
            public int X;
            public int Z;
        }
    }
}