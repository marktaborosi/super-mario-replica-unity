using System;

namespace Level
{
    using System.Collections;
    using UnityEngine;

    /// <summary>
    /// Handles classic "pipe travel" behavior.  
    /// When the player stands on the pipe and presses a key (default: S), they are animated into the pipe,  
    /// teleported to a connected pipe, and reappear at the destination.  
    /// 
    /// Events:
    /// - <see cref="OnPipeEnter"/>: Triggered when entering a pipe.
    /// - <see cref="OnUndeground"/>: Triggered when emerging underground.
    /// - <see cref="OnAbove"/>: Triggered when emerging above ground.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Pipe : MonoBehaviour
    {
        #region Inspector Fields

        [Header("Pipe Settings")]
        [Tooltip("Destination transform the player will exit from.")]
        public Transform connection;

        [Tooltip("Key the player must press to enter the pipe.")]
        public KeyCode enterKeyCode = KeyCode.S;

        [Tooltip("Direction the player moves when entering the pipe.")]
        public Vector3 enterDirection = Vector3.down;

        [Tooltip("Optional direction to offset the player when exiting the pipe.")]
        public Vector3 exitDirection = Vector3.zero;

        #endregion

        #region Events

        public static event Action OnPipeEnter;
        public static event Action OnUndeground;
        public static event Action OnAbove;

        #endregion

        #region Unity Callbacks

        /// <summary>
        /// Called once per frame while another collider stays inside this pipe's trigger.
        /// If the player is standing on the pipe and presses the enter key, the pipe sequence starts.
        /// </summary>
        private void OnTriggerStay2D(Collider2D other)
        {
            // ✅ Only respond to player collisions with a valid connection target
            if (connection == null || !other.CompareTag("Player"))
                return;

            var playerMovementComponent = other.GetComponent<Player.PlayerMovement>();
            if (playerMovementComponent == null)
                return;

            // ✅ Require player input before triggering pipe entry
            if (!Input.GetKey(enterKeyCode))
                return;

            // 🚀 Begin the pipe travel sequence
            OnPipeEnter?.Invoke();
            StartCoroutine(EnterAnimation(playerMovementComponent, other.transform));
        }

        #endregion

        #region Pipe Animation Logic

        /// <summary>
        /// Handles the full pipe travel sequence:
        /// - Player moves into the pipe.
        /// - Game waits briefly (simulating travel).
        /// - Player reappears at the destination.
        /// </summary>
        private IEnumerator EnterAnimation(Player.PlayerMovement playerMovement, Transform player)
        {
            // 🛑 Disable player control during the animation
            playerMovement.enabled = false;

            // Step 1️⃣: Animate player shrinking and moving into the pipe
            Vector3 enteredPosition = transform.position + enterDirection;
            Vector3 enteredScale = Vector3.one * 0.5f;
            yield return Move(player, enteredPosition, enteredScale);

            // Step 2️⃣: Wait while the player "travels" through the pipe
            yield return new WaitForSeconds(1f);

            // Step 3️⃣: Handle underground vs aboveground environment
            bool isUnderground = connection.position.y < 0f;
            var mainCamera = Camera.main;

            if (mainCamera != null)
            {
                var scrollComponent = mainCamera.GetComponent<SideScroll>();
                if (scrollComponent != null)
                {
                    scrollComponent.SetUnderground(isUnderground);
                }
                else
                {
                    Debug.LogWarning($"[{nameof(Pipe)}] No SideScroll component found on Camera.");
                }
            }
            else
            {
                Debug.LogWarning($"[{nameof(Pipe)}] No Main Camera found.");
            }

            // Step 4️⃣: Handle exit logic (optional offset or instant teleport)
            if (exitDirection != Vector3.zero)
            {
                // Adjust player spawn point relative to the exit pipe
                player.position = connection.position - exitDirection;

                // Optional camera reposition
                if (mainCamera != null)
                {
                    Vector3 cameraPos = mainCamera.transform.position;
                    cameraPos.y = 6.5f;
                    mainCamera.transform.position = cameraPos;
                }

                // Fire event for above-ground exit
                OnAbove?.Invoke();

                // Animate player emerging from the pipe
                yield return Move(player, connection.position + exitDirection, Vector3.one);
            }
            else
            {
                // Teleport directly without animation
                OnUndeground?.Invoke();
                player.position = connection.position;
                player.localScale = Vector3.one;
            }

            // ✅ Re-enable player control
            playerMovement.enabled = true;
        }

        /// <summary>
        /// Smoothly moves and scales a player transforms over time.
        /// </summary>
        private IEnumerator Move(Transform player, Vector3 endPosition, Vector3 endScale)
        {
            const float duration = 1f;
            float elapsed = 0f;

            Vector3 startPosition = player.position;
            Vector3 startScale = player.localScale;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                player.position = Vector3.Lerp(startPosition, endPosition, t);
                player.localScale = Vector3.Lerp(startScale, endScale, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            player.position = endPosition;
            player.localScale = endScale;
        }

        #endregion
    }
}
