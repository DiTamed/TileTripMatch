using System.Collections.Generic;
using UnityEngine;

public static class LevelGenerator
{
    public static BoardModel GenerateBoard(LevelConfig config)
    {

        if (config == null)
        {
            Debug.LogError("LevelGenerator: config is NULL");
            return new BoardModel();
        }

        if (config.tiles == null || config.tiles.Count == 0)
        {
            Debug.LogWarning($"LevelGenerator: Level {config.levelIndex} has no tiles, generating runtime fallback");
            return GenerateFallback(config);
        }

        var board = new BoardModel();

        for (int i = 0; i < config.layerCount; i++)
            board.AddLayer(new List<TileModel>());


        foreach (var tileData in config.tiles)
        {
            int safeLayer = Mathf.Clamp(tileData.layer, 0, config.layerCount - 1);

            int row = Mathf.RoundToInt(tileData.posY / 1.1f);
            int col = Mathf.RoundToInt(tileData.posX / 1.1f);

            var model = new TileModel(tileData.symbol, safeLayer, row, col);
            model.PosX = tileData.posX;
            model.PosY = tileData.posY;

            board.Layers[safeLayer].Add(model);
        }

        return board;
    }

    private static BoardModel GenerateFallback(LevelConfig config)
    {
        var board = new BoardModel();
        var allSymbols = (TileSymbol[])System.Enum.GetValues(typeof(TileSymbol));
        int useSymbols = Mathf.Min(config.symbolTypeCount, allSymbols.Length);
        int totalTiles = config.targetMatchCount * 3;
        int perLayer = Mathf.CeilToInt((float)totalTiles / config.layerCount);
        int cols = Mathf.CeilToInt(Mathf.Sqrt(perLayer));
        float tileSize = 1.1f;
        float startX = -(cols - 1) * tileSize * 0.5f;

        // Pool bội số 3
        var pool = new List<TileSymbol>();
        int perSym = Mathf.Max(3, (totalTiles / useSymbols / 3) * 3);
        for (int s = 0; s < useSymbols && pool.Count < totalTiles; s++)
            for (int k = 0; k < perSym && pool.Count < totalTiles; k++)
                pool.Add(allSymbols[s]);

        // Shuffle
        var rng = new System.Random(config.levelIndex * 100);
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        int idx = 0;
        for (int layer = 0; layer < config.layerCount; layer++)
        {
            var layerList = new List<TileModel>();
            int count = Mathf.Min(perLayer, totalTiles - idx);

            for (int t = 0; t < count && idx < pool.Count; t++, idx++)
            {
                int row = t / cols;
                int col = t % cols;
                float px = startX + col * tileSize + layer * 0.12f;
                float py = -(Mathf.CeilToInt((float)perLayer / cols) - 1)
                           * tileSize * 0.5f + row * tileSize + layer * 0.12f;

                var model = new TileModel(pool[idx], layer, row, col);
                model.PosX = px;
                model.PosY = py;
                layerList.Add(model);
            }

            board.AddLayer(layerList);
        }

        return board;
    }
}