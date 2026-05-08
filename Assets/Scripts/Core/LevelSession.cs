using UnityEngine;

public static class LevelSession
{
    private const int DefaultLevelIndex = 1;

    public static LevelConfig SelectedConfig { get; private set; }

    public static int CurrentLevelIndex =>
        SelectedConfig != null ? SelectedConfig.levelIndex : DefaultLevelIndex;

    public static void SetSelectedConfig(LevelConfig config)
    {
        SelectedConfig = config;
    }

    public static LevelConfig GetCurrentOrFallback(LevelConfig fallbackConfig)
    {
        if (SelectedConfig != null)
            return SelectedConfig;

        if (fallbackConfig != null)
        {
            SelectedConfig = fallbackConfig;
            return fallbackConfig;
        }

        TrySelectLevel(DefaultLevelIndex);
        return SelectedConfig;
    }

    public static bool TrySelectLevel(int levelIndex)
    {
        LevelConfig config = LoadLevelConfig(levelIndex);
        if (config == null)
            return false;

        SelectedConfig = config;
        return true;
    }

    public static bool ReloadCurrentLevel()
    {
        return TrySelectLevel(CurrentLevelIndex);
    }

    public static bool TryAdvanceToNextLevel()
    {
        return TrySelectLevel(CurrentLevelIndex + 1);
    }

    public static bool HasNextLevel()
    {
        return LoadLevelConfig(CurrentLevelIndex + 1) != null;
    }

    public static void ClearSelection()
    {
        SelectedConfig = null;
    }

    private static LevelConfig LoadLevelConfig(int levelIndex)
    {
        return Resources.Load<LevelConfig>($"Levels/Level_{levelIndex:D2}");
    }
}
