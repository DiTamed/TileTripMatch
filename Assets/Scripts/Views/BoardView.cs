using System.Collections.Generic;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    private const float TileSize = 90f;

    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Sprite tileBaseSprite;
    [SerializeField] private Sprite[] iconSprites;
    [SerializeField] private float layerZOffset = 0.1f;
    [SerializeField] private float tileSpacing = 90f;
    [SerializeField] private RectTransform boardCanvas;

    private BoardModel _boardModel;
    private BoardPresenter _boardPresenter;
    private readonly Dictionary<TileModel, TileView> _tileViews = new();

    public void Init(BoardModel boardModel, BoardPresenter boardPresenter)
    {
        _boardModel = boardModel;
        _boardPresenter = boardPresenter;
        SpawnAllTiles();
        RefreshAllTileVisuals();
        SubscribeEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    private void SpawnAllTiles()
    {
        int totalTiles = 0;
        foreach (List<TileModel> layer in _boardModel.Layers)
            totalTiles += layer.Count;

        Debug.Log($"BoardView: {_boardModel.Layers.Count} layers, {totalTiles} total tiles");

        for (int layerIndex = 0; layerIndex < _boardModel.Layers.Count; layerIndex++)
        {
            Debug.Log($"  Layer {layerIndex}: {_boardModel.Layers[layerIndex].Count} tiles");
            foreach (TileModel tileModel in _boardModel.Layers[layerIndex])
                SpawnTile(tileModel, layerIndex);
        }
    }

    private void SpawnTile(TileModel model, int layerIndex)
    {
        GameObject tileObject = Instantiate(tilePrefab, boardCanvas);
        RectTransform rectTransform = tileObject.GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(TileSize, TileSize);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(model.PosX * tileSpacing, model.PosY * tileSpacing);
        Vector3 localPosition = rectTransform.localPosition;
        rectTransform.localPosition = new Vector3(localPosition.x, localPosition.y, -layerIndex * layerZOffset);

        tileObject.transform.SetSiblingIndex(layerIndex * 100 + _tileViews.Count);

        TileView view = tileObject.GetComponent<TileView>();
        Sprite icon = iconSprites[(int)model.Symbol];
        view.Init(model, icon, tileBaseSprite);
        _tileViews[model] = view;

        Debug.Log($"  Spawning tile {model.Symbol} at anchoredPos {rectTransform.anchoredPosition}, layer {layerIndex}, size {rectTransform.sizeDelta}");
    }

    private void SubscribeEvents()
    {
        GameEventBus.OnTileInserted += HandleTileInserted;
        GameEventBus.OnBoardStateChanged += HandleBoardStateChanged;
    }

    private void UnsubscribeEvents()
    {
        GameEventBus.OnTileInserted -= HandleTileInserted;
        GameEventBus.OnBoardStateChanged -= HandleBoardStateChanged;
    }

    private void HandleBoardStateChanged()
    {
        RefreshAllTileVisuals();
    }

    private void HandleTileInserted(TileModel tile, int slotIndex)
    {
        if (!_tileViews.TryGetValue(tile, out TileView view)) return;

        _tileViews.Remove(tile);

        Vector3 slotTarget = SlotView.Instance != null
            ? SlotView.Instance.GetSlotWorldPosition(slotIndex)
            : view.transform.position;

        view.AnimateMoveToSlot(slotTarget, () =>
        {
            if (SlotView.Instance == null) return;

            SlotView.Instance.RegisterTileView(tile, view);
            SlotView.Instance.RelayoutSlotViews();
        });

        RefreshAllTileVisuals();
    }

    private void RefreshAllTileVisuals()
    {
        foreach (KeyValuePair<TileModel, TileView> pair in _tileViews)
            pair.Value.Refresh();
    }
}
