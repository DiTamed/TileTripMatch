using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class GameUIView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI matchCountText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private GameObject      winPanel;
    [SerializeField] private GameObject      losePanel;
    [SerializeField] private Button          nextLevelButton;
    [SerializeField] private Button          retryButton;

    private LevelConfig _levelConfig;

    private void Awake()
    {
        BindButton(nextLevelButton, HandleNextLevelClicked);
        BindButton(retryButton, HandleRetryClicked);
        EnsureLevelTextExists();
        ResetPanels();
        EnsureHudOnTop();
    }

    public void Init(LevelConfig levelConfig)
    {
        _levelConfig = levelConfig;
        RefreshStaticHud();
    }

    private void OnEnable()
    {
        GameEventBus.OnScoreChanged += UpdateScore;
        GameEventBus.OnTimerChanged += UpdateTimer;
        GameEventBus.OnGameWin      += ShowWinPanel;
        GameEventBus.OnGameOver     += ShowLosePanel;

        RefreshStaticHud();
    }

    private void OnDisable()
    {
        GameEventBus.OnScoreChanged -= UpdateScore;
        GameEventBus.OnTimerChanged -= UpdateTimer;
        GameEventBus.OnGameWin      -= ShowWinPanel;
        GameEventBus.OnGameOver     -= ShowLosePanel;
    }

    private void UpdateScore(int matchCount)
    {
        matchCountText.text = $"{matchCount}";

        if (matchCount > 0)
            matchCountText.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f);
    }

    private void UpdateTimer(float remainingSeconds)
    {
        if (timerText == null)
            return;

        int totalSeconds = Mathf.Max(0, Mathf.CeilToInt(remainingSeconds));
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void ShowWinPanel()
    {
        EnsureHudOnTop();
        winPanel.SetActive(true);
        winPanel.transform.localScale = Vector3.zero;
        winPanel.transform.SetAsLastSibling();
        winPanel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
    }

    private void ShowLosePanel()
    {
        EnsureHudOnTop();
        losePanel.SetActive(true);
        losePanel.transform.localScale = Vector3.zero;
        losePanel.transform.SetAsLastSibling();
        losePanel.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
    }

    private void HandleNextLevelClicked()
    {
        PlayClickSound();

        if (LevelSession.TryAdvanceToNextLevel())
        {
            SceneManager.LoadScene("GamePlay");
            return;
        }

        LevelSession.ClearSelection();
        SceneManager.LoadScene("Home");
    }

    private void HandleRetryClicked()
    {
        PlayClickSound();

        if (!LevelSession.ReloadCurrentLevel() && !LevelSession.TrySelectLevel(1))
        {
            Debug.LogError("Unable to reload the current level.");
            return;
        }

        SceneManager.LoadScene("GamePlay");
    }

    private void RefreshStaticHud()
    {
        EnsureHudOnTop();
        UpdateLevelText();
        UpdateScore(0);
        ResetPanels();
        UpdateTimerVisibility();
        UpdateNextLevelButtonLabel();
    }

    private void ResetPanels()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }

    private void UpdateTimerVisibility()
    {
        if (timerText == null)
            return;

        LevelConfig currentConfig = _levelConfig != null ? _levelConfig : LevelSession.SelectedConfig;
        bool shouldShowTimer = currentConfig != null && currentConfig.timeLimitSeconds > 0f;
        timerText.gameObject.SetActive(shouldShowTimer);

        if (shouldShowTimer)
            UpdateTimer(currentConfig.timeLimitSeconds);
    }

    private void UpdateNextLevelButtonLabel()
    {
        if (nextLevelButton == null)
            return;

        TextMeshProUGUI label = nextLevelButton.GetComponentInChildren<TextMeshProUGUI>();
        if (label == null)
            return;

        label.text = LevelSession.HasNextLevel() ? "NEXT" : "HOME";
    }

    private void UpdateLevelText()
    {
        if (levelText == null)
            return;

        LevelConfig currentConfig = _levelConfig != null ? _levelConfig : LevelSession.SelectedConfig;
        int levelIndex = currentConfig != null ? currentConfig.levelIndex : LevelSession.CurrentLevelIndex;
        levelText.text = $"LEVEL {levelIndex}";
    }

    private void EnsureLevelTextExists()
    {
        if (levelText != null)
            return;

        TextMeshProUGUI template = timerText != null ? timerText : matchCountText;
        if (template == null)
            return;

        GameObject levelTextObject = new GameObject("LevelText", typeof(RectTransform));
        levelTextObject.transform.SetParent(transform, false);

        RectTransform rectTransform = levelTextObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 1f);
        rectTransform.anchorMax = new Vector2(0.5f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = new Vector2(0f, 350f);
        rectTransform.sizeDelta = new Vector2(280f, 50f);

        levelText = levelTextObject.AddComponent<TextMeshProUGUI>();
        levelText.font = template.font;
        levelText.fontSharedMaterial = template.fontSharedMaterial;
        levelText.fontSize = template.fontSize;
        levelText.fontStyle = template.fontStyle;
        levelText.alignment = TextAlignmentOptions.Center;
        levelText.color = template.color;
        levelText.enableAutoSizing = template.enableAutoSizing;
        levelText.fontSizeMin = template.fontSizeMin;
        levelText.fontSizeMax = template.fontSizeMax;
        levelText.overflowMode = TextOverflowModes.Overflow;
        levelText.raycastTarget = false;
    }

    private static void BindButton(Button button, UnityEngine.Events.UnityAction handler)
    {
        if (button == null)
            return;

        button.onClick.RemoveListener(handler);
        button.onClick.AddListener(handler);
    }

    private static void PlayClickSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayClickSound();
    }

    private void EnsureHudOnTop()
    {
        transform.SetAsLastSibling();
    }
}
