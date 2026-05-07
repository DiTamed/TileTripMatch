using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [SerializeField]
    private TileView tilePrefab;

    [SerializeField]
    private Sprite[] tileSprites;

    [SerializeField]
    private Transform boardParent;

    [SerializeField]
    private int tileCount = 30;

    private int remainingTiles;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        remainingTiles = tileCount;

        List<int> ids =
            GenerateTileIds(tileCount);

        for (int i = 0; i < tileCount; i++)
        {
            int id = ids[i];

            TileData data =
                new TileData(id);

            TileView tile =
                Instantiate(
                    tilePrefab,
                    boardParent
                );

            tile.Initialize(
                data,
                tileSprites[id]
            );

        }
    }

    private List<int> GenerateTileIds(
        int totalCount
    )
    {
        List<int> ids = new();

        int groupCount =
            totalCount / 3;

        for (int i = 0; i < groupCount; i++)
        {
            int randomId =
                Random.Range(
                    0,
                    tileSprites.Length
                );

            ids.Add(randomId);
            ids.Add(randomId);
            ids.Add(randomId);
        }

        Shuffle(ids);

        return ids;
    }

    private void Shuffle(
        List<int> list
    )
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex =
                Random.Range(i, list.Count);

            (list[i], list[randomIndex]) =
                (list[randomIndex], list[i]);
        }
    }

    public void SelectTile(
        TileView tile
    )
    {
        if (SlotManager.Instance.IsFull())
        {
            Debug.Log("SLOT FULL");

            return;
        }

        tile.Data.IsInSlot = true;
        AudioManager.Instance.PlayClickSound();
        SlotManager.Instance.AddTile(tile);

        remainingTiles--;
    }

    public void CheckWin()
    {
        if (remainingTiles <= 0)
        {
            Debug.Log("WIN");
        }
    }
    public void RemoveTiles(int amount)
    {
        remainingTiles -= amount;

        CheckWin();
    }
}