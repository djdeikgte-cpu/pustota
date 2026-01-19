using UnityEngine;
using TapOrDie.Yandex;

namespace TapOrDie.Core
{
    /// <summary>
    /// Initializes the game on startup. Place on a GameObject in the first loaded scene.
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject gameManagerPrefab;
        [SerializeField] private GameObject audioManagerPrefab;
        [SerializeField] private GameObject localizationManagerPrefab;
        [SerializeField] private GameObject yandexAdsManagerPrefab;

        private void Awake()
        {
            InitializeManagers();
        }

        private void Start()
        {
            // Notify Yandex that game is ready (after a brief delay to ensure everything is loaded)
            Invoke(nameof(NotifyGameReady), 0.5f);
        }

        private void InitializeManagers()
        {
            // Initialize GameManager
            if (GameManager.Instance == null && gameManagerPrefab != null)
            {
                Instantiate(gameManagerPrefab);
            }
            else if (GameManager.Instance == null)
            {
                CreateDefaultGameManager();
            }

            // Initialize AudioManager
            if (Audio.AudioManager.Instance == null && audioManagerPrefab != null)
            {
                Instantiate(audioManagerPrefab);
            }
            else if (Audio.AudioManager.Instance == null)
            {
                CreateDefaultAudioManager();
            }

            // Initialize LocalizationManager
            if (Localization.LocalizationManager.Instance == null && localizationManagerPrefab != null)
            {
                Instantiate(localizationManagerPrefab);
            }
            else if (Localization.LocalizationManager.Instance == null)
            {
                CreateDefaultLocalizationManager();
            }

            // Initialize YandexAdsManager
            if (YandexAdsManager.Instance == null && yandexAdsManagerPrefab != null)
            {
                Instantiate(yandexAdsManagerPrefab);
            }
            else if (YandexAdsManager.Instance == null)
            {
                CreateDefaultYandexAdsManager();
            }
        }

        private void CreateDefaultGameManager()
        {
            GameObject go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }

        private void CreateDefaultAudioManager()
        {
            GameObject go = new GameObject("AudioManager");
            go.AddComponent<Audio.AudioManager>();
        }

        private void CreateDefaultLocalizationManager()
        {
            GameObject go = new GameObject("LocalizationManager");
            go.AddComponent<Localization.LocalizationManager>();
        }

        private void CreateDefaultYandexAdsManager()
        {
            GameObject go = new GameObject("YandexAdsManager");
            go.AddComponent<YandexAdsManager>();
        }

        private void NotifyGameReady()
        {
            YandexAdsManager.Instance?.NotifyGameReady();
        }
    }
}
