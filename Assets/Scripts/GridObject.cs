public class GridObject<T> {
    private GridSystem2D<GridObject<T>> grid;
    private int x;
    private int y;
    private T gem;
    public T GridObj {
        get => gem;
        set => gem = value;
    }

    public GridObject(GridSystem2D<GridObject<T>> grid, int x, int y) {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }
}