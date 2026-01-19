using UnityEngine;

namespace TapOrDie.Core
{
    /// <summary>
    /// Infinite scrolling ground
    /// </summary>
    public class Ground : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float width = 20f;
        [SerializeField] private Transform[] groundSegments;

        private void Update()
        {
            if (GameManager.Instance == null) return;
            if (!GameManager.Instance.IsPlaying || GameManager.Instance.IsPaused) return;

            float speed = GameManager.Instance.CurrentSpeed;

            foreach (var segment in groundSegments)
            {
                if (segment == null) continue;

                segment.position += Vector3.left * speed * Time.deltaTime;

                // Reset segment position for infinite scrolling
                if (segment.position.x <= -width)
                {
                    // Find the rightmost segment
                    float maxX = float.MinValue;
                    foreach (var s in groundSegments)
                    {
                        if (s != null && s.position.x > maxX)
                            maxX = s.position.x;
                    }

                    segment.position = new Vector3(
                        maxX + width,
                        segment.position.y,
                        segment.position.z
                    );
                }
            }
        }

        public void ResetGround()
        {
            if (groundSegments == null) return;

            for (int i = 0; i < groundSegments.Length; i++)
            {
                if (groundSegments[i] != null)
                {
                    groundSegments[i].position = new Vector3(
                        i * width,
                        groundSegments[i].position.y,
                        groundSegments[i].position.z
                    );
                }
            }
        }
    }
}
