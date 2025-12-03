using System;
using Enemies.Base;
using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Represents a Koopa enemy with two primary states:
    /// - Walking: Default state where Koopa patrols and damages the player on contact.
    /// - Shelled: After being stomped, Koopa hides in its shell and can be pushed as a projectile.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(EntityMovement))]
    [RequireComponent(typeof(Collider2D))]
    public class Koopa : Enemy
    {
        #region Events

        /// <summary>
        /// Fired when Koopa first enters its shell after being stomped.
        /// </summary>
        public static event Action OnShellEnter;

        /// <summary>
        /// Fired when the shell is kicked and begins moving.
        /// </summary>
        public static event Action OnShellKick;

        #endregion

        #region Inspector Fields

        [Header("Koopa Settings")]
        [Tooltip("Sprite to use when Koopa is inside its shell.")]
        public Sprite shellSprite;

        [Tooltip("Movement speed of the shell when pushed.")]
        public float shellSpeed = 12f;

        #endregion

        #region Private State

        private bool _shelled;     // True if Koopa has entered its shell (after being stomped)
        private bool _shellPushed; // True if the shell has been kicked and is moving

        #endregion

        #region Collision Logic

        /// <summary>
        /// Handles all collision interactions with the player.
        /// Determines the correct behavior based on Koopa's state and the direction of the collision.
        /// </summary>
        /// <param name="player">The player character that collided with Koopa.</param>
        /// <param name="collision">The collision data from Unity's physics system.</param>
        protected override void OnPlayerCollision(Player.PlayerController player, Collision2D collision)
        {
            // 🟢 CASE 1: Koopa is walking
            if (!_shelled)
            {
                if (player.Star)
                {
                    // Player is invincible → Koopa dies immediately
                    Hit();
                    return;
                }

                if (collision.transform.IsInDirectionDotTest(transform, Vector2.down))
                {
                    // Player stomped Koopa from the above → Enter shell state
                    EnterShell();
                    return;
                }

                // Player hit Koopa from the side → Damage the player
                player.Hit();
                return;
            }

            // 🟡 CASE 2: Koopa is already in shell state
            if (!_shellPushed)
            {
                // Shell is stationary → Push it away from the player
                var direction = new Vector2(transform.position.x - player.transform.position.x, 0f);
                PushShell(direction);
            }
            else
            {
                // Shell is moving → It damages the player or dies if the player is invincible
                if (player.Star)
                {
                    Hit();
                }
                else
                {
                    player.Hit();
                }
            }
        }

        /// <summary>
        /// Unity callback for 2D collision events.  
        /// Delegates collision handling to <see cref="OnPlayerCollision"/> if the collider is a player.
        /// </summary>
        /// <param name="collision">The collision data provided by Unity.</param>
        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);

            // Ignore non-player collisions
            if (!collision.gameObject.CompareTag("Player"))
                return;

            // Attempt to retrieve the Player component and process collision
            var player = collision.gameObject.GetComponent<Player.PlayerController>();
            if (player != null)
            {
                OnPlayerCollision(player, collision);
            }
        }

        #endregion

        #region State Logic

        /// <summary>
        /// Transitions Koopa into its shell state after being stomped.
        /// Disables movement, updates the sprite, and keeps the collider active for shell interactions.
        /// </summary>
        private void EnterShell()
        {
            // Prevent multiple state transitions
            if (_shelled) return;

            _shelled = true;
            OnShellEnter?.Invoke();

            // Disable base movement and animations
            DisableComponents();

            // Update to shell sprite
            if (spriteRenderer != null && shellSprite != null)
            {
                spriteRenderer.sprite = shellSprite;
            }

            // Keep collider active so shell can interact with environment and player
            var thisCollider = GetComponent<Collider2D>();
            if (thisCollider != null)
            {
                thisCollider.enabled = true;
            }
        }

        /// <summary>
        /// Pushes the Koopa shell in a given direction and turns it into a projectile hazard.
        /// Updates the GameObject's layer so other enemies can detect shell collisions.
        /// </summary>
        /// <param name="direction">The horizontal direction to push the shell (away from the player).</param>
        private void PushShell(Vector2 direction)
        {
            // Prevent double-push
            if (_shellPushed) return;

            _shellPushed = true;
            OnShellKick?.Invoke();

            if (movement != null)
            {
                movement.direction = direction.normalized;
                movement.speed = shellSpeed;
                movement.enabled = true;
            }
            else
            {
                Debug.LogWarning($"[{nameof(Koopa)}] No EntityMovement component found on {name}");
            }

            // Update layer for enemy collision detection
            gameObject.layer = LayerMask.NameToLayer("Shell");
        }

        #endregion
    }
}
