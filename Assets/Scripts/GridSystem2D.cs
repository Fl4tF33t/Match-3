using System;
using TMPro;
using UnityEngine;

public class GridSystem2D<T> {
    public enum GridLayout {
        Vertical,
        Horizontal
    }
    
    private readonly int width;
    private readonly int height;
    private readonly float cellSize;
    private readonly Vector3 origin;
    private readonly T[,] grid;
    private readonly ICoordinateConverter iCoordinateConverter;

    public event Action<int, int, T> OnGridChanged;

    private GridSystem2D(int width, int height, float cellSize, Vector3 origin, ICoordinateConverter iCoordinateConverter, bool debug) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;
        this.iCoordinateConverter = iCoordinateConverter ?? new VerticalConverter();
        grid = new T[width, height];

        if (debug) DrawDebugLines();
    }

    public static GridSystem2D<T> CreateGrid(GridLayout layout, int width, int height, float cellSize, Vector3 origin, bool debug = false) {
        return layout == GridLayout.Vertical ? 
            new GridSystem2D<T>(width, height, cellSize, origin, new VerticalConverter(), debug) : 
            new GridSystem2D<T>(width, height, cellSize, origin, new HorizontalConverter(), debug);
    }
    public static GridSystem2D<T> VerticalGrid (int width, int height, float cellSize, Vector3 origin, bool debug = false) {
        return new GridSystem2D<T>(width, height, cellSize, origin, new VerticalConverter(), debug);
    }
    public static GridSystem2D<T> HorizontalGrid (int width, int height, float cellSize, Vector3 origin, bool debug = false) {
        return new GridSystem2D<T>(width, height, cellSize, origin, new HorizontalConverter(), debug);
    }
    private void DrawDebugLines() {
        const float DURATION = 100f;
        GameObject parent = new GameObject("Debugging");

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                CreateWorldText(parent, x + "," + y, GetWorldPositionCenter(x, y), iCoordinateConverter.Forward);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, DURATION);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, DURATION);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, DURATION);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, DURATION);
    }

    public T GetValue(Vector3 worldPosition) {
        Vector2Int gridPosition = iCoordinateConverter.WorldToGrid(worldPosition, cellSize, origin);
        return GetValue(gridPosition.x, gridPosition.y);    
    }
    public T GetValue (int x, int y){
        return IsValid(x, y) ? grid[x, y] : default(T);
    }
    public void SetValue (Vector3 worldPosition, T value){
        Vector2Int gridPosition = iCoordinateConverter.WorldToGrid(worldPosition, cellSize, origin);
        SetValue(gridPosition.x, gridPosition.y, value);
    }
    public void SetValue (int x, int y, T value) {
        if (!IsValid(x, y)) return;
        grid[x, y] = value;
        OnGridChanged?.Invoke(x, y, value);
    }
    private Vector3 GetWorldPosition (int x, int y) => iCoordinateConverter.GridToWorld(x, y, cellSize, origin);
    public bool IsValid(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;
    public Vector3 GetWorldPositionCenter (int x, int y) => iCoordinateConverter.GetToWorldCenter(x, y, cellSize, origin);
    public Vector2Int GetXY (Vector3 worldPositon) => iCoordinateConverter.WorldToGrid(worldPositon, cellSize, origin);

    private void CreateWorldText(GameObject parent, string text, Vector3 position, Vector3 direction, int fontSize = 2, Color color = default, TextAlignmentOptions textAlignmentOptions= TextAlignmentOptions.Center, int sortingOrder = 0) {
        GameObject gameObject = new GameObject ("DebugText_" + text, typeof(TextMeshPro));
        gameObject.transform.SetParent(parent.transform);
        gameObject.transform.position = position;
        gameObject.transform.forward = direction;

        TextMeshPro textMesh = gameObject.GetComponent<TextMeshPro>();
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.alpha = 1.0f;
        textMesh.alignment = textAlignmentOptions;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
    }
    
    // Coordinate Converter Interface and classes
    private interface ICoordinateConverter {
        public Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin);
        public Vector3 GetToWorldCenter(int x, int y, float cellSize, Vector3 origin);
        public Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin);
        public Vector3 Forward {get; }
    }
    private class VerticalConverter : ICoordinateConverter {
        public Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin) {
            Vector3 gridPosition = (worldPosition - origin) / cellSize;
            int x = Mathf.FloorToInt(gridPosition.x);
            int y = Mathf.FloorToInt(gridPosition.y);
            return new Vector2Int(x, y);
        }
        public Vector3 GetToWorldCenter(int x, int y, float cellSize, Vector3 origin) => origin + new Vector3(x * cellSize + cellSize * 0.5f, y * cellSize + cellSize * 0.5f, 0);
        public Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin) => origin + new Vector3(x, y, 0) * cellSize;
        public Vector3 Forward => Vector3.forward;
    }
    private class HorizontalConverter : ICoordinateConverter {
        public Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin) {
            Vector3 gridPosition = (worldPosition - origin) / cellSize;
            int x = Mathf.FloorToInt(gridPosition.x);
            int y = Mathf.FloorToInt(gridPosition.z);
            return new Vector2Int(x, y);
        }
        public Vector3 GetToWorldCenter(int x, int y, float cellSize, Vector3 origin) => origin + new Vector3(x * cellSize + cellSize * 0.5f, 0, y * cellSize + cellSize * 0.5f);
        public Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin) => origin + new Vector3(x, 0, y) * cellSize;
        public Vector3 Forward => -Vector3.up;
    }
}