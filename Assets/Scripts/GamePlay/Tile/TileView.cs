using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TileView :
    MonoBehaviour,
    IPointerClickHandler
{
    [SerializeField]
    private Image iconImage;

    public TileData Data { get; private set; }

    public void Initialize(
        TileData data,
        Sprite sprite
    )
    {
        Data = data;

        iconImage.sprite = sprite;
    }

    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        if (Data.IsInSlot)
            return;


        BoardManager.Instance.SelectTile(this);
    }
}