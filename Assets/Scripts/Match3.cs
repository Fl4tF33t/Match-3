using System.Collections;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(InputReader))]    
public class Match3 : MonoBehaviour{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 origin = Vector3.zero;
    [SerializeField] private bool debug = true;

    [SerializeField] private Gem gemPrefab;
    [SerializeField] private GemType[] gemTypes;
    [SerializeField] private Ease ease = Ease.InQuad;

    private InputReader inputReader;
    private GridSystem2D<GridObject<Gem>> grid;
    private Vector2Int selectedGem = Vector2Int.one * -1;

    private void Awake(){
        inputReader = GetComponent<InputReader>();
    }

    private void Start(){
        InitializeGrid();
        inputReader.OnFire += OnSelectGem;
    }

    private void OnDestroy(){
        inputReader.OnFire -= OnSelectGem;
    }

    private void OnSelectGem(){ 
        Vector2Int gridPosition = grid.GetXY(Camera.main.ScreenToWorldPoint(inputReader.Selected));

        if (!IsValidPosition(gridPosition) || IsEmptyPosition(gridPosition)) return;

        if (selectedGem == gridPosition){
            DeselectGem();
        }else if (selectedGem == Vector2Int.one * -1){
            SelectGem(gridPosition);
        }else{
            StartCoroutine(RunGameLoop(selectedGem, gridPosition));
        }
    }

    private bool IsEmptyPosition(Vector2Int gridPosition) => grid.GetValue(gridPosition.x, gridPosition.y) == null;

    private bool IsValidPosition(Vector2Int gridPosition) => gridPosition.x >= 0 && gridPosition.y >= 0 && gridPosition.x < width && gridPosition.y < height;

    private void SelectGem(Vector2Int gridPosition) => selectedGem = gridPosition;

    private void DeselectGem() =>selectedGem = Vector2Int.one * -1;
    

    private IEnumerator RunGameLoop(Vector2Int gridPositionA, Vector2Int gridPositionB){
        StartCoroutine(SwapGems(gridPositionA, gridPositionB));       
        DeselectGem();
        yield return null;
    }

    private IEnumerator SwapGems(Vector2Int gridPositionA, Vector2Int gridPositionB){
        GridObject<Gem> gridObjectA = grid.GetValue(gridPositionA.x, gridPositionA.y);
        GridObject<Gem> gridObjectB = grid.GetValue(gridPositionB.x, gridPositionB.y);

        gridObjectA.GetValue().transform
            .DOMove(grid.GetWorldPositionCenter(gridPositionB.x, gridPositionB.y), 0.5f)
            .SetEase(ease);
        gridObjectB.GetValue().transform
            .DOMove(grid.GetWorldPositionCenter(gridPositionA.x, gridPositionA.y), 0.5f)
            .SetEase(ease);
                
        grid.SetValue(gridPositionA.x, gridPositionA.y, gridObjectB);
        grid.SetValue(gridPositionB.x, gridPositionB.y, gridObjectA); 

        yield return new WaitForSeconds(0.5f);
    }

    private void InitializeGrid(){
        grid = GridSystem2D<GridObject<Gem>>.VerticalGrid(width, height, cellSize, origin, debug);
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){
                CreateGem(x, y);
            }
        }
    }

    private void CreateGem(int x, int y){
        Gem gem = Instantiate(gemPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
        gem.SetType(gemTypes[Random.Range(0, gemTypes.Length)]);
        GridObject<Gem> gridObject = new GridObject<Gem>(grid, x, y);
        gridObject.SetValue(gem);
        grid.SetValue(x, y, gridObject);   
    }
}