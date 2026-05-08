using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotView : MonoBehaviour
{
    public static SlotView Instance { get; private set; }

    [SerializeField] private List<RectTransform> slotAnchors;

    private SlotModel _slotModel;
    private readonly Dictionary<TileModel, TileView> _tileViews = new();
    private RectTransform _trayRect;
    private HorizontalLayoutGroup _layoutGroup;

    private void Awake()
    {
        Instance = this;
        _trayRect = transform as RectTransform;
        TryGetComponent(out _layoutGroup);
    }

    public void Init(SlotModel slotModel)
    {
        _slotModel = slotModel;
        GameEventBus.OnSlotStateChanged += HandleSlotStateChanged;
        GameEventBus.OnMatchFound += OnMatchFound;
    }

    private void OnDestroy()
    {
        GameEventBus.OnSlotStateChanged -= HandleSlotStateChanged;
        GameEventBus.OnMatchFound -= OnMatchFound;
        Instance = null;
    }

    public Vector3 GetSlotWorldPosition(int slotIndex)
    {
        if (!CanCalculateSlotPosition())
            return transform.position;

        return GetEvenSlotWorldPosition(slotIndex);
    }

    public void RegisterTileView(TileModel model, TileView view)
    {
        _tileViews[model] = view;
    }

    public void RelayoutSlotViews()
    {
        RelayoutSlot();
    }

    private void HandleSlotStateChanged()
    {
        RelayoutSlot();
    }

    private void RelayoutSlot()
    {
        if (_slotModel == null || !CanCalculateSlotPosition())
            return;

        for (int i = 0; i < _slotModel.Slots.Count; i++)
        {
            TileModel model = _slotModel.Slots[i];
            if (_tileViews.TryGetValue(model, out TileView view))
            {
                Vector3 target = GetEvenSlotWorldPosition(i);
                view.AnimateMoveToSlot(target);
            }
        }
    }

    private void OnMatchFound(List<TileModel> matched)
    {
        foreach (TileModel tile in matched)
        {
            if (_tileViews.TryGetValue(tile, out TileView view))
            {
                _tileViews.Remove(tile);
                view.AnimateMatch();
            }
        }
    }

    private Vector3 GetEvenSlotWorldPosition(int slotIndex)
    {
        int usableSlotCount = GetUsableSlotCount();
        slotIndex = Mathf.Clamp(slotIndex, 0, Mathf.Max(usableSlotCount - 1, 0));

        if (usableSlotCount <= 1)
            return GetTrayCenterWorldPosition();

        if (_trayRect != null)
            return GetLeftAlignedTrayWorldPosition(slotIndex, usableSlotCount);

        return GetAnchorWorldPosition(slotIndex, usableSlotCount);
    }

    private Vector3 GetLeftAlignedTrayWorldPosition(int slotIndex, int usableSlotCount)
    {
        Rect rect = _trayRect.rect;
        float leftPadding = _layoutGroup != null ? _layoutGroup.padding.left : 0f;
        float rightPadding = _layoutGroup != null ? _layoutGroup.padding.right : 0f;
        float spacing = _layoutGroup != null ? _layoutGroup.spacing : 0f;
        float availableWidth = Mathf.Max(rect.width - leftPadding - rightPadding, 0f);
        float itemWidth = GetSlotItemWidth(usableSlotCount, availableWidth, spacing);
        float localX = rect.xMin + leftPadding + (itemWidth * 0.5f) + slotIndex * (itemWidth + spacing);
        float localY = rect.center.y;

        return _trayRect.TransformPoint(new Vector3(localX, localY, 0f));
    }

    private float GetSlotItemWidth(int usableSlotCount, float availableWidth, float spacing)
    {
        float contentWidth = availableWidth - spacing * Mathf.Max(usableSlotCount - 1, 0);
        if (contentWidth <= 0f)
            return 0f;

        float preferredWidth = GetRegisteredTileWidth();
        if (preferredWidth <= 0f)
            return contentWidth / usableSlotCount;

        return Mathf.Min(preferredWidth, contentWidth / usableSlotCount);
    }

    private float GetRegisteredTileWidth()
    {
        foreach (TileView tileView in _tileViews.Values)
        {
            if (tileView != null && tileView.TryGetComponent(out RectTransform tileRect))
                return tileRect.rect.width;
        }

        return 0f;
    }

    private Vector3 GetAnchorWorldPosition(int slotIndex, int usableSlotCount)
    {
        if (!HasValidAnchors())
            return transform.position;

        if (slotAnchors.Count == 1)
            return slotAnchors[0].position;

        Vector3 left = slotAnchors[0].position;
        Vector3 right = slotAnchors[slotAnchors.Count - 1].position;
        float t = slotIndex / (float)(usableSlotCount - 1);
        return Vector3.Lerp(left, right, t);
    }

    private int GetUsableSlotCount()
    {
        if (_slotModel == null)
            return slotAnchors != null && slotAnchors.Count > 0 ? slotAnchors.Count : 1;

        int configuredSlotCount = HasValidAnchors() ? slotAnchors.Count : _slotModel.MaxSlots;
        return Mathf.Clamp(_slotModel.MaxSlots, 1, Mathf.Max(configuredSlotCount, 1));
    }

    private Vector3 GetTrayCenterWorldPosition()
    {
        if (_trayRect != null)
            return _trayRect.TransformPoint(_trayRect.rect.center);

        Vector3 left = slotAnchors[0].position;
        Vector3 right = slotAnchors[slotAnchors.Count - 1].position;
        return (left + right) * 0.5f;
    }

    private bool CanCalculateSlotPosition()
    {
        return _trayRect != null || HasValidAnchors();
    }

    private bool HasValidAnchors()
    {
        return slotAnchors != null && slotAnchors.Count > 0;
    }
}
