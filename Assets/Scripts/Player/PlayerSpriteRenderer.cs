using Animations;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Handles the player's visual state transitions by switching between static sprites and animations.
    /// Controls which sprite or animation is displayed based on the player's current movement state:
    /// - Idle, Jumping, Sliding, Running
    /// - Flag pole hold (end-level animation)
    /// - Cinematic movement (automatic sequences)
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class PlayerSpriteRenderer : MonoBehaviour
    {
        #region Inspector Fields

        [Header("Sprite Renderer")]
        public SpriteRenderer SpriteRenderer { get; private set; }

        [Header("Static Sprites")]
        [Tooltip("Sprite used when the player is idle.")]
        public Sprite idle;

        [Tooltip("Sprite used when the player is jumping.")]
        public Sprite jump;

        [Tooltip("Sprite used when the player is sliding.")]
        public Sprite slide;

        [Tooltip("Sprite used when holding the flag pole.")]
        public Sprite flagPoleHold;

        [Header("Animations")]
        [Tooltip("Running animation for when the player is moving.")]
        public AnimatedSprite run;

        #endregion

        #region Private Fields

        private PlayerMovement _movement;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            _movement = GetComponentInParent<PlayerMovement>();
        }

        private void OnEnable()
        {
            SpriteRenderer.enabled = true;
        }

        private void OnDisable()
        {
            SpriteRenderer.enabled = false;
            run.enabled = false;
        }

        private void LateUpdate()
        {
            UpdateSpriteState();
        }

        #endregion

        #region Sprite Logic

        /// <summary>
        /// Updates the player's sprite based on their current movement state.
        /// Priority order:
        /// 1. Cinematic (force run animation)
        /// 2. Jumping
        /// 3. Sliding
        /// 4. Flag pole hold
        /// 5. Idle
        /// 6. Running (run animation)
        /// </summary>
        private void UpdateSpriteState()
        {
            if (_movement.cinematic)
            {
                run.enabled = true;
            }
            else if (_movement.Jumping)
            {
                SpriteRenderer.sprite = jump;
                run.enabled = false;
            }
            else if (_movement.Sliding)
            {
                SpriteRenderer.sprite = slide;
                run.enabled = false;
            }
            else if (_movement.flagPoleHold)
            {
                SpriteRenderer.sprite = flagPoleHold;
                run.enabled = false;
            }
            else if (_movement.Idle)
            {
                SpriteRenderer.sprite = idle;
                run.enabled = false;
            }
            else if (_movement.Running)
            {
                run.enabled = true;
            }
            else
            {
                // fallback - ensure run animation is off if nothing else applies
                run.enabled = false;
            }
        }

        #endregion
    }
}
