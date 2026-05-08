using System.Collections.Generic;

public class SlotModel
{
    public int MaxSlots       { get; }
    public List<TileModel> Slots { get; } = new();

    public bool IsFull   => Slots.Count >= MaxSlots;
    public bool HasSpace => Slots.Count < MaxSlots;

    public SlotModel(int maxSlots) => MaxSlots = maxSlots;


    public int GetInsertIndex(TileSymbol symbol)
    {
   
        int lastIdx = -1;
        for (int i = 0; i < Slots.Count; i++)
            if (Slots[i].Symbol == symbol) lastIdx = i;

        return lastIdx >= 0 ? lastIdx + 1 : Slots.Count;
    }

    public void InsertAt(TileModel tile, int index)
    {
        Slots.Insert(index, tile);
    }


    public List<TileModel> FindMatchGroup()
    {
        for (int i = 0; i <= Slots.Count - 3; i++)
        {
            if (Slots[i].Symbol == Slots[i+1].Symbol &&
                Slots[i].Symbol == Slots[i+2].Symbol)
                return new List<TileModel> { Slots[i], Slots[i+1], Slots[i+2] };
        }
        return null;
    }

    public void RemoveTiles(List<TileModel> tiles)
    {
        foreach (var t in tiles) Slots.Remove(t);
    }
}