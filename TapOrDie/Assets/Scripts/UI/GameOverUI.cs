using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TapOrDie.Core;
using TapOrDie.Audio;
using TapOrDie.Yandex;
using TapOrDie.Localization;

namespace TapOrDie.UI
{
    /// <summary>
    /// Game over screen UI handler
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI gameOverTitle;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI newRecordText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button rewardedAdButton;
        [SerializeField] private TextMeshProUGUI rewardedAdButtonText;

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private CanvasGroup canvasGroup;

        private bool hasUsedRewardedAd;

        private void Start()
        {
            SetupButtons();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver += ShowGameOver;
            }

            // Initially hidden
            if (canvasGroup != null)
                canvasGroup.alpha = 0f;

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameOver -= ShowGameOver;
            }
        }

        private void SetupButtons()
        {
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);

            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClicked);

            if (rewardedAdButton != null)
                rewardedAdButton.onClick.AddListener(OnRewardedAdClicked);
        }

        public void ShowGameOver()
        {
            gameObject.SetActive(true);
            UpdateUI();
            StartCoroutine(FadeIn());

            // Show banner
            YandexAdsManager.Instance?.ShowBanner();

            // Play game over sound
            AudioManager.Instance?.PlayGameOverSound();

            // Show fullscreen ad after delay
            StartCoroutine(ShowFullscreenAdDelayed());
        }

        private System.Collections.IEnumerator ShowFullscreenAdDelayed()
        {
            yield return new WaitForSecondsRealtime(1.5f);
            YandexAdsManager.Instance?.ShowFullscreenAd();
        }

        private void UpdateUI()
        {
            int score = GameManager.Instance?.CurrentScore ?? 0;
            int highScore = GameManager.Instance?.HighScore ?? 0;
            bool isNewRecord = score >= highScore && score > 0;

            // Localized text
            if (gameOverTitle != null)
                gameOverTitle.text = LocalizationManager.Instance?.GetText("game_over") ?? "Game Over";

            if (scoreText != null)
            {
                string label = LocalizationManager.Instance?.GetText("score") ?? "Score";
                scoreText.text = $"{label}: {score}";
            }

            if (highScoreText != null)
            {
                string label = LocalizationManager.Instance?.GetText("high_score") ?? "Best";
                highScoreText.text = $"{label}: {highScore}";
            }

            if (newRecordText != null)
            {
                newRecordText.gameObject.SetActive(isNewRecord);
                if (isNewRecord)
                    newRecordText.text = LocalizationManager.Instance?.GetText("new_record") ?? "New Record!";
            }

            // Rewarded ad button
            if (rewardedAdButton != null)
            {
                rewardedAdButton.gameObject.SetActive(!hasUsedRewardedAd);

                if (rewardedAdButtonText != null)
                    rewardedAdButtonText.text = LocalizationManager.Instance?.GetText("watch_ad_continue") ?? "Watch Ad";
            }

            // Localize buttons
            var restartText = restartButton?.GetComponentInChildren<TextMeshProUGUI>();
            if (restartText != null)
                restartText.text = LocalizationManager.Instance?.GetText("restart") ?? "Restart";

            var menuText = menuButton?.GetComponentInChildren<TextMeshProUGUI>();
            if (menuText != null)
                menuText.text = LocalizationManager.Instance?.GetText("menu") ?? "Menu";
        }

        private System.Collections.IEnumerator FadeIn()
        {
            if (canvasGroup == null) yield break;

            canvasGroup.alpha = 0f;
            float elapsed = 0f;

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeInDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnRestartClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            Hide();
            GameManager.Instance?.RestartGame();
        }

        private void OnMenuClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            Hide();
            GameManager.Instance?.LoadMainMenu();
        }

        private void OnRewardedAdClicked()
        {
            AudioManager.Instance?.PlayButtonSound();

            YandexAdsManager.Instance?.ShowRewardedAd(() =>
            {
                // Grant extra life
                hasUsedRewardedAd = true;
                GameManager.Instance?.GrantExtraLife();
                Hide();

                // Resume with extra life
                GameManager.Instance?.UseExtraLife();
            });
        }

        public void ResetRewardedAdUsage()
        {
            hasUsedRewardedAd = false;
        }
    }
}
