using System;
using System.Collections;
using Animations;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Handles player state transitions such as:
    /// - Growth and shrinking (power-up / damage)
    /// - Death handling and level reset
    /// - Temporary invincibility (Star Power)
    /// - Short invulnerability after shrinking
    /// 
    /// This script also raises global events for external systems (e.g. audio, UI).
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        #region Inspector Fields

        [Header("Sprite Renderers in Mario prefab")]
        [Tooltip("Renderer used when the player is in small form.")]
        public PlayerSpriteRenderer smallRenderer;

        [Tooltip("Renderer used when the player is in big form.")]
        public PlayerSpriteRenderer bigRenderer;

        #endregion

        #region Events

        public static event Action OnGrow;
        public static event Action OnShrink;
        public static event Action OnDeath;
        public static event Action OnInvincible;

        #endregion

        #region Private Fields

        private PlayerSpriteRenderer ActiveRenderer { get; set; }
        private DeathAnimation _deathAnimation;
        private CapsuleCollider2D _capsuleCollider;

        #endregion

        #region Properties

        public bool Big => bigRenderer.enabled;
        public bool Small => smallRenderer.enabled;
        public bool Dead => _deathAnimation.enabled;
        public bool Star { get; private set; }
        public bool ShrinkOrGrowIsAnimating { get; private set; }

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _deathAnimation = GetComponent<DeathAnimation>();
            _capsuleCollider = GetComponent<CapsuleCollider2D>();
            ActiveRenderer = smallRenderer;
            ShrinkOrGrowIsAnimating = false;
        }

        #endregion

        #region Damage & Death

        /// <summary>
        /// Called when the player takes damage.
        /// If the player is big, they shrink. Otherwise, they die.
        /// </summary>
        public void Hit()
        {
            if (Dead || Star) return;

            if (Big)
                Shrink();
            else
                Death();
        }

        /// <summary>
        /// Handles player death:
        /// - Disables renderers
        /// - Enables death animation
        /// - Requests level reset from GameManager
        /// </summary>
        public void Death()
        {
            OnDeath?.Invoke();

            smallRenderer.enabled = false;
            bigRenderer.enabled = false;
            _deathAnimation.enabled = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetLevel(3f);
            }
            else
            {
                Debug.LogWarning("[PlayerController] GameManager.Instance is null — cannot reset level.");
            }
        }

        #endregion

        #region Growth & Shrinking

        /// <summary>
        /// Transitions the player into their big form after collecting a power-up.
        /// </summary>
        public void Grow()
        {
            OnGrow?.Invoke();

            smallRenderer.enabled = false;
            bigRenderer.enabled = true;
            ActiveRenderer = bigRenderer;

            _capsuleCollider.size = new Vector2(0.9f, 2f);
            _capsuleCollider.offset = new Vector2(0f, 0.5f);

            StartCoroutine(ScaleAnimation());
        }

        /// <summary>
        /// Transitions the player into their small form after taking damage.
        /// </summary>
        private void Shrink()
        {
            OnShrink?.Invoke();

            smallRenderer.enabled = true;
            bigRenderer.enabled = false;
            ActiveRenderer = smallRenderer;

            _capsuleCollider.size = new Vector2(0.9f, 1f);
            _capsuleCollider.offset = new Vector2(0f, 0f);

            StartCoroutine(ScaleAnimation());
        }

        #endregion

        #region Star Power

        /// <summary>
        /// Activates temporary invincibility ("Star Power") for a given duration.
        /// </summary>
        /// <param name="duration">How long the player remains invincible (in seconds).</param>
        public void StarPower(float duration = 10f)
        {
            OnInvincible?.Invoke();
            StartCoroutine(StarPowerAnimation(duration));
        }

        #endregion

        #region Coroutines - Growth Animation

        /// <summary>
        /// Plays the flashing grow/shrink animation and temporarily pauses time during the transition.
        /// </summary>
        private IEnumerator ScaleAnimation()
        {
            Time.timeScale = 0f;
            ShrinkOrGrowIsAnimating = true;

            float elapsed = 0f;
            const float duration = 0.5f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;

                // Toggle renderers to create a flicker effect
                if (Time.frameCount % 4 == 0)
                {
                    smallRenderer.enabled = !smallRenderer.enabled;
                    bigRenderer.enabled = !bigRenderer.enabled;
                }

                yield return null;
            }

            // Reset renderers to the correct state
            smallRenderer.enabled = false;
            bigRenderer.enabled = false;
            ActiveRenderer.enabled = true;

            Time.timeScale = 1f;

            // If we shrank, trigger short invulnerability
            if (ActiveRenderer == smallRenderer)
            {
                StartCoroutine(PlayerIsUnkillable());
            }

            ShrinkOrGrowIsAnimating = false;
        }

        #endregion

        #region Coroutines - Temporary Invulnerability

        /// <summary>
        /// Temporarily disables collisions between the player and enemies.
        /// </summary>
        private IEnumerator PlayerIsUnkillable()
        {
            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"), 
                LayerMask.NameToLayer("Enemy"), 
                true
            );

            yield return PlayerUnkillableAnimation();

            Physics2D.IgnoreLayerCollision(
                LayerMask.NameToLayer("Player"), 
                LayerMask.NameToLayer("Enemy"), 
                false
            );
        }

        /// <summary>
        /// Plays the flashing effect while the player is invulnerable after shrinking.
        /// </summary>
        private IEnumerator PlayerUnkillableAnimation()
        {
            float elapsed = 0f;
            const float duration = 1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                if (Time.frameCount % 4 == 0)
                {
                    ActiveRenderer.enabled = !ActiveRenderer.enabled;
                }

                yield return null;
            }

            ActiveRenderer.enabled = true;
        }

        #endregion

        #region Coroutines - Star Power

        /// <summary>
        /// Handles star power timing and rainbow color effect.
        /// </summary>
        private IEnumerator StarPowerAnimation(float duration = 10f)
        {
            Star = true;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                // Cycle through random colors for visual feedback
                if (Time.frameCount % 4 == 0)
                {
                    ActiveRenderer.SpriteRenderer.color = UnityEngine.Random.ColorHSV(
                        0f, 1f, 
                        1f, 1f, 
                        1f, 1f
                    );
                }

                yield return null;
            }

            // Reset color and state
            ActiveRenderer.SpriteRenderer.color = Color.white;
            Star = false;
        }

        #endregion
    }
}
