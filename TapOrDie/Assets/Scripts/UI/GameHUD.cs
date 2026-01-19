using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TapOrDie.Core;
using TapOrDie.Audio;
using TapOrDie.Localization;

namespace TapOrDie.UI
{
    /// <summary>
    /// In-game HUD handler
    /// </summary>
    public class GameHUD : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;

        [Header("Buttons")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button musicButton;
        [SerializeField] private Image musicIcon;
        [SerializeField] private Sprite musicOnSprite;
        [SerializeField] private Sprite musicOffSprite;

        [Header("Effects")]
        [SerializeField] private TextMeshProUGUI speedUpText;
        [SerializeField] private float speedUpDisplayDuration = 1.5f;

        private void Start()
        {
            SetupButtons();
            SubscribeToEvents();
            UpdateUI();

            if (speedUpText != null)
                speedUpText.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SetupButtons()
        {
            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseClicked);

            if (musicButton != null)
                musicButton.onClick.AddListener(OnMusicClicked);
        }

        private void SubscribeToEvents()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreChanged += OnScoreChanged;
                GameManager.Instance.OnSpeedIncreased += OnSpeedIncreased;
                GameManager.Instance.OnGameStart += OnGameStart;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreChanged -= OnScoreChanged;
                GameManager.Instance.OnSpeedIncreased -= OnSpeedIncreased;
                GameManager.Instance.OnGameStart -= OnGameStart;
            }
        }

        private void UpdateUI()
        {
            UpdateScore(GameManager.Instance?.CurrentScore ?? 0);
            UpdateHighScore();
            UpdateMusicButton();
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = score.ToString();
            }
        }

        private void UpdateHighScore()
        {
            if (highScoreText != null)
            {
                int highScore = GameManager.Instance?.HighScore ?? 0;
                string label = LocalizationManager.Instance?.GetText("high_score") ?? "Best";
                highScoreText.text = $"{label}: {highScore}";
            }
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

        #region Event Handlers

        private void OnScoreChanged(int score)
        {
            UpdateScore(score);

            // Score pop animation
            if (scoreText != null)
            {
                StartCoroutine(ScorePopAnimation());
            }
        }

        private System.Collections.IEnumerator ScorePopAnimation()
        {
            Vector3 originalScale = scoreText.transform.localScale;
            scoreText.transform.localScale = originalScale * 1.2f;

            float elapsed = 0f;
            float duration = 0.1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                scoreText.transform.localScale = Vector3.Lerp(originalScale * 1.2f, originalScale, elapsed / duration);
                yield return null;
            }

            scoreText.transform.localScale = originalScale;
        }

        private void OnSpeedIncreased()
        {
            ShowSpeedUpNotification();
            AudioManager.Instance?.PlaySpeedUpSound();
        }

        private void OnGameStart()
        {
            UpdateUI();
        }

        #endregion

        #region Button Handlers

        private void OnPauseClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            GameManager.Instance?.PauseGame();
        }

        private void OnMusicClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            AudioManager.Instance?.ToggleMusic();
            UpdateMusicButton();
        }

        #endregion

        private void ShowSpeedUpNotification()
        {
            if (speedUpText == null) return;

            speedUpText.text = LocalizationManager.Instance?.GetText("speed_up") ?? "Speed Up!";
            speedUpText.gameObject.SetActive(true);

            StartCoroutine(HideSpeedUpNotification());
        }

        private System.Collections.IEnumerator HideSpeedUpNotification()
        {
            yield return new WaitForSeconds(speedUpDisplayDuration);

            if (speedUpText != null)
                speedUpText.gameObject.SetActive(false);
        }
    }
}
