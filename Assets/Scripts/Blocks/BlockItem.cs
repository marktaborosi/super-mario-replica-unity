using System;

namespace Blocks
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Handles the behavior of an item emerging from a block.
    /// Disables physics during the spawn animation, raises the item upward, and re-enables it afterward.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class BlockItem : MonoBehaviour
    {
        #region Events

        /// <summary>
        /// Event triggered when a power-up item appears from a block.
        /// </summary>
        public static event Action OnPowerUpAppears;

        #endregion

        #region Private Fields

        private Rigidbody2D _rigidBody;
        private SpriteRenderer _spriteRenderer;
        private CircleCollider2D _physicsCollider;
        private BoxCollider2D _triggerCollider;
        private bool _isPowerUp;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // Cache all required components
            _rigidBody = GetComponent<Rigidbody2D>();
            _physicsCollider = GetComponent<CircleCollider2D>();
            _triggerCollider = GetComponent<BoxCollider2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            // Determine if this object is a power-up by checking its layer name
            _isPowerUp = LayerMask.LayerToName(gameObject.layer) == "PowerUp";
        }

        private void Start()
        {
            // Trigger a power-up event if applicable
            if (_isPowerUp)
            {
                OnPowerUpAppears?.Invoke();
            }

            // Start the spawn animation
            StartCoroutine(Animate());
        }

        #endregion

        #region Animation Logic

        /// <summary>
        /// Handles the "emerge from block" animation sequence:
        /// - Disables physics and visibility
        /// - Waits briefly
        /// - Moves the item upward smoothly
        /// - Re-enables physics and colliders
        /// </summary>
        private IEnumerator Animate()
        {
            // 1️⃣ Disable physics and rendering before animation
            if (_rigidBody != null)
                _rigidBody.bodyType = RigidbodyType2D.Kinematic;
            if (_physicsCollider != null)
                _physicsCollider.enabled = false;
            if (_triggerCollider != null)
                _triggerCollider.enabled = false;
            if (_spriteRenderer != null)
                _spriteRenderer.enabled = false;

            // 2️⃣ Small delay before appearing
            yield return new WaitForSeconds(0.25f);

            // 3️⃣ Enable sprite for the rising animation
            if (_spriteRenderer != null)
                _spriteRenderer.enabled = true;

            // 4️⃣ Animate upward movement
            const float duration = 1f;
            float elapsed = 0f;
            Vector3 startPosition = transform.localPosition;
            Vector3 endPosition = transform.localPosition + Vector3.up;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                transform.localPosition = Vector3.Lerp(startPosition, endPosition, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = endPosition;

            // 5️⃣ Re-enable physics and collisions
            if (_rigidBody != null)
                _rigidBody.bodyType = RigidbodyType2D.Dynamic;
            if (_physicsCollider != null)
                _physicsCollider.enabled = true;
            if (_triggerCollider != null)
                _triggerCollider.enabled = true;
        }

        #endregion
    }
}
