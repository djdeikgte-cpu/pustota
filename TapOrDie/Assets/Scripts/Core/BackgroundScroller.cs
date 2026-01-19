using UnityEngine;

namespace TapOrDie.Core
{
    /// <summary>
    /// Scrolls background elements for parallax effect
    /// </summary>
    public class BackgroundScroller : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float parallaxMultiplier = 0.5f;
        [SerializeField] private float resetPositionX = -20f;
        [SerializeField] private float startPositionX = 20f;
        [SerializeField] private bool useGameSpeed = true;
        [SerializeField] private float customSpeed = 2f;

        private Vector3 startPosition;

        private void Start()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            if (GameManager.Instance == null) return;
            if (!GameManager.Instance.IsPlaying || GameManager.Instance.IsPaused) return;

            float speed = useGameSpeed ?
                GameManager.Instance.CurrentSpeed * parallaxMultiplier :
                customSpeed;

            transform.position += Vector3.left * speed * Time.deltaTime;

            // Reset position for infinite scrolling
            if (transform.position.x <= resetPositionX)
            {
                transform.position = new Vector3(
                    startPositionX,
                    transform.position.y,
                    transform.position.z
                );
            }
        }

        public void ResetPosition()
        {
            transform.position = startPosition;
        }
    }
}
