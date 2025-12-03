using System;
using Enemies.Base;

namespace Enemies
{
    using UnityEngine;

    /// <summary>
    /// Represents a basic Goomba enemy.  
    /// Handles player collisions, flattening behavior, and death from invincibility or shell hits.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(EntityMovement))]
    public class Goomba : Enemy
    {
        #region Events

        /// <summary>
        /// Invoked when a Goomba is flattened by the player.
        /// </summary>
        public static event Action OnGoombaFlatten;

        #endregion

        #region Inspector Fields

        [Header("Goomba Settings")]
        [Tooltip("Sprite displayed when the Goomba has been flattened.")]
        public Sprite flatSprite;

        #endregion

        #region Private Fields

        private bool _flattened; // Tracks if the Goomba was already stomped

        #endregion

        #region Collision Handling

        /// <summary>
        /// Handles collision logic when the player interacts with this Goomba.
        /// </summary>
        /// <param name="player">The player instance that collided with the Goomba.</param>
        /// <param name="collision">The collision information.</param>
        protected override void OnPlayerCollision(Player.PlayerController player, Collision2D collision)
        {
            // 🟢 If the player is invincible (star power), kill the Goomba immediately.
            if (player.Star)
            {
                Hit();
                return;
            }

            // 🟡 If the player landed from above, flatten the Goomba.
            if (collision.transform.IsInDirectionDotTest(transform, Vector2.down))
            {
                Flatten();
                return;
            }

            // 🔴 Otherwise, the player collided from the side — damage the player.
            player.Hit();
        }

        /// <summary>
        /// Unity callback for collision events.
        /// Delegates collision handling to OnPlayerCollision if the collider is a Player.
        /// </summary>
        /// <param name="collision">Collision data for the event.</param>
        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);

            // 🛑 Ignore collisions with non-player objects after flattening.
            if (_flattened && !collision.gameObject.CompareTag("Player"))
                return;

            // ✅ Only process player collisions.
            var player = collision.gameObject.GetComponent<Player.PlayerController>();
            if (player != null)
            {
                OnPlayerCollision(player, collision);
            }
        }

        #endregion

        #region Flatten Logic

        /// <summary>
        /// Handles the behavior when the player stomps the Goomba.
        /// Disables movement and collisions, changes the sprite, and schedules destruction.
        /// </summary>
        private void Flatten()
        {
            // Prevent multiple flatten calls
            if (_flattened) return;

            _flattened = true;
            OnGoombaFlatten?.Invoke();

            // Disable collider to prevent additional collisions after flattening
            var thisCollider = GetComponent<CircleCollider2D>();
            if (thisCollider != null)
            {
                thisCollider.enabled = false;
            }

            // Disable common components such as movement and animation
            DisableComponents();

            // Swap the sprite to the flattened version
            if (spriteRenderer != null && flatSprite != null)
            {
                spriteRenderer.sprite = flatSprite;
            }

            // Destroy the Goomba after a short delay to allow the flattened sprite to display
            Destroy(gameObject, 0.5f);
        }

        #endregion
    }
}
