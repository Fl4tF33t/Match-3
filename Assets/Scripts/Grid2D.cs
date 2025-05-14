using System;
using TMPro;
using UnityEngine;

public class GridSystem2D<T> {
    private int width;
    private int height;
    private float cellSize;
    private Vector3 origin;
    private T[,] grid;

    private CoordinateConverter coordinateConverter;

    public event Action<int, int, T> OnGridChanged;


    public GridSystem2D(int width, int height, float cellSize, Vector3 origin, CoordinateConverter coordinateConverter, bool debug) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;
        this.coordinateConverter = coordinateConverter ?? new VerticalConverter();

        grid = new T[width, height];

        if (debug) {
            DrawDebugLines();
        }
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
                CreateWorldText(parent, x + "," + y, GetWorldPositionCenter(x, y), coordinateConverter.Forward);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, DURATION);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, DURATION);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, DURATION);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, DURATION);
    }

    public T GetValue(Vector3 worldPosition) {
        Vector2Int gridPosition = coordinateConverter.WorldToGrid(worldPosition, cellSize, origin);
        return GetValue(gridPosition.x, gridPosition.y);    
    }
    public T GetValue (int x, int y){
        return IsValid(x, y) ? grid[x, y] : default(T);
    }
    public void SetValue (Vector3 worldPosition, T value){
        Vector2Int gridPosition = coordinateConverter.WorldToGrid(worldPosition, cellSize, origin);
        SetValue(gridPosition.x, gridPosition.y, value);
    }
    public void SetValue (int x, int y, T value) {
        if (IsValid(x, y)){
            grid[x, y] = value;
            OnGridChanged?.Invoke(x, y, value);
        }
    }
    private bool IsValid(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;

    public Vector3 GetWorldPositionCenter (int x, int y) => coordinateConverter.GetToWorldCenter(x, y, cellSize, origin);
    private Vector3 GetWorldPosition (int x, int y) => coordinateConverter.GridToWorld(x, y, cellSize, origin);
    public Vector2Int GetXY (Vector3 worldPositon) => coordinateConverter.WorldToGrid(worldPositon, cellSize, origin);

    private TextMeshPro CreateWorldText(GameObject parent, string text, Vector3 position, Vector3 direction, int fontSize = 2, Color color = default, TextAlignmentOptions textAlignmentOptions= TextAlignmentOptions.Center, int sortingOrder = 0) {
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
        
        return textMesh;
    }
    
    public abstract class CoordinateConverter {

        public abstract Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin);
        public abstract Vector3 GetToWorldCenter(int x, int y, float cellSize, Vector3 origin);

        public abstract Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin);
        public abstract Vector3 Forward {get; }
    }

    public class VerticalConverter : CoordinateConverter {

        public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin) {
            return origin + new Vector3(x, y, 0) * cellSize;
        }

        public override Vector3 GetToWorldCenter(int x, int y, float cellSize, Vector3 origin) {
            return origin + new Vector3(x * cellSize + cellSize * 0.5f, y * cellSize + cellSize * 0.5f, 0);
        }

        public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin) {
            Vector3 gridPostion = (worldPosition - origin) / cellSize;
            int x = Mathf.FloorToInt(gridPostion.x);
            int y = Mathf.FloorToInt(gridPostion.y);
            return new Vector2Int(x, y);
        }

        public override Vector3 Forward => Vector3.forward;
    }

    public class HorizontalConverter : CoordinateConverter {

        public override Vector3 GridToWorld(int x, int y, float cellSize, Vector3 origin) {
            return origin + new Vector3(x, 0, y) * cellSize;
        }

        public override Vector3 GetToWorldCenter(int x, int y, float cellSize, Vector3 origin) {
            return origin + new Vector3(x * cellSize + cellSize * 0.5f, 0, y * cellSize + cellSize * 0.5f);
        }

        public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSize, Vector3 origin) {
            Vector3 gridPostion = (worldPosition - origin) / cellSize;
            int x = Mathf.FloorToInt(gridPostion.x);
            int y = Mathf.FloorToInt(gridPostion.z);
            return new Vector2Int(x, y);
        }

        public override Vector3 Forward => -Vector3.up;
    }
}