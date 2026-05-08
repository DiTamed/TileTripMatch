public class TileModel
{
    public TileSymbol Symbol     { get; }
    public int        Layer      { get; }
    public int        Row        { get; }
    public int        Col        { get; }
    public float      PosX       { get; set; }  
    public float      PosY       { get; set; } 
    public bool       IsExposed  { get; set; }
    public bool       IsQueuedForSlot { get; set; }
    public bool       HasMatched { get; set; }

    public TileModel(TileSymbol symbol, int layer, int row, int col)
    {
        Symbol = symbol;
        Layer  = layer;
        Row    = row;
        Col    = col;
    }
}
