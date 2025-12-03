using System;
using Animations;
using UnityEngine;

namespace Enemies.Base
{
    /// <summary>
    /// Base class for all enemies in the game.  
    /// Provides common functionality such as death handling, shell collision response,
    /// and component management. All enemy types should inherit from this class.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class Enemy : MonoBehaviour
    {
        #region Events

        /// <summary>
        /// Invoked when any enemy is hit by a shell.
        /// </summary>
        public static event Action OnShellHit;

        /// <summary>
        /// Invoked when any enemy is hit (flattened, stomped, etc.).
        /// </summary>
        public static event Action OnHit;

        #endregion

        #region Inspector Fields

        [Header("Common Enemy Settings")]
        [Tooltip("Delay in seconds before the enemy object is destroyed after death.")]
        public float deathDelay = 3f;

        #endregion

        #region Protected Fields

        protected AnimatedSprite animatedSprite;
        protected DeathAnimation deathAnimation;
        protected EntityMovement movement;
        protected SpriteRenderer spriteRenderer;

        #endregion

        #region Unity Lifecycle

        /// <summary>
        /// Initializes component references required by all enemies.
        /// </summary>
        protected virtual void Awake()
        {
            CacheComponents();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Caches all required components on this enemy GameObject.
        /// </summary>
        protected virtual void CacheComponents()
        {
            animatedSprite = GetComponent<AnimatedSprite>();
            deathAnimation = GetComponent<DeathAnimation>();
            movement = GetComponent<EntityMovement>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Optional safety warning to catch setup issues early
            if (spriteRenderer == null)
            {
                Debug.LogWarning($"[{nameof(Enemy)}] Missing SpriteRenderer on {gameObject.name}");
            }
        }

        #endregion

        #region Damage & Death Logic

        /// <summary>
        /// Called when the enemy takes a hit (e.g., from the player or a shell).  
        /// Disables components, triggers death animation, and schedules destruction.
        /// </summary>
        public virtual void Hit()
        {
            OnHit?.Invoke();

            DisableComponents();

            if (deathAnimation != null)
            {
                deathAnimation.enabled = true;
            }

            Destroy(gameObject, deathDelay);
        }

        /// <summary>
        /// Disables common components such as animation and movement before death.
        /// </summary>
        protected virtual void DisableComponents()
        {
            if (animatedSprite != null)
                animatedSprite.enabled = false;

            if (movement != null)
                movement.enabled = false;
        }

        #endregion

        #region Collision Handling

        /// <summary>
        /// Automatically handles shell collisions for all enemies.  
        /// If a GameObject on the "Shell" layer collides with this enemy → it dies.
        /// </summary>
        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Shell"))
            {
                OnShellHit?.Invoke();
                Hit();
            }
        }

        /// <summary>
        /// Must be implemented by derived enemy classes to define how they interact with the player.
        /// </summary>
        /// <param name="player">The player that collided with this enemy.</param>
        /// <param name="collision">The collision data associated with the contact.</param>
        protected abstract void OnPlayerCollision(Player.PlayerController player, Collision2D collision);

        #endregion
    }
}
