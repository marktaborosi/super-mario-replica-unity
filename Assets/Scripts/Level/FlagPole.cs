using UnityEditor;

namespace Level
{
    using System;
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Handles the flagpole sequence triggered when the player touches the goal flag.  
    /// Responsible for:
    /// - Lowering the flag and sliding the player down
    /// - Playing a short "cinematic" to the castle
    /// - Signaling the end of the level
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class FlagPole : MonoBehaviour
    {
        #region Inspector Fields

        [Header("Transforms")] [Tooltip("Reference to the flag object that will slide down the pole.")]
        public Transform flag;

        [Tooltip("Bottom point of the flagpole where the player and flag stop sliding.")]
        public Transform poleBottom;

        [Tooltip("Target position of the castle the player will walk toward.")]
        public Transform castle;

        [Header("Parameters")] [Tooltip("Movement speed used for cinematic transitions.")]
        public float speed = 6f;

        [Tooltip("World index of the next level.")]
        public int nextWorld = 1;

        [Tooltip("Stage index of the next level.")]
        public int nextStage = 1;

        #endregion

        #region Events

        /// <summary>
        /// Fired when the flagpole sequence begins (player touches the pole).
        /// </summary>
        public static event Action OnFlagPole;

        /// <summary>
        /// Fired when the level completion cinematic begins.
        /// </summary>
        public static event Action OnLevelFinish;

        #endregion

        #region Private Fields

        private Rigidbody2D _rigidBody;
        private bool _sequenceRunning;
        private bool _playerAtBottom;
        private bool _flagAtBottom;

        #endregion

        #region Unity Callbacks

        /// <summary>
        /// Triggered when a collider enters the flagpole trigger zone.  
        /// Starts the flagpole sequence if the collider is the player.
        /// </summary>
        /// <param name="other">The collider that entered the trigger.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            // ✅ Only respond once and only to the player
            if (_sequenceRunning || !other.CompareTag("Player"))
                return;

            _sequenceRunning = true;

            // Get references to components on the player
            var playerMovement = other.GetComponent<Player.PlayerMovement>();
            _rigidBody = other.GetComponent<Rigidbody2D>();

            if (playerMovement == null || _rigidBody == null)
            {
                Debug.LogWarning($"[{nameof(FlagPole)}] Missing required Player components — aborting sequence.");
                return;
            }

            // Disable player control before the cinematic
            playerMovement.enabled = false;

            // Start the flagpole cutscene
            StartCoroutine(FlagPoleSequence(playerMovement, other.transform));
        }

        #endregion

        #region Flagpole Sequence

        /// <summary>
        /// Handles the main flagpole cutscene:
        /// - Player and flag slide down
        /// - Level finish event is triggered
        /// - Player walks into the castle
        /// </summary>
        private IEnumerator FlagPoleSequence(Player.PlayerMovement playerMovement, Transform player)
        {
            // Set state flags
            playerMovement.flagPoleHold = true;
            _playerAtBottom = false;
            _flagAtBottom = false;

            // 🔽 Begin sliding both the player and flag down
            OnFlagPole?.Invoke();
            StartCoroutine(MoveAndMark(player, poleBottom.position, () => _playerAtBottom = true));
            StartCoroutine(MoveAndMark(flag, poleBottom.position, () => _flagAtBottom = true));

            // Wait until both reach the bottom
            yield return new WaitUntil(() => _playerAtBottom && _flagAtBottom);

            // Release the player's hold on the pole
            playerMovement.flagPoleHold = false;

            // 🏁 Begin the end-of-level cinematic
            OnLevelFinish?.Invoke();
            yield return LevelCompleteSequence(playerMovement, player);

            _sequenceRunning = false;
        }

        /// <summary>
        /// Handles the post-flag sequence:
        /// - Player walks a few steps right
        /// - Moves diagonally toward the ground
        /// - Walks into the castle and disappears
        /// </summary>
        private IEnumerator LevelCompleteSequence(Player.PlayerMovement playerMovement, Transform player)
        {
            playerMovement.cinematic = true;

            // Step 1️⃣: Move a small distance to the right
            yield return MoveTo(player, player.position + Vector3.right);

            // Step 2️⃣: Move diagonally down and right
            yield return MoveTo(player, player.position + Vector3.right + Vector3.down);

            // Step 3️⃣: Walk into the castle
            float distanceToCastle = castle.position.x - player.position.x;
            yield return MoveForward(_rigidBody, distanceToCastle, 7f);

            playerMovement.cinematic = false;

            // Hide player after entering the castle
            player.gameObject.SetActive(false);

            // Optional small pause before level transition
            yield return new WaitForSeconds(6f);

            QuitGame();
        }

        private void QuitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
# else
            Application.Quit();
#endif
        }

        #endregion

        #region Movement Helpers

        /// <summary>
        /// Smoothly moves a transform to a target position.
        /// </summary>
        private IEnumerator MoveTo(Transform subject, Vector3 destination)
        {
            const float epsilon = 0.125f;

            while (Vector3.Distance(subject.position, destination) > epsilon)
            {
                subject.position = Vector3.MoveTowards(subject.position, destination, speed * Time.deltaTime);
                yield return null;
            }

            subject.position = destination;
        }

        /// <summary>
        /// Moves a transform to a target position and calls a callback when complete.
        /// </summary>
        private IEnumerator MoveAndMark(Transform subject, Vector3 destination, Action onComplete)
        {
            yield return MoveTo(subject, destination);
            onComplete?.Invoke();
        }

        /// <summary>
        /// Moves a Rigidbody2D forward for a given distance and stops it.
        /// </summary>
        private IEnumerator MoveForward(Rigidbody2D rb, float distance, float thisSpeed)
        {
            float duration = distance / thisSpeed;
            rb.linearVelocity = new Vector2(thisSpeed, 0f);

            yield return new WaitForSeconds(duration);

            rb.linearVelocity = Vector2.zero;
        }

        #endregion
    }
}