using System;
using System.Collections.Generic;

public static class GameEventBus
{
    public static event Action<TileModel>       OnTileSelected;
    public static event Action<TileModel, int>  OnTileInserted;
    public static event Action                  OnBoardStateChanged;
    public static event Action                  OnSlotStateChanged;
    public static event Action<List<TileModel>> OnMatchFound;
    public static event Action<int>             OnScoreChanged;
    public static event Action<float>           OnTimerChanged;
    public static event Action                  OnGameWin;
    public static event Action                  OnGameOver;

    public static void TileSelected(TileModel tile) => OnTileSelected?.Invoke(tile);

    public static void TileInserted(TileModel tile, int slotIndex)
        => OnTileInserted?.Invoke(tile, slotIndex);

    public static void BoardStateChanged() => OnBoardStateChanged?.Invoke();

    public static void SlotStateChanged() => OnSlotStateChanged?.Invoke();

    public static void MatchFound(List<TileModel> tiles) => OnMatchFound?.Invoke(tiles);

    public static void ScoreChanged(int count) => OnScoreChanged?.Invoke(count);

    public static void TimerChanged(float remainingSeconds) => OnTimerChanged?.Invoke(remainingSeconds);

    public static void GameWin() => OnGameWin?.Invoke();

    public static void GameOver() => OnGameOver?.Invoke();

    public static void ClearAll()
    {
        OnTileSelected = null;
        OnTileInserted = null;
        OnBoardStateChanged = null;
        OnSlotStateChanged = null;
        OnMatchFound = null;
        OnScoreChanged = null;
        OnTimerChanged = null;
        OnGameWin = null;
        OnGameOver = null;
    }
}
