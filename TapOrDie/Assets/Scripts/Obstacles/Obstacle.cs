using UnityEngine;

namespace TapOrDie.Obstacles
{
    public class Obstacle : MonoBehaviour
    {
        [Header("Obstacle Settings")]
        [SerializeField] private ObstacleType obstacleType = ObstacleType.Static;
        [SerializeField] private float rotationSpeed = 0f;
        [SerializeField] private float bobSpeed = 0f;
        [SerializeField] private float bobAmount = 0f;

        private Vector3 startPosition;
        private float bobTimer;

        public enum ObstacleType
        {
            Static,
            Rotating,
            Bobbing,
            RotatingAndBobbing
        }

        private void OnEnable()
        {
            startPosition = transform.position;
            bobTimer = Random.Range(0f, Mathf.PI * 2f); // Random start phase
        }

        private void Update()
        {
            switch (obstacleType)
            {
                case ObstacleType.Rotating:
                    Rotate();
                    break;
                case ObstacleType.Bobbing:
                    Bob();
                    break;
                case ObstacleType.RotatingAndBobbing:
                    Rotate();
                    Bob();
                    break;
            }
        }

        private void Rotate()
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }

        private void Bob()
        {
            bobTimer += bobSpeed * Time.deltaTime;
            float yOffset = Mathf.Sin(bobTimer) * bobAmount;
            Vector3 currentPos = transform.position;
            // Only modify Y relative to current movement (X changes from ObstacleManager)
            transform.position = new Vector3(currentPos.x, startPosition.y + yOffset, currentPos.z);
        }

        public void ResetObstacle()
        {
            transform.rotation = Quaternion.identity;
            bobTimer = 0f;
        }
    }
}
