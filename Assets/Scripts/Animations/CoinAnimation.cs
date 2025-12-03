namespace Animations
{
    using UnityEngine;

    /// <summary>
    /// Handles a simple color pulsing animation for a coin by oscillating its brightness over time.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class CoinAnimation : MonoBehaviour
    {
        #region Private Fields

        private SpriteRenderer _spriteRenderer;

        #endregion

        #region Unity Lifecycle

        private void Reset()
        {
            // Automatically cache the SpriteRenderer if the component is missing in the Inspector
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_spriteRenderer == null)
            {
                Debug.LogWarning($"[{nameof(CoinAnimation)}] Missing SpriteRenderer on {gameObject.name}.");
                return;
            }

            AnimateCoinColor();
        }

        #endregion

        #region Animation Logic

        /// <summary>
        /// Animates the coin's color by oscillating its brightness using a sine-like PingPong function.
        /// </summary>
        private void AnimateCoinColor()
        {
            // Animation parameters
            const float speed = 2f;             // Speed multiplier for the pulsing effect
            const float maxBrightness = 1f;     // Maximum brightness (HSV value)
            const float minBrightness = 0.6f;   // Minimum brightness (HSV value)
            const float hue = 0.13f;            // Fixed hue for gold/yellow color
            const float saturation = 1f;        // Saturation level (always full color)

            // Compute the brightness oscillation value (v) between max and min
            float t = Mathf.PingPong(Time.time * speed, 1f);
            float v = Mathf.Lerp(maxBrightness, minBrightness, t);

            // Apply the computed color to the sprite
            _spriteRenderer.color = Color.HSVToRGB(hue, saturation, v);
        }

        #endregion
    }
}
