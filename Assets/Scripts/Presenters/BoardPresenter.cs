public class BoardPresenter
{
    private readonly BoardModel _board;

    public BoardPresenter(BoardModel board) => _board = board;

    public void RefreshExposedState()
    {
   
        foreach (var tile in _board.AllTiles())
            tile.IsExposed = true;


        for (int layer = 1; layer < _board.Layers.Count; layer++)
        {
            foreach (var upper in _board.Layers[layer])
            {
                foreach (var lower in _board.Layers[layer - 1])
                {
                    if (TilesOverlap(upper, lower))
                        lower.IsExposed = false;
                }
            }
        }
    }

    private bool TilesOverlap(TileModel a, TileModel b)
    {

        return a.Row == b.Row && a.Col == b.Col;
   
    }

    public bool CanTap(TileModel tile) => tile.IsExposed && !tile.IsQueuedForSlot && !tile.HasMatched;

    public void RemoveTile(TileModel tile)
    {
        foreach (var layer in _board.Layers)
            layer.Remove(tile);
        RefreshExposedState();
    }
}
