using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public static SlotManager Instance;

    [SerializeField]
    private Transform slotParent;

    private readonly List<TileView> slots =
        new();

    private const int MAX_SLOT = 5;

    public bool IsFull()
    {
        return slots.Count >= MAX_SLOT;
    }
    private void Awake()
    {
        Instance = this;
    }

    public void AddTile(TileView tile)
    {
        slots.Add(tile);

        tile.transform.SetParent(
            slotParent,
            false
        );

        RearrangeSlots();

        CheckMatch();

        CheckLose();
    }

    private void RearrangeSlots()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            RectTransform rect =
                slots[i]
                .GetComponent<RectTransform>();

            rect.anchoredPosition =
                new Vector2(i * 120f, 0f);
        }
    }

    private void CheckMatch()
    {
        Dictionary<int, List<TileView>> groups =
            new();

        foreach (TileView tile in slots)
        {
            int id = tile.Data.Id;

            if (!groups.ContainsKey(id))
            {
                groups[id] =
                    new List<TileView>();
            }

            groups[id].Add(tile);
        }

        foreach (var pair in groups)
        {
            if (pair.Value.Count >= 3)
            {
                ClearMatch(pair.Value);
                return;
            }
        }
    }

    private void ClearMatch(
    List<TileView> matchedTiles
)
    {
        for (int i = 0; i < 3; i++)
        {
            TileView tile =
                matchedTiles[i];

            slots.Remove(tile);

            Destroy(tile.gameObject);
        }

        BoardManager.Instance.RemoveTiles(3);

        RearrangeSlots();
    }

    private void CheckLose()
    {
        if (slots.Count < MAX_SLOT)
            return;

        bool hasPossibleMatch = false;

        Dictionary<int, int> counts =
            new();

        foreach (TileView tile in slots)
        {
            int id = tile.Data.Id;

            if (!counts.ContainsKey(id))
            {
                counts[id] = 0;
            }

            counts[id]++;

            if (counts[id] >= 3)
            {
                hasPossibleMatch = true;
                break;
            }
        }

        if (!hasPossibleMatch)
        {
            Debug.Log("LOSE");
        }
    }
}