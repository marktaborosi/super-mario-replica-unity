namespace Animations
{
    using System.Collections;
    using Enemies.Base;
    using UnityEngine;

    /// <summary>
    /// Handles the death animation for both player and enemy objects.
    /// Disables physics, swaps the sprite, and performs a jump-and-fall death motion.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class DeathAnimation : MonoBehaviour
    {
        #region Inspector Fields

        [Header("Death Animation Settings")]
        [Tooltip("SpriteRenderer component used for rendering the death sprite.")]
        public SpriteRenderer spriteRenderer;

        [Tooltip("Optional sprite to display when the object dies.")]
        public Sprite deathSprite;

        #endregion

        #region Private Fields

        private bool _isEnemy;

        #endregion

        #region Unity Lifecycle

        private void Reset()
        {
            // Automatically assign the SpriteRenderer if missing in the inspector
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            DetectIfEnemy();
            UpdateSprite();
            DisablePhysics();
            StartCoroutine(Animate());
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Detects if this object represents an enemy by checking for an <see cref="Enemy"/> component.
        /// </summary>
        private void DetectIfEnemy()
        {
            _isEnemy = TryGetComponent<Enemy>(out _);
        }

        /// <summary>
        /// Updates the sprite renderer to show the death sprite and ensures correct rendering order.
        /// </summary>
        private void UpdateSprite()
        {
            if (spriteRenderer == null)
            {
                Debug.LogWarning($"[{nameof(DeathAnimation)}] Missing SpriteRenderer on {gameObject.name}");
                return;
            }

            spriteRenderer.enabled = true;
            spriteRenderer.sortingOrder = 10;

            if (deathSprite != null)
            {
                spriteRenderer.sprite = deathSprite;
            }
        }

        #endregion

        #region Physics Control

        /// <summary>
        /// Disables physics and movement components to allow for the death animation to play.
        /// </summary>
        private void DisablePhysics()
        {
            // Disable all 2D colliders
            var colliders = GetComponents<Collider2D>();
            foreach (var currentCollider in colliders)
            {
                currentCollider.enabled = false;
            }

            // Switch the Rigidbody to kinematic so gravity does not affect it
            if (TryGetComponent<Rigidbody2D>(out var rb))
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            // Disable movement scripts if they exist
            if (TryGetComponent<Player.PlayerMovement>(out var playerMovement))
            {
                playerMovement.enabled = false;
            }

            if (TryGetComponent<EntityMovement>(out var entityMovement))
            {
                entityMovement.enabled = false;
            }
        }

        #endregion

        #region Death Animation

        /// <summary>
        /// Performs a jump-and-fall animation to simulate a death arc.
        /// </summary>
        private IEnumerator Animate()
        {
            // If this is the player, wait briefly before starting the animation
            if (CompareTag("Player"))
            {
                yield return new WaitForSeconds(0.5f);
            }

            // If the object is an enemy, rotate it upside-down
            if (_isEnemy)
            {
                transform.eulerAngles = new Vector3(0, 0, 180f);
            }

            // Animation parameters
            const float duration = 3f;
            const float jumpVelocity = 10f;
            const float gravity = -36f;

            var elapsed = 0f;
            var velocity = Vector3.up * jumpVelocity;

            // Simulate parabolic jump motion
            while (elapsed < duration)
            {
                transform.position += velocity * Time.deltaTime;
                velocity.y += gravity * Time.deltaTime;
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        #endregion
    }
}
