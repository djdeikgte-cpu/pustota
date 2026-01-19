using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace TapOrDie.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private float baseSpeed = 5f;
        [SerializeField] private float speedIncreaseInterval = 10f;
        [SerializeField] private float speedIncreaseAmount = 0.5f;
        [SerializeField] private float maxSpeed = 15f;

        public float CurrentSpeed { get; private set; }
        public int CurrentScore { get; private set; }
        public int HighScore { get; private set; }
        public bool IsPlaying { get; private set; }
        public bool IsPaused { get; private set; }
        public bool HasExtraLife { get; private set; }

        public event Action OnGameStart;
        public event Action OnGameOver;
        public event Action OnGamePause;
        public event Action OnGameResume;
        public event Action<int> OnScoreChanged;
        public event Action OnSpeedIncreased;
        public event Action OnExtraLifeUsed;

        private float gameTime;
        private float scoreTimer;
        private float speedTimer;
        private int lastSpeedIncreaseCount;

        private const string HIGH_SCORE_KEY = "TapOrDie_HighScore";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadHighScore();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "GameScene")
            {
                StartGame();
            }
        }

        private void Update()
        {
            if (!IsPlaying || IsPaused) return;

            gameTime += Time.deltaTime;
            scoreTimer += Time.deltaTime;
            speedTimer += Time.deltaTime;

            // Score increases every second
            if (scoreTimer >= 1f)
            {
                scoreTimer -= 1f;
                AddScore(1);
            }

            // Speed increases every 10 seconds
            int currentSpeedIncreaseCount = Mathf.FloorToInt(gameTime / speedIncreaseInterval);
            if (currentSpeedIncreaseCount > lastSpeedIncreaseCount)
            {
                lastSpeedIncreaseCount = currentSpeedIncreaseCount;
                IncreaseSpeed();
            }
        }

        public void StartGame()
        {
            CurrentScore = 0;
            CurrentSpeed = baseSpeed;
            gameTime = 0f;
            scoreTimer = 0f;
            speedTimer = 0f;
            lastSpeedIncreaseCount = 0;
            HasExtraLife = false;
            IsPlaying = true;
            IsPaused = false;

            Time.timeScale = 1f;
            OnGameStart?.Invoke();
            OnScoreChanged?.Invoke(CurrentScore);
        }

        public void GameOver()
        {
            if (!IsPlaying) return;

            IsPlaying = false;
            Time.timeScale = 0f;

            if (CurrentScore > HighScore)
            {
                HighScore = CurrentScore;
                SaveHighScore();
            }

            OnGameOver?.Invoke();
        }

        public void UseExtraLife()
        {
            if (HasExtraLife)
            {
                HasExtraLife = false;
                IsPlaying = true;
                Time.timeScale = 1f;
                OnExtraLifeUsed?.Invoke();
            }
        }

        public void GrantExtraLife()
        {
            HasExtraLife = true;
        }

        public void PauseGame()
        {
            if (!IsPlaying || IsPaused) return;

            IsPaused = true;
            Time.timeScale = 0f;
            OnGamePause?.Invoke();
        }

        public void ResumeGame()
        {
            if (!IsPaused) return;

            IsPaused = false;
            Time.timeScale = 1f;
            OnGameResume?.Invoke();
        }

        public void TogglePause()
        {
            if (IsPaused)
                ResumeGame();
            else
                PauseGame();
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("GameScene");
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            IsPlaying = false;
            SceneManager.LoadScene("MainMenu");
        }

        public void LoadGameScene()
        {
            SceneManager.LoadScene("GameScene");
        }

        private void AddScore(int points)
        {
            CurrentScore += points;
            OnScoreChanged?.Invoke(CurrentScore);
        }

        public void AddBonusScore(int bonus)
        {
            AddScore(bonus);
        }

        private void IncreaseSpeed()
        {
            CurrentSpeed = Mathf.Min(CurrentSpeed + speedIncreaseAmount, maxSpeed);
            OnSpeedIncreased?.Invoke();
        }

        private void LoadHighScore()
        {
            HighScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        }

        private void SaveHighScore()
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, HighScore);
            PlayerPrefs.Save();
        }

        public void ResetHighScore()
        {
            HighScore = 0;
            SaveHighScore();
        }
    }
}
