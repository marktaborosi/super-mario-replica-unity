namespace Animations
{
    using UnityEngine;

    /// <summary>
    /// Handles simple frame-based sprite animation by cycling through an array of sprites.
    /// Attach this to a GameObject with a <see cref="SpriteRenderer"/> to create a looping animation.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class AnimatedSprite : MonoBehaviour
    {
        [Header("Animation Frames")]
        [Tooltip("The list of sprites to loop through for this animation.")]
        public Sprite[] sprites;

        [Header("Settings")]
        [Tooltip("Time in seconds between each frame update.")]
        public float framerate = 1f / 6f;

        private SpriteRenderer _spriteRenderer;
        private int _frame;

        #region Unity Lifecycle

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnEnable()
        {
            // Reset frame index to start animation from the beginning each time it's enabled
            _frame = 0;

            // Safety check to avoid errors if no sprites are assigned
            if (sprites == null || sprites.Length == 0)
            {
                return;
            }

            // Start the animation loop
            InvokeRepeating(nameof(Animate), framerate, framerate);
        }

        private void OnDisable()
        {
            CancelInvoke(nameof(Animate));
        }

        #endregion

        #region Animation Logic

        /// <summary>
        /// Advances the animation by one frame and updates the <see cref="SpriteRenderer"/> sprite.
        /// </summary>
        private void Animate()
        {
            if (sprites == null || sprites.Length == 0)
                return;

            _frame++;

            // Loop back to the start if we've reached the end of the sprite array
            if (_frame >= sprites.Length)
            {
                _frame = 0;
            }

            // Update the current sprite safely
            _spriteRenderer.sprite = sprites[_frame];
        }

        #endregion
    }
}
