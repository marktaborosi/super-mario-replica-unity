namespace Blocks
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Handles the behavior of a coin spawned from a block.
    /// It increments the player's coin count and plays a simple pop-up animation before destroying itself.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class BlockCoin : MonoBehaviour
    {
        #region Unity Lifecycle

        /// <summary>
        /// Called when the coin is first spawned. 
        /// Increases the player's coin count and starts the pop animation.
        /// </summary>
        public void Start()
        {
            // Safely attempt to add a coin if the GameManager exists
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoin();
            }
            else
            {
                Debug.LogWarning($"[{nameof(BlockCoin)}] GameManager instance not found. Coin count was not incremented.");
            }

            // Start the pop animation coroutine
            StartCoroutine(Animate());
        }

        #endregion

        #region Animation Logic

        /// <summary>
        /// Handles the pop-up and return animation of the coin before destroying the GameObject.
        /// </summary>
        private IEnumerator Animate()
        {
            var restingPosition = transform.localPosition;
            var animatedPosition = restingPosition + Vector3.up * 2f;

            yield return Move(restingPosition, animatedPosition);
            yield return Move(animatedPosition, restingPosition);

            // Destroy the coin object after the animation is finished
            Destroy(gameObject);
        }

        /// <summary>
        /// Smoothly interpolates the coin's position from one point to another over time.
        /// </summary>
        /// <param name="from">Starting position.</param>
        /// <param name="to">Target position.</param>
        private IEnumerator Move(Vector3 from, Vector3 to)
        {
            const float duration = 0.15f;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                transform.localPosition = Vector3.Lerp(from, to, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = to;
        }

        #endregion
    }
}
