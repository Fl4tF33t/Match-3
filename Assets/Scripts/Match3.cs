using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

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
    [SerializeField] private Transform destroyVfx;

    private AudioManager audioManager;
    private InputReader inputReader;
    private GridSystem2D<GridObject<Gem>> grid;
    private Vector2Int selectedGem = Vector2Int.one * -1;

    private void Awake(){
        inputReader = GetComponent<InputReader>();
        audioManager = GetComponent<AudioManager>();
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
            audioManager.PlayDeselect();
            DeselectGem();
        }else if (selectedGem == Vector2Int.one * -1){
            audioManager.PlayClick();
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

        List<Vector2Int> matches = FindMatches();    

        yield return StartCoroutine(DeleteMatches(matches));
        yield return StartCoroutine(MakeGemsFall());
        yield return StartCoroutine(FillEmptySpots());
        DeselectGem();
    }

    private IEnumerator FillEmptySpots(){
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){
                if (grid.GetValue(x, y) == null){
                    CreateGem(x, y);
                    audioManager.PlayPop();
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }

    }

    private IEnumerator MakeGemsFall(){
        for (int x = 0; x < width; x++){
            for (int y = 0; y < height; y++){
                if (grid.GetValue(x, y) == null){
                    for (int i = y + 1; i < height; i++){
                        if (grid.GetValue(x, i) != null){
                            Gem gem = grid.GetValue(x, i).GetValue();
                            grid.SetValue(x, y, grid.GetValue(x, i));
                            grid.SetValue(x, i, null);
                            gem.transform
                                .DOLocalMove(grid.GetWorldPositionCenter(x, y), 0.5f)
                                .SetEase(ease);
                            audioManager.PlayWhoosh();
                            yield return new WaitForSeconds(0.51f);
                            break;
                        }
                    }
                }
            }
        }
    }

    private IEnumerator DeleteMatches(List<Vector2Int> matches){
        audioManager.PlayPop();

        foreach (Vector2Int match in matches){
            Gem gem = grid.GetValue(match.x, match.y).GetValue();
            grid.SetValue(match.x, match.y, null);

            DestroyVfx(match);

            gem.transform
                .DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 0.5f);

            yield return new WaitForSeconds(0.11f);

            gem.DestroyGem();    
        }
    }

    private void DestroyVfx(Vector2Int position) {
        Transform vfx = Instantiate(destroyVfx, transform);
        vfx.position = grid.GetWorldPositionCenter(position.x, position.y);
        Destroy(vfx.gameObject, 5f);
    }

    private List<Vector2Int> FindMatches(){
        HashSet<Vector2Int> matches = new HashSet<Vector2Int>();
        
        //Horizontal
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width - 2; x++) {
                GridObject<Gem> gemA = grid.GetValue(x, y);
                GridObject<Gem> gemB = grid.GetValue(x + 1, y);
                GridObject<Gem> gemC = grid.GetValue(x + 2, y);

                if (gemA == null || gemB == null || gemC == null) continue;

                if (gemA.GetValue().GetType() == gemB.GetValue().GetType() && gemB.GetValue().GetType()  == gemC.GetValue().GetType()) {
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

                if (gemA.GetValue().GetType() == gemB.GetValue().GetType() && gemB.GetValue().GetType()  == gemC.GetValue().GetType()) {
                    matches.Add(new Vector2Int(x, y));
                    matches.Add(new Vector2Int(x, y + 1));
                    matches.Add(new Vector2Int(x, y + 2));
                }
            }
        }
        if (matches.Count > 0) {
            audioManager.PlayMatch();
        } else audioManager.PlayNoMatch();

        return new List<Vector2Int>(matches);
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