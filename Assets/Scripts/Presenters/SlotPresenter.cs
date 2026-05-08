using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public enum SlotInsertResult
{
    Inserted,
    Overflowed,
    Win
}

public class SlotPresenter
{
    private const int InsertAnimationDelayMs = 400;
    private const int MatchAnimationDelayMs = 400;

    private readonly SlotModel _slot;
    private readonly int _targetMatchCount;
    private int _matchCount;

    public SlotPresenter(SlotModel slot, int targetMatchCount)
    {
        _slot = slot;
        _targetMatchCount = targetMatchCount;
    }

    public async UniTask<SlotInsertResult> InsertTileAsync(TileModel tile, CancellationToken cancellationToken)
    {
        if (!_slot.HasSpace)
            return SlotInsertResult.Overflowed;

        int insertIndex = _slot.GetInsertIndex(tile.Symbol);
        _slot.InsertAt(tile, insertIndex);

        GameEventBus.TileInserted(tile, insertIndex);
        GameEventBus.SlotStateChanged();

        await UniTask.Delay(InsertAnimationDelayMs, cancellationToken: cancellationToken);
        await CheckMatchAsync(cancellationToken);

        if (_matchCount >= _targetMatchCount)
            return SlotInsertResult.Win;

        return _slot.IsFull ? SlotInsertResult.Overflowed : SlotInsertResult.Inserted;
    }

    private async UniTask CheckMatchAsync(CancellationToken cancellationToken)
    {
        List<TileModel> group = _slot.FindMatchGroup();
        if (group == null) return;

        foreach (TileModel tile in group)
            tile.HasMatched = true;

        GameEventBus.MatchFound(group);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMatchSound();

        await UniTask.Delay(MatchAnimationDelayMs, cancellationToken: cancellationToken);

        _slot.RemoveTiles(group);
        _matchCount++;

        GameEventBus.SlotStateChanged();
        GameEventBus.ScoreChanged(_matchCount);
    }

    public bool IsOverflowed() => _slot.IsFull;
}
