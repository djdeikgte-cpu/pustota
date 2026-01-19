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
    /// Pause menu UI handler
    /// </summary>
    public class PauseUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI pauseTitle;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button musicButton;
        [SerializeField] private Image musicIcon;
        [SerializeField] private Sprite musicOnSprite;
        [SerializeField] private Sprite musicOffSprite;

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private CanvasGroup canvasGroup;

        private void Start()
        {
            SetupButtons();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGamePause += ShowPause;
                GameManager.Instance.OnGameResume += HidePause;
            }

            // Initially hidden
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGamePause -= ShowPause;
                GameManager.Instance.OnGameResume -= HidePause;
            }
        }

        private void SetupButtons()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(OnResumeClicked);

            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);

            if (menuButton != null)
                menuButton.onClick.AddListener(OnMenuClicked);

            if (musicButton != null)
                musicButton.onClick.AddListener(OnMusicClicked);
        }

        public void ShowPause()
        {
            gameObject.SetActive(true);
            UpdateUI();
            StartCoroutine(FadeIn());

            // Show banner when paused
            YandexAdsManager.Instance?.ShowBanner();
        }

        public void HidePause()
        {
            gameObject.SetActive(false);

            // Hide banner when resuming
            YandexAdsManager.Instance?.HideBanner();
        }

        private void UpdateUI()
        {
            // Localized title
            if (pauseTitle != null)
                pauseTitle.text = LocalizationManager.Instance?.GetText("pause") ?? "Paused";

            // Localize buttons
            var resumeText = resumeButton?.GetComponentInChildren<TextMeshProUGUI>();
            if (resumeText != null)
                resumeText.text = LocalizationManager.Instance?.GetText("resume") ?? "Resume";

            var restartText = restartButton?.GetComponentInChildren<TextMeshProUGUI>();
            if (restartText != null)
                restartText.text = LocalizationManager.Instance?.GetText("restart") ?? "Restart";

            var menuText = menuButton?.GetComponentInChildren<TextMeshProUGUI>();
            if (menuText != null)
                menuText.text = LocalizationManager.Instance?.GetText("menu") ?? "Menu";

            // Update music button
            UpdateMusicButton();
        }

        private void UpdateMusicButton()
        {
            if (musicIcon == null) return;

            bool isMusicOn = AudioManager.Instance?.IsMusicEnabled ?? true;

            if (musicOnSprite != null && musicOffSprite != null)
            {
                musicIcon.sprite = isMusicOn ? musicOnSprite : musicOffSprite;
            }
            else
            {
                musicIcon.color = isMusicOn ? Color.white : new Color(1f, 1f, 1f, 0.5f);
            }
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

        #region Button Handlers

        private void OnResumeClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            GameManager.Instance?.ResumeGame();
        }

        private void OnRestartClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            HidePause();
            GameManager.Instance?.RestartGame();
        }

        private void OnMenuClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            HidePause();
            GameManager.Instance?.LoadMainMenu();
        }

        private void OnMusicClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            AudioManager.Instance?.ToggleMusic();
            UpdateMusicButton();
        }

        #endregion
    }
}
