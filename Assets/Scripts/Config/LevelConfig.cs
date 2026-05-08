using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_XX", menuName = "TileGame/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [Header("Basic")]
    public int levelIndex;
    public int targetMatchCount;   
    public int slotCount = 7;    

    [Header("Board")]
    public int layerCount = 3;
    public int symbolTypeCount = 6; 

    [Header("Time (0 = unlimited)")]
    public float timeLimitSeconds = 0f;

    [Header("Tile Data — auto-generated or manual")]
    public List<TileData> tiles = new();
}

[System.Serializable]
public struct TileData
{
    public TileSymbol symbol;
    public int layer;
    public float posX;
    public float posY;
}