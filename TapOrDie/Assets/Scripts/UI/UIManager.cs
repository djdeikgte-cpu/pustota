using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TapOrDie.Core;
using TapOrDie.Audio;
using TapOrDie.Yandex;
using TapOrDie.Localization;

namespace TapOrDie.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button musicToggleButton;
        [SerializeField] private Image musicIcon;
        [SerializeField] private Sprite musicOnSprite;
        [SerializeField] private Sprite musicOffSprite;

        [Header("Pause Panel")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button pauseMenuButton;
        [SerializeField] private Button pauseRestartButton;

        [Header("Game Over Panel")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI finalHighScoreText;
        [SerializeField] private TextMeshProUGUI newHighScoreText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button rewardedAdButton;
        [SerializeField] private TextMeshProUGUI rewardedAdText;

        [Header("Main Menu Elements")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private Button playButton;
        [SerializeField] private Button menuMusicToggleButton;
        [SerializeField] private TextMeshProUGUI menuHighScoreText;

        [Header("Tutorial")]
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private TextMeshProUGUI tutorialText;

        [Header("Speed Warning")]
        [SerializeField] private GameObject speedWarningPanel;
        [SerializeField] private TextMeshProUGUI speedWarningText;

        [Header("Animation")]
        [SerializeField] private float panelFadeTime = 0.3f;

        private bool hasUsedRewardedAd;
        private CanvasGroup gameOverCanvasGroup;
        private CanvasGroup pauseCanvasGroup;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            SetupCanvasGroups();
            SetupButtons();
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStart += OnGameStart;
                GameManager.Instance.OnGameOver += OnGameOver;
                GameManager.Instance.OnGamePause += OnGamePause;
                GameManager.Instance.OnGameResume += OnGameResume;
                GameManager.Instance.OnScoreChanged += OnScoreChanged;
                GameManager.Instance.OnSpeedIncreased += OnSpeedIncreased;
            }

            UpdateMusicButtonVisual();
            UpdateHighScoreDisplay();
            HideAllPanels();

            // Show main menu if in MainMenu scene
            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
                YandexAdsManager.Instance?.ShowBanner();
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStart -= OnGameStart;
                GameManager.Instance.OnGameOver -= OnGameOver;
                GameManager.Instance.OnGamePause -= OnGamePause;
                GameManager.Instance.OnGameResume -= OnGameResume;
                GameManager.Instance.OnScoreChanged -= OnScoreChanged;
                GameManager.Instance.OnSpeedIncreased -= OnSpeedIncreased;
            }
        }

        private void SetupCanvasGroups()
        {
            if (gameOverPanel != null)
            {
                gameOverCanvasGroup = gameOverPanel.GetComponent<CanvasGroup>();
                if (gameOverCanvasGroup == null)
                    gameOverCanvasGroup = gameOverPanel.AddComponent<CanvasGroup>();
            }

            if (pausePanel != null)
            {
                pauseCanvasGroup = pausePanel.GetComponent<CanvasGroup>();
                if (pauseCanvasGroup == null)
                    pauseCanvasGroup = pausePanel.AddComponent<CanvasGroup>();
            }
        }

        private void SetupButtons()
        {
            // Pause button
            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseClicked);

            // Music toggle
            if (musicToggleButton != null)
                musicToggleButton.onClick.AddListener(OnMusicToggleClicked);

            if (menuMusicToggleButton != null)
                menuMusicToggleButton.onClick.AddListener(OnMusicToggleClicked);

            // Pause panel buttons
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);

            if (pauseMenuButton != null)
                pauseMenuButton.onClick.AddListener(OnMenuClicked);

            if (pauseRestartButton != null)
                pauseRestartButton.onClick.AddListener(OnRestartClicked);

            // Game over buttons
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);

            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClicked);

            if (rewardedAdButton != null)
                rewardedAdButton.onClick.AddListener(OnRewardedAdClicked);

            // Main menu buttons
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayClicked);
        }

        private void HideAllPanels()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            if (pausePanel != null)
                pausePanel.SetActive(false);

            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);

            if (speedWarningPanel != null)
                speedWarningPanel.SetActive(false);
        }

        #region Event Handlers

        private void OnGameStart()
        {
            hasUsedRewardedAd = false;
            HideAllPanels();

            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);

            UpdateScoreDisplay(0);

            // Show tutorial briefly on first play
            if (!PlayerPrefs.HasKey("TapOrDie_TutorialShown"))
            {
                ShowTutorial();
                PlayerPrefs.SetInt("TapOrDie_TutorialShown", 1);
                PlayerPrefs.Save();
            }

            // Hide banner during gameplay
            YandexAdsManager.Instance?.HideBanner();
        }

        private void OnGameOver()
        {
            ShowGameOverPanel();
        }

        private void OnGamePause()
        {
            ShowPausePanel();
        }

        private void OnGameResume()
        {
            HidePausePanel();
        }

        private void OnScoreChanged(int score)
        {
            UpdateScoreDisplay(score);
        }

        private void OnSpeedIncreased()
        {
            ShowSpeedWarning();
        }

        #endregion

        #region UI Updates

        private void UpdateScoreDisplay(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = LocalizationManager.Instance != null ?
                    $"{LocalizationManager.Instance.GetText("score")}: {score}" :
                    $"Score: {score}";
            }
        }

        private void UpdateHighScoreDisplay()
        {
            int highScore = GameManager.Instance?.HighScore ?? 0;

            if (highScoreText != null)
            {
                highScoreText.text = LocalizationManager.Instance != null ?
                    $"{LocalizationManager.Instance.GetText("high_score")}: {highScore}" :
                    $"Best: {highScore}";
            }

            if (menuHighScoreText != null)
            {
                menuHighScoreText.text = LocalizationManager.Instance != null ?
                    $"{LocalizationManager.Instance.GetText("high_score")}: {highScore}" :
                    $"Best: {highScore}";
            }
        }

        private void UpdateMusicButtonVisual()
        {
            bool isMusicOn = AudioManager.Instance?.IsMusicEnabled ?? true;

            if (musicIcon != null)
            {
                if (musicOnSprite != null && musicOffSprite != null)
                {
                    musicIcon.sprite = isMusicOn ? musicOnSprite : musicOffSprite;
                }
                else
                {
                    musicIcon.color = isMusicOn ? Color.white : Color.gray;
                }
            }
        }

        #endregion

        #region Panel Management

        private void ShowGameOverPanel()
        {
            if (gameOverPanel == null) return;

            int finalScore = GameManager.Instance?.CurrentScore ?? 0;
            int highScore = GameManager.Instance?.HighScore ?? 0;

            if (finalScoreText != null)
            {
                finalScoreText.text = LocalizationManager.Instance != null ?
                    $"{LocalizationManager.Instance.GetText("score")}: {finalScore}" :
                    $"Score: {finalScore}";
            }

            if (finalHighScoreText != null)
            {
                finalHighScoreText.text = LocalizationManager.Instance != null ?
                    $"{LocalizationManager.Instance.GetText("high_score")}: {highScore}" :
                    $"Best: {highScore}";
            }

            // Show new high score indicator
            if (newHighScoreText != null)
            {
                newHighScoreText.gameObject.SetActive(finalScore >= highScore && finalScore > 0);
            }

            // Setup rewarded ad button
            if (rewardedAdButton != null)
            {
                rewardedAdButton.gameObject.SetActive(!hasUsedRewardedAd);
                if (rewardedAdText != null)
                {
                    rewardedAdText.text = LocalizationManager.Instance != null ?
                        LocalizationManager.Instance.GetText("watch_ad_continue") :
                        "Watch Ad to Continue";
                }
            }

            gameOverPanel.SetActive(true);

            if (gameOverCanvasGroup != null)
            {
                StartCoroutine(FadeInPanel(gameOverCanvasGroup));
            }

            // Show banner on game over
            YandexAdsManager.Instance?.ShowBanner();

            // Show fullscreen ad after delay
            StartCoroutine(ShowFullscreenAdDelayed());
        }

        private System.Collections.IEnumerator ShowFullscreenAdDelayed()
        {
            yield return new WaitForSecondsRealtime(1f);
            YandexAdsManager.Instance?.ShowFullscreenAd();
        }

        private void HideGameOverPanel()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
        }

        private void ShowPausePanel()
        {
            if (pausePanel == null) return;

            pausePanel.SetActive(true);

            if (pauseCanvasGroup != null)
            {
                StartCoroutine(FadeInPanel(pauseCanvasGroup));
            }

            // Show banner when paused
            YandexAdsManager.Instance?.ShowBanner();
        }

        private void HidePausePanel()
        {
            if (pausePanel != null)
                pausePanel.SetActive(false);

            // Hide banner when resuming
            YandexAdsManager.Instance?.HideBanner();
        }

        private void ShowTutorial()
        {
            if (tutorialPanel == null) return;

            if (tutorialText != null)
            {
                tutorialText.text = LocalizationManager.Instance != null ?
                    LocalizationManager.Instance.GetText("tutorial") :
                    "Tap to Jump!\nAvoid Obstacles!";
            }

            tutorialPanel.SetActive(true);
            StartCoroutine(HideTutorialAfterDelay());
        }

        private System.Collections.IEnumerator HideTutorialAfterDelay()
        {
            yield return new WaitForSeconds(3f);
            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);
        }

        private void ShowSpeedWarning()
        {
            if (speedWarningPanel == null) return;

            if (speedWarningText != null)
            {
                speedWarningText.text = LocalizationManager.Instance != null ?
                    LocalizationManager.Instance.GetText("speed_up") :
                    "Speed Up!";
            }

            speedWarningPanel.SetActive(true);
            StartCoroutine(HideSpeedWarningAfterDelay());
        }

        private System.Collections.IEnumerator HideSpeedWarningAfterDelay()
        {
            yield return new WaitForSeconds(1.5f);
            if (speedWarningPanel != null)
                speedWarningPanel.SetActive(false);
        }

        private System.Collections.IEnumerator FadeInPanel(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < panelFadeTime)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / panelFadeTime);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        #endregion

        #region Button Callbacks

        private void OnPlayClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            YandexAdsManager.Instance?.HideBanner();
            GameManager.Instance?.LoadGameScene();
        }

        private void OnPauseClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            GameManager.Instance?.TogglePause();
        }

        private void OnResumeClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            GameManager.Instance?.ResumeGame();
        }

        private void OnRestartClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            HideGameOverPanel();
            HidePausePanel();
            GameManager.Instance?.RestartGame();
        }

        private void OnMenuClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            GameManager.Instance?.LoadMainMenu();
        }

        private void OnMusicToggleClicked()
        {
            AudioManager.Instance?.ToggleMusic();
            UpdateMusicButtonVisual();
        }

        private void OnRewardedAdClicked()
        {
            AudioManager.Instance?.PlayButtonSound();

            YandexAdsManager.Instance?.ShowRewardedAd(() =>
            {
                // Reward callback - grant extra life
                hasUsedRewardedAd = true;
                GameManager.Instance?.GrantExtraLife();
                HideGameOverPanel();

                // Resume game with extra life
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.UseExtraLife();
                }
            });
        }

        #endregion

        #region Public Methods

        public void ShowMessage(string message, float duration = 2f)
        {
            // Could implement a toast/message system here
            Debug.Log($"UI Message: {message}");
        }

        public void RefreshUI()
        {
            UpdateHighScoreDisplay();
            UpdateMusicButtonVisual();
        }

        #endregion
    }
}
