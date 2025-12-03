namespace Blocks
{
    using Enemies.Base;
    using UnityEngine;

    /// <summary>
    /// Handles the block-breaking visual effect by launching four debris pieces outward.
    /// Can damage enemies if they collide with the fragments.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class BlockBreakEffect : MonoBehaviour
    {
        #region Inspector Fields

        [Header("Fragment References")]
        [Tooltip("Rigidbody2D for the top-left fragment.")]
        public Rigidbody2D topLeft;

        [Tooltip("Rigidbody2D for the top-right fragment.")]
        public Rigidbody2D topRight;

        [Tooltip("Rigidbody2D for the bottom-left fragment.")]
        public Rigidbody2D bottomLeft;

        [Tooltip("Rigidbody2D for the bottom-right fragment.")]
        public Rigidbody2D bottomRight;

        [Header("Settings")]
        [Tooltip("Time in seconds before this effect object is destroyed.")]
        public float destroyDelay = 2f;

        [Tooltip("Horizontal launch speed for the debris.")]
        public float horizontalSpeed = 2f;

        [Tooltip("Vertical launch speed for the debris.")]
        public float verticalSpeed = 6f;

        [Tooltip("Angular velocity applied to each debris piece.")]
        public float angularSpeed = 360f;

        #endregion

        #region Unity Lifecycle

        private void OnEnable()
        {
            ApplyInitialForces();
            ScheduleDestruction();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            HandleEnemyCollision(collision);
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Applies initial velocity and angular velocity to each fragment to simulate an explosion.
        /// </summary>
        private void ApplyInitialForces()
        {
            if (topLeft != null)
            {
                topLeft.linearVelocity = new Vector2(-horizontalSpeed, verticalSpeed);
                topLeft.angularVelocity = angularSpeed;
            }

            if (topRight != null)
            {
                topRight.linearVelocity = new Vector2(horizontalSpeed, verticalSpeed);
                topRight.angularVelocity = -angularSpeed;
            }

            if (bottomLeft != null)
            {
                bottomLeft.linearVelocity = new Vector2(-horizontalSpeed * 1.5f, verticalSpeed);
                bottomLeft.angularVelocity = angularSpeed;
            }

            if (bottomRight != null)
            {
                bottomRight.linearVelocity = new Vector2(horizontalSpeed * 1.5f, verticalSpeed);
                bottomRight.angularVelocity = -angularSpeed;
            }
        }

        /// <summary>
        /// Schedules the destruction of this effect object after the specified delay.
        /// </summary>
        private void ScheduleDestruction()
        {
            Destroy(gameObject, destroyDelay);
        }

        #endregion

        #region Collision Handling

        /// <summary>
        /// Handles collision detection with enemies and applies damage if necessary.
        /// </summary>
        /// <param name="collision">The collider currently overlapping the debris collider.</param>
        private void HandleEnemyCollision(Collider2D collision)
        {
            if (collision.gameObject.layer != LayerMask.NameToLayer("Enemy")) return;

            if (collision.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.Hit();

                // Disable this collider after a successful hit to prevent repeated damage.
                var thisCollider = GetComponent<Collider2D>();
                if (thisCollider != null)
                {
                    thisCollider.enabled = false;
                }
            }
        }

        #endregion
    }
}
