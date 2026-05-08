using System.Collections.Generic;

public class BoardModel
{

    public List<List<TileModel>> Layers { get; } = new();

    public void AddLayer(List<TileModel> layer) => Layers.Add(layer);

    public IEnumerable<TileModel> AllTiles()
    {
        foreach (var layer in Layers)
            foreach (var tile in layer)
                yield return tile;
    }
}