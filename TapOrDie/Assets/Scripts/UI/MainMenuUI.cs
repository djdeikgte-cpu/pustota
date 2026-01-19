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
    /// Main menu specific UI handler
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button musicToggleButton;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image musicIcon;
        [SerializeField] private Sprite musicOnSprite;
        [SerializeField] private Sprite musicOffSprite;

        [Header("Animation")]
        [SerializeField] private float titlePulseSpeed = 1f;
        [SerializeField] private float titlePulseAmount = 0.05f;

        private Vector3 titleOriginalScale;

        private void Start()
        {
            SetupButtons();
            UpdateUI();

            if (titleText != null)
                titleOriginalScale = titleText.transform.localScale;

            // Show banner in menu
            YandexAdsManager.Instance?.ShowBanner();

            // Play menu music
            AudioManager.Instance?.PlayMenuMusic();
        }

        private void Update()
        {
            AnimateTitle();
        }

        private void SetupButtons()
        {
            if (playButton != null)
                playButton.onClick.AddListener(OnPlayClicked);

            if (musicToggleButton != null)
                musicToggleButton.onClick.AddListener(OnMusicToggleClicked);
        }

        private void UpdateUI()
        {
            // Update high score
            if (highScoreText != null)
            {
                int highScore = GameManager.Instance?.HighScore ?? PlayerPrefs.GetInt("TapOrDie_HighScore", 0);
                string label = LocalizationManager.Instance?.GetText("high_score") ?? "Best";
                highScoreText.text = $"{label}: {highScore}";
            }

            // Update music button
            UpdateMusicButtonVisual();

            // Update localized title
            if (titleText != null)
            {
                titleText.text = "TAP OR DIE";
            }
        }

        private void UpdateMusicButtonVisual()
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

        private void AnimateTitle()
        {
            if (titleText == null) return;

            float pulse = 1f + Mathf.Sin(Time.time * titlePulseSpeed) * titlePulseAmount;
            titleText.transform.localScale = titleOriginalScale * pulse;
        }

        private void OnPlayClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            YandexAdsManager.Instance?.HideBanner();
            GameManager.Instance?.LoadGameScene();
        }

        private void OnMusicToggleClicked()
        {
            AudioManager.Instance?.PlayButtonSound();
            AudioManager.Instance?.ToggleMusic();
            UpdateMusicButtonVisual();
        }
    }
}
