using System;
using System.Collections.Generic;
using UnityEngine;
public class BoardManager : Singleton<BoardManager> {
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 origin = Vector3.zero;
    [SerializeField] private bool debug = true;
    private GridSystem2D<GridObject<Gem>> grid;
    public GridSystem2D<GridObject<Gem>> Grid => grid;
    
    [SerializeField] private GemFactory gemFactory;
    private GameObject gemsHolder;
    
    public event Action OnGemCreation;
    private void Start() {
        InitializeGrid();    
    }
    public void SwapGems(Vector2Int gridPositionA, Vector2Int gridPositionB, out GameObject objA, out GameObject objB){
        GridObject<Gem> gridObjectA = grid.GetValue(gridPositionA.x, gridPositionA.y);
        GridObject<Gem> gridObjectB = grid.GetValue(gridPositionB.x, gridPositionB.y);
        grid.SetValue(gridPositionA.x, gridPositionA.y, gridObjectB);
        grid.SetValue(gridPositionB.x, gridPositionB.y, gridObjectA);
        objA = gridObjectA.GridObj.gameObject;
        objB = gridObjectB.GridObj.gameObject;
    }
    public bool IsEmptyPosition(Vector2Int gridPosition) => grid.GetValue(gridPosition.x, gridPosition.y) == null;
    public bool MatchFound(out List<Vector2Int> matchList){
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();
        
        //Horizontal
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width - 2; x++) {
                GridObject<Gem> gemA = grid.GetValue(x, y);
                GridObject<Gem> gemB = grid.GetValue(x + 1, y);
                GridObject<Gem> gemC = grid.GetValue(x + 2, y);

                if (gemA == null || gemB == null || gemC == null) continue;

                if (gemA.GridObj.GetGemType() == gemB.GridObj.GetGemType() && gemB.GridObj.GetGemType()  == gemC.GridObj.GetGemType()) {
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x + 1, y));
                    matches.Add(new Vector2Int(x + 2, y));
                }
            }
        }

        //Vertical
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height - 2; y++) {
                GridObject<Gem> gemA = grid.GetValue(x, y);
                GridObject<Gem> gemB = grid.GetValue(x, y + 1);
                GridObject<Gem> gemC = grid.GetValue(x, y + 2);

                if (gemA == null || gemB == null || gemC == null) continue;

                if (gemA.GridObj.GetGemType() == gemB.GridObj.GetGemType() && gemB.GridObj.GetGemType() == gemC.GridObj.GetGemType()) {
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x, y + 1));
                    matches.Add(new Vector2Int(x, y + 2));
                }
            }
        }

        matchList = new List<Vector2Int>(matches);
        return matchList.Count != 0;
    }

    public void DeleteMatches(List<Vector2Int> matchList, out List<Gem> gemList) {
        gemList = new List<Gem>();
        foreach (Vector2Int match in matchList){
            Gem gem = grid.GetValue(match.x, match.y).GridObj;
            gemList.Add(gem);
            grid.SetValue(match.x, match.y, null);
        }
    }
    
    public void FillEmptySpots(){
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){
                if (grid.GetValue(x, y) == null){
                    OnGemCreation?.Invoke();
                    InitializeGem(x, y, gemsHolder.transform);
                }
            }
        }
    }
    public void MakeGemsFall(out List<Gem> gemList, out List<Vector3> fallPositions) {
        gemList = new List<Gem>();
        fallPositions = new List<Vector3>();

        for (int x = 0; x < width; x++) {
            int emptyY = -1;
            for (int y = 0; y < height; y++) {
                GridObject<Gem> cell = grid.GetValue(x, y);
                if (cell == null) {
                    if (emptyY == -1)
                        emptyY = y;
                } else if (emptyY != -1) {
                    grid.SetValue(x, emptyY, cell);
                    grid.SetValue(x, y, null);
                    Gem gem = cell.GridObj;
                    gemList.Add(gem);
                    fallPositions.Add(grid.GetWorldPositionCenter(x, emptyY));
                    emptyY++;
                }
            }
        }
    }
    
    private void InitializeGrid(){
        grid = GridSystem2D<GridObject<Gem>>.CreateGrid(GridSystem2D<GridObject<Gem>>.GridLayout.Vertical, width, height, cellSize, origin, debug);
        gemsHolder = new GameObject("Gem Holder");
        gemsHolder.transform.SetParent(transform);
        
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){
                InitializeGem(x, y, gemsHolder.transform);
            }
        }
    }
    private void InitializeGem(int x, int y, Transform parent) {
        Gem gem = gemFactory.Create();
        gem.transform.SetParent(parent);
        gem.transform.position = grid.GetWorldPositionCenter(x, y);
        GridObject<Gem> gridObject = new GridObject<Gem>(grid, x, y);
        gridObject.GridObj = gem;
        grid.SetValue(x, y, gridObject);   
    }
}
