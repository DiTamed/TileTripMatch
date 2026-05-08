using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public class GamePresenter
{
    private readonly BoardPresenter _boardPresenter;
    private readonly SlotPresenter _slotPresenter;
    private readonly LevelConfig _config;

    private CancellationTokenSource _cts;
    private readonly Queue<TileModel> _pendingSelections = new();
    private float _timeRemaining;
    private bool _isGameActive;
    private bool _isProcessingSelectionQueue;

    public GamePresenter(BoardPresenter boardPresenter, SlotPresenter slotPresenter, LevelConfig config)
    {
        _boardPresenter = boardPresenter;
        _slotPresenter = slotPresenter;
        _config = config;
    }

    public void StartGame()
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        _pendingSelections.Clear();
        _isProcessingSelectionQueue = false;
        _isGameActive = true;
        _timeRemaining = _config.timeLimitSeconds;

        GameEventBus.OnTileSelected += HandleTileSelected;
        _boardPresenter.RefreshExposedState();
        GameEventBus.ScoreChanged(0);

        if (_config.timeLimitSeconds > 0f)
            GameEventBus.TimerChanged(_timeRemaining);

        if (_config.timeLimitSeconds > 0f)
            RunTimerAsync(_cts.Token).Forget();
    }

    private void HandleTileSelected(TileModel tile)
    {
        if (!_isGameActive) return;
        if (!_boardPresenter.CanTap(tile)) return;

        tile.IsQueuedForSlot = true;
        _pendingSelections.Enqueue(tile);
        GameEventBus.BoardStateChanged();

        if (!_isProcessingSelectionQueue)
            ProcessSelectionQueueAsync().Forget();
    }

    private async UniTaskVoid ProcessSelectionQueueAsync()
    {
        _isProcessingSelectionQueue = true;

        try
        {
            while (_isGameActive && _pendingSelections.Count > 0)
            {
                TileModel tile = _pendingSelections.Dequeue();

                _boardPresenter.RemoveTile(tile);
                GameEventBus.BoardStateChanged();

                SlotInsertResult insertResult = await _slotPresenter.InsertTileAsync(tile, _cts.Token);

                if (!_isGameActive) return;

                if (insertResult == SlotInsertResult.Win)
                {
                    EndGame(isWin: true);
                    return;
                }

                if (insertResult == SlotInsertResult.Overflowed)
                {
                    EndGame(isWin: false);
                    return;
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _isProcessingSelectionQueue = false;
        }
    }

    private async UniTask RunTimerAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (_timeRemaining > 0f && _isGameActive)
            {
                await UniTask.Delay(1000, cancellationToken: cancellationToken);
                _timeRemaining -= 1f;
                GameEventBus.TimerChanged(_timeRemaining);
            }

            if (_isGameActive)
                EndGame(isWin: false);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void EndGame(bool isWin)
    {
        if (!_isGameActive) return;

        _isGameActive = false;
        _cts?.Cancel();
        GameEventBus.OnTileSelected -= HandleTileSelected;

        if (isWin) GameEventBus.GameWin();
        else GameEventBus.GameOver();
    }
}
