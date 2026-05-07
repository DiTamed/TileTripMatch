public class TileData
{
    public int Id;
    
    public bool IsInSlot;
    public bool IsBlocked;
    public bool HasMatched;

    public TileData(int id)
    {
        Id = id;
    }
}