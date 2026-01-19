using UnityEngine;
using System.Collections.Generic;
using TapOrDie.Core;

namespace TapOrDie.Obstacles
{
    public class ObstacleManager : MonoBehaviour
    {
        [Header("Obstacle Settings")]
        [SerializeField] private GameObject[] obstaclePrefabs;
        [SerializeField] private int poolSizePerType = 10;
        [SerializeField] private float spawnInterval = 2f;
        [SerializeField] private float minSpawnInterval = 0.8f;
        [SerializeField] private float spawnIntervalDecrease = 0.1f;

        [Header("Spawn Position")]
        [SerializeField] private float spawnXOffset = 15f;
        [SerializeField] private float minSpawnY = -2f;
        [SerializeField] private float maxSpawnY = 2f;
        [SerializeField] private float groundY = -3.5f;

        [Header("Despawn")]
        [SerializeField] private float despawnX = -15f;

        [Header("Obstacle Patterns")]
        [SerializeField] private bool usePatterns = true;
        [SerializeField] private ObstaclePattern[] patterns;

        private Dictionary<int, Queue<GameObject>> obstaclePools;
        private List<GameObject> activeObstacles;
        private float spawnTimer;
        private float currentSpawnInterval;
        private bool isSpawning;

        [System.Serializable]
        public class ObstaclePattern
        {
            public string patternName;
            public Vector2[] offsets;
            public int[] obstacleTypes;
            public float difficulty; // 0-1, higher means harder
        }

        private void Awake()
        {
            InitializePools();
            activeObstacles = new List<GameObject>();
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStart += OnGameStart;
                GameManager.Instance.OnGameOver += OnGameOver;
                GameManager.Instance.OnSpeedIncreased += OnSpeedIncreased;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStart -= OnGameStart;
                GameManager.Instance.OnGameOver -= OnGameOver;
                GameManager.Instance.OnSpeedIncreased -= OnSpeedIncreased;
            }
        }

        private void Update()
        {
            if (!isSpawning || GameManager.Instance == null) return;
            if (!GameManager.Instance.IsPlaying || GameManager.Instance.IsPaused) return;

            MoveObstacles();
            HandleSpawning();
            DespawnObstacles();
        }

        private void InitializePools()
        {
            obstaclePools = new Dictionary<int, Queue<GameObject>>();

            if (obstaclePrefabs == null || obstaclePrefabs.Length == 0)
            {
                Debug.LogWarning("No obstacle prefabs assigned! Creating default obstacle.");
                CreateDefaultObstaclePrefab();
            }

            for (int i = 0; i < obstaclePrefabs.Length; i++)
            {
                obstaclePools[i] = new Queue<GameObject>();

                for (int j = 0; j < poolSizePerType; j++)
                {
                    GameObject obstacle = Instantiate(obstaclePrefabs[i], transform);
                    obstacle.SetActive(false);
                    obstaclePools[i].Enqueue(obstacle);
                }
            }
        }

        private void CreateDefaultObstaclePrefab()
        {
            // Create a simple default obstacle if none assigned
            obstaclePrefabs = new GameObject[1];

            GameObject defaultObstacle = new GameObject("DefaultObstacle");
            defaultObstacle.tag = "Obstacle";

            SpriteRenderer sr = defaultObstacle.AddComponent<SpriteRenderer>();
            sr.sprite = CreateSquareSprite();
            sr.color = Color.red;

            BoxCollider2D col = defaultObstacle.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            defaultObstacle.SetActive(false);
            defaultObstacle.transform.SetParent(transform);

            obstaclePrefabs[0] = defaultObstacle;
        }

        private Sprite CreateSquareSprite()
        {
            Texture2D texture = new Texture2D(32, 32);
            Color[] colors = new Color[32 * 32];
            for (int i = 0; i < colors.Length; i++)
                colors[i] = Color.white;
            texture.SetPixels(colors);
            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f), 32);
        }

        private void HandleSpawning()
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= currentSpawnInterval)
            {
                spawnTimer = 0f;
                SpawnObstacle();
            }
        }

        private void SpawnObstacle()
        {
            if (usePatterns && patterns != null && patterns.Length > 0 && Random.value > 0.5f)
            {
                SpawnPattern();
            }
            else
            {
                SpawnSingleObstacle();
            }
        }

        private void SpawnSingleObstacle()
        {
            int typeIndex = Random.Range(0, obstaclePrefabs.Length);
            GameObject obstacle = GetFromPool(typeIndex);

            if (obstacle != null)
            {
                float yPos = Random.value > 0.7f ?
                    Random.Range(minSpawnY, maxSpawnY) :
                    groundY + 0.5f; // Most obstacles on ground

                obstacle.transform.position = new Vector3(spawnXOffset, yPos, 0);
                obstacle.SetActive(true);
                activeObstacles.Add(obstacle);
            }
        }

        private void SpawnPattern()
        {
            // Select pattern based on current difficulty
            float currentDifficulty = GameManager.Instance != null ?
                Mathf.Clamp01((GameManager.Instance.CurrentSpeed - 5f) / 10f) : 0f;

            List<ObstaclePattern> validPatterns = new List<ObstaclePattern>();
            foreach (var pattern in patterns)
            {
                if (pattern.difficulty <= currentDifficulty + 0.3f)
                    validPatterns.Add(pattern);
            }

            if (validPatterns.Count == 0) return;

            ObstaclePattern selectedPattern = validPatterns[Random.Range(0, validPatterns.Count)];

            for (int i = 0; i < selectedPattern.offsets.Length; i++)
            {
                int typeIndex = i < selectedPattern.obstacleTypes.Length ?
                    selectedPattern.obstacleTypes[i] : 0;
                typeIndex = Mathf.Clamp(typeIndex, 0, obstaclePrefabs.Length - 1);

                GameObject obstacle = GetFromPool(typeIndex);
                if (obstacle != null)
                {
                    Vector3 pos = new Vector3(
                        spawnXOffset + selectedPattern.offsets[i].x,
                        groundY + 0.5f + selectedPattern.offsets[i].y,
                        0
                    );
                    obstacle.transform.position = pos;
                    obstacle.SetActive(true);
                    activeObstacles.Add(obstacle);
                }
            }
        }

        private void MoveObstacles()
        {
            float speed = GameManager.Instance?.CurrentSpeed ?? 5f;
            float movement = speed * Time.deltaTime;

            foreach (var obstacle in activeObstacles)
            {
                if (obstacle.activeInHierarchy)
                {
                    obstacle.transform.position += Vector3.left * movement;
                }
            }
        }

        private void DespawnObstacles()
        {
            for (int i = activeObstacles.Count - 1; i >= 0; i--)
            {
                if (activeObstacles[i].transform.position.x < despawnX)
                {
                    ReturnToPool(activeObstacles[i]);
                    activeObstacles.RemoveAt(i);
                }
            }
        }

        private GameObject GetFromPool(int typeIndex)
        {
            if (!obstaclePools.ContainsKey(typeIndex)) return null;

            Queue<GameObject> pool = obstaclePools[typeIndex];

            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            else
            {
                // Expand pool if needed
                GameObject newObstacle = Instantiate(obstaclePrefabs[typeIndex], transform);
                return newObstacle;
            }
        }

        private void ReturnToPool(GameObject obstacle)
        {
            obstacle.SetActive(false);

            // Find which pool it belongs to
            for (int i = 0; i < obstaclePrefabs.Length; i++)
            {
                if (obstacle.name.Contains(obstaclePrefabs[i].name))
                {
                    obstaclePools[i].Enqueue(obstacle);
                    return;
                }
            }

            // Default to first pool
            if (obstaclePools.ContainsKey(0))
                obstaclePools[0].Enqueue(obstacle);
        }

        private void OnGameStart()
        {
            ClearAllObstacles();
            currentSpawnInterval = spawnInterval;
            spawnTimer = 0f;
            isSpawning = true;
        }

        private void OnGameOver()
        {
            isSpawning = false;
        }

        private void OnSpeedIncreased()
        {
            currentSpawnInterval = Mathf.Max(currentSpawnInterval - spawnIntervalDecrease, minSpawnInterval);
        }

        private void ClearAllObstacles()
        {
            foreach (var obstacle in activeObstacles)
            {
                ReturnToPool(obstacle);
            }
            activeObstacles.Clear();
        }

        public void ResetManager()
        {
            ClearAllObstacles();
            currentSpawnInterval = spawnInterval;
            spawnTimer = 0f;
        }
    }
}
