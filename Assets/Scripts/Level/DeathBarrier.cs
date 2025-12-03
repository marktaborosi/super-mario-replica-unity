using System;
using UnityEngine;

namespace Level
{
    /// <summary>
    /// A death barrier placed at the bottom (or edges) of a level.  
    /// When the player enters it, the level resets after a short delay.  
    /// Any other object that touches it is immediately destroyed.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class DeathBarrier : MonoBehaviour
    {
        #region Events

        /// <summary>
        /// Invoked when the player enters the death barrier trigger.
        /// </summary>
        public static event Action OnTouchDeathBarrier;

        #endregion

        #region Unity Callbacks

        /// <summary>
        /// Triggered when any Collider2D enters the death barrier.  
        /// If the object is the player, trigger a death sequence and reset the level.  
        /// Otherwise, destroy the object.
        /// </summary>
        /// <param name="other">The collider that entered the trigger zone.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                HandlePlayerDeath(other.gameObject);
            }
            else
            {
                HandleNonPlayerObject(other.gameObject);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the case when the player touches the death barrier:
        /// - Fires the death event
        /// - Disables the player object
        /// - Requests a level reset from the GameManager after a delay
        /// </summary>
        /// <param name="player">The player GameObject.</param>
        private void HandlePlayerDeath(GameObject player)
        {
            OnTouchDeathBarrier?.Invoke();

            // Disable the player immediately (so they disappear / stop interacting)
            player.SetActive(false);

            // Request a level reset from the GameManager after a delay
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetLevel(3f);
            }
            else
            {
                Debug.LogWarning($"[{nameof(DeathBarrier)}] No GameManager instance found — cannot reset level.");
            }
        }

        /// <summary>
        /// Handles all non-player objects entering the death barrier by destroying them.
        /// </summary>
        /// <param name="obj">The GameObject to destroy.</param>
        private void HandleNonPlayerObject(GameObject obj)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        #endregion
    }
}
