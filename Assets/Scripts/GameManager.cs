using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameManager : Singleton<GameManager> {
    private BoardManager boardManager;
    private PlayerController playerController;
    private AnimationManager animationManager;
    private new Camera camera;
    private GridSystem2D<GridObject<Gem>> grid;
    
    private Vector2Int selectedGem = Vector2Int.one * -1;
    private Gem currentSelectedGem = null;
    public event Action<bool, Gem> OnSelection;
    public event Action<bool> OnFindMatch;
    private void Start() {
        boardManager = BoardManager.Instance;
        grid = boardManager.Grid;
        playerController = PlayerController.Instance;
        animationManager = AnimationManager.Instance;
        camera = Camera.main;
        
        playerController.OnClick += PlayerController_OnClick;
    }

    private void PlayerController_OnClick(Vector2 obj) {
        Vector2Int gridPosition = grid.GetXY(camera.ScreenToWorldPoint(obj));

        if (!grid.IsValid(gridPosition.x, gridPosition.y) || boardManager.IsEmptyPosition(gridPosition)) return;

        if (selectedGem == gridPosition){
            //turn to events
            //audioManager.PlayDeselect();
            DeselectGem();
        }else if (selectedGem == Vector2Int.one * -1){
            //turn to events
            //audioManager.PlayClick();
            SelectGem(gridPosition);
        }else{
            StartCoroutine(RunGameLoop(selectedGem, gridPosition));
        }
    }
    private void SelectGem(Vector2Int gridPosition) {
        selectedGem = gridPosition;
        currentSelectedGem = grid.GetValue(gridPosition.x, gridPosition.y).GridObj;
        OnSelection?.Invoke(true, currentSelectedGem);
    }

    private void DeselectGem() {
        selectedGem = Vector2Int.one * -1;
        OnSelection?.Invoke(false, currentSelectedGem);
        currentSelectedGem = null;
    } 
    private IEnumerator RunGameLoop(Vector2Int gridPositionA, Vector2Int gridPositionB) {
        boardManager.SwapGems(gridPositionA, gridPositionB, out GameObject objA, out GameObject objB);
        yield return StartCoroutine(animationManager.SwapGem(objA, objB));
        DeselectGem();

        if (boardManager.MatchFound(out List<Vector2Int> matches)) {
            OnFindMatch?.Invoke(true);
            boardManager.DeleteMatches(matches, out List<Gem> deleteGemList);
            yield return animationManager.DeleteMatches(deleteGemList);
            
            boardManager.MakeGemsFall(out List<Gem> fallGemList, out List<Vector3> fallPositionsList);
            yield return animationManager.MakeGemsFall(fallGemList, fallPositionsList);
            
            boardManager.FillEmptySpots();
        } else OnFindMatch?.Invoke(false);
    }
}
