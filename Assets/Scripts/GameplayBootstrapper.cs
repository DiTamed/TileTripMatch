using UnityEngine;

public class GameplayBootstrapper : MonoBehaviour
{
    [SerializeField] private BoardView boardView;
    [SerializeField] private SlotView slotView;
    [SerializeField] private GameUIView gameUIView;
    [SerializeField] private LevelConfig fallbackLevel;

    private GamePresenter _gamePresenter;

    private void Start()
    {
        var levelConfig = LevelSession.GetCurrentOrFallback(fallbackLevel);

        if (levelConfig == null)
        {
            Debug.LogError("Missing LevelConfig. Assign fallbackLevel in the inspector.");
            return;
        }

        LevelSession.SetSelectedConfig(levelConfig);
        Debug.Log($"Loading level {levelConfig.levelIndex}");

        var boardModel = LevelGenerator.GenerateBoard(levelConfig);
        var slotModel = new SlotModel(levelConfig.slotCount);
        var boardPresenter = new BoardPresenter(boardModel);
        var slotPresenter = new SlotPresenter(slotModel, levelConfig.targetMatchCount);
        _gamePresenter = new GamePresenter(boardPresenter, slotPresenter, levelConfig);

        boardPresenter.RefreshExposedState();
        boardView.Init(boardModel, boardPresenter);
        slotView.Init(slotModel);
        gameUIView.Init(levelConfig);

        _gamePresenter.StartGame();
    }

    private void OnDestroy()
    {
        GameEventBus.ClearAll();
    }
}
