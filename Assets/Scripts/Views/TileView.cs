using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TileView : MonoBehaviour
{
    private const float TweenDuration = 0.35f;
    private const float DimAlpha = 0.4f;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button tapButton;

    public TileModel Model { get; private set; }

    public void Init(TileModel model, Sprite iconSprite, Sprite tileBaseSprite)
    {
        Model = model;
        backgroundImage.sprite = tileBaseSprite;
        iconImage.sprite = iconSprite;
        tapButton.onClick.RemoveListener(OnTapped);
        tapButton.onClick.AddListener(OnTapped);
        Refresh();
    }

    private void OnTapped()
    {
        GameEventBus.TileSelected(Model);
    }

    public void Refresh()
    {
        bool shouldDim = !Model.IsExposed;
        float targetAlpha = shouldDim ? DimAlpha : 1f;

        backgroundImage.color = new Color(1f, 1f, 1f, targetAlpha);
        iconImage.color = new Color(1f, 1f, 1f, targetAlpha);
        tapButton.interactable = Model.IsExposed && !Model.IsQueuedForSlot && !Model.HasMatched;
    }

    public void AnimateMoveToSlot(Vector3 targetWorldPos, System.Action onComplete = null)
    {
        tapButton.interactable = false;
        transform.DOMove(targetWorldPos, TweenDuration)
            .SetEase(Ease.InOutQuart)
            .OnComplete(() => onComplete?.Invoke());
    }

    public void AnimateMatch(System.Action onComplete = null)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(1.3f, 0.15f).SetEase(Ease.OutBack));
        sequence.Append(transform.DOScale(0f, 0.2f).SetEase(Ease.InBack));
        sequence.Join(GetCanvasGroup().DOFade(0f, 0.2f));
        sequence.OnComplete(() =>
        {
            onComplete?.Invoke();
            Destroy(gameObject);
        });
    }

    private CanvasGroup GetCanvasGroup()
    {
        if (TryGetComponent(out CanvasGroup canvasGroup))
            return canvasGroup;

        return gameObject.AddComponent<CanvasGroup>();
    }
}
