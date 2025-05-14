using UnityEngine;

public class Match3 : MonoBehaviour{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;
    [SerializeField] private Vector3 origin = Vector3.zero;
    [SerializeField] private bool debug = true;

    private void Start(){
        GridSystem2D<GridObject<Gem>>.VerticalGrid(width, height, cellSize, origin, debug);
    }

}