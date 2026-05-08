#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class LevelConfigGenerator : EditorWindow
{
    private const string OutputPath = "Assets/Resources/Levels";

    [MenuItem("TileGame/Generate All 10 Levels")]
    public static void GenerateAll()
    {
        if (!Directory.Exists(OutputPath))
            Directory.CreateDirectory(OutputPath);

        for (int i = 1; i <= 10; i++)
            CreateLevel(i);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ Generated 10 LevelConfigs!");
    }

    private static void CreateLevel(int levelIndex)
    {
        var config = ScriptableObject.CreateInstance<LevelConfig>();
        ApplyLevelSettings(config, levelIndex);

        config.tiles = GenerateTiles(config);

        string path = $"{OutputPath}/Level_{levelIndex:D2}.asset";
        AssetDatabase.CreateAsset(config, path);
    }


    private static void ApplyLevelSettings(LevelConfig c, int level)
    {
        c.levelIndex = level;

        switch (level)
        {
            case 1:
                c.targetMatchCount = 3;  c.slotCount = 7;
                c.layerCount = 2;        c.symbolTypeCount = 4;
                c.timeLimitSeconds = 0f;
                break;
            case 2:
                c.targetMatchCount = 4;  c.slotCount = 7;
                c.layerCount = 2;        c.symbolTypeCount = 5;
                c.timeLimitSeconds = 0f;
                break;
            case 3:
                c.targetMatchCount = 5;  c.slotCount = 7;
                c.layerCount = 3;        c.symbolTypeCount = 5;
                c.timeLimitSeconds = 0f;
                break;
            case 4:
                c.targetMatchCount = 6;  c.slotCount = 7;
                c.layerCount = 3;        c.symbolTypeCount = 6;
                c.timeLimitSeconds = 0f;
                break;
            case 5:
                c.targetMatchCount = 7;  c.slotCount = 6;
                c.layerCount = 3;        c.symbolTypeCount = 7;
                c.timeLimitSeconds = 120f;
                break;
            case 6:
                c.targetMatchCount = 8;  c.slotCount = 6;
                c.layerCount = 4;        c.symbolTypeCount = 8;
                c.timeLimitSeconds = 110f;
                break;
            case 7:
                c.targetMatchCount = 9;  c.slotCount = 6;
                c.layerCount = 4;        c.symbolTypeCount = 9;
                c.timeLimitSeconds = 100f;
                break;
            case 8:
                c.targetMatchCount = 10; c.slotCount = 5;
                c.layerCount = 5;        c.symbolTypeCount = 10;
                c.timeLimitSeconds = 90f;
                break;
            case 9:
                c.targetMatchCount = 12; c.slotCount = 5;
                c.layerCount = 5;        c.symbolTypeCount = 12;
                c.timeLimitSeconds = 80f;
                break;
            case 10:
                c.targetMatchCount = 14; c.slotCount = 5;
                c.layerCount = 6;        c.symbolTypeCount = 14;
                c.timeLimitSeconds = 60f;
                break;
        }
    }

    // ── Sinh tile đảm bảo solvable ──────────────────────────────────────

    private static List<TileData> GenerateTiles(LevelConfig c)
    {
  
        var pool = BuildSymbolPool(c.targetMatchCount, c.symbolTypeCount);

        Shuffle(pool);


        return DistributeToLayers(pool, c.layerCount);
    }

    private static List<TileSymbol> BuildSymbolPool(int targetMatches, int symbolTypes)
    {
        var symbols = new List<TileSymbol>();


        var allSymbols = (TileSymbol[])System.Enum.GetValues(typeof(TileSymbol));
        int useCount   = Mathf.Min(symbolTypes, allSymbols.Length);

        int totalTiles    = targetMatches * 3;
        int basePerSymbol = totalTiles / useCount;        
        basePerSymbol     = (basePerSymbol / 3) * 3;      
        if (basePerSymbol < 3) basePerSymbol = 3;

        for (int i = 0; i < useCount; i++)
        {
            int count = basePerSymbol;
      
            int remaining = totalTiles - basePerSymbol * useCount;
            if (i < remaining / 3) count += 3;

            for (int j = 0; j < count; j++)
                symbols.Add(allSymbols[i]);
        }

        return symbols;
    }

    private static List<TileData> DistributeToLayers(List<TileSymbol> pool, int layerCount)
    {
        var result     = new List<TileData>();
        int total      = pool.Count;
        int perLayer   = Mathf.CeilToInt((float)total / layerCount);

        // Grid layout: cố gắng làm hình vuông gần nhất
        int cols       = Mathf.CeilToInt(Mathf.Sqrt(perLayer));
        int rows       = Mathf.CeilToInt((float)perLayer / cols);

        float tileSize = 1.1f;
        float offsetX  = -(cols - 1) * tileSize * 0.5f;
        float offsetY  = -(rows - 1) * tileSize * 0.5f;

        int idx = 0;
        for (int layer = 0; layer < layerCount && idx < total; layer++)
        {
            int layerTileCount = Mathf.Min(perLayer, total - idx);

            // Layer trên cùng nhỏ hơn để tạo cảm giác pyramid
            float scale = 1f - layer * 0.05f;

            for (int t = 0; t < layerTileCount && idx < total; t++, idx++)
            {
                int row = t / cols;
                int col = t % cols;

                // Offset nhẹ mỗi layer để tạo hiệu ứng xếp chồng
                float jitterX = layer * 0.15f;
                float jitterY = layer * 0.15f;

                result.Add(new TileData
                {
                    symbol = pool[idx],
                    layer  = layer,
                    posX   = offsetX + col * tileSize * scale + jitterX,
                    posY   = offsetY + row * tileSize * scale + jitterY,
                });
            }
        }

        return result;
    }

    private static void Shuffle<T>(List<T> list)
    {
        // Dùng seed cố định để reproducible (debug dễ hơn)
        var rng = new System.Random(42);
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
#endif