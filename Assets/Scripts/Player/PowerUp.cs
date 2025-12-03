using System;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Represents a collectible power-up in the game world.
    /// When the player collides with this object, a specific effect is triggered
    /// based on the configured <see cref="Type"/>.
    /// </summary>
    public class PowerUp : MonoBehaviour
    {
        /// <summary>
        /// Defines the type of power-up and its corresponding effect.
        /// </summary>
        public enum Type
        {
            Coin,           // Adds a coin to the player's total
            ExtraLife,      // Adds an extra life
            MagicMushroom,  // Grows the player if they are small
            StarPower       // Grants temporary invincibility
        }

        [Header("Power-Up Settings")]
        [Tooltip("The type of power-up effect this object gives.")]
        public Type type;

        #region Unity Lifecycle

        /// <summary>
        /// Triggered when another collider enters the power-up's trigger zone.
        /// Only reacts if the collider belongs to the player.
        /// </summary>
        /// <param name="other">The collider that entered the trigger.</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            Collect(other.gameObject);
        }

        #endregion

        #region Collection Logic

        /// <summary>
        /// Applies the appropriate effect to the player and destroys the power-up.
        /// </summary>
        /// <param name="player">The player GameObject that collected the power-up.</param>
        private void Collect(GameObject player)
        {
            var playerController = player.GetComponent<PlayerController>();
            if (playerController == null)
            {
                Debug.LogWarning("[PowerUp] PlayerController not found on player object.");
                return;
            }

            switch (type)
            {
                case Type.Coin:
                    GameManager.Instance.AddCoin();
                    break;

                case Type.ExtraLife:
                    GameManager.Instance.AddLife();
                    break;

                case Type.MagicMushroom:
                    if (!playerController.Big)
                    {
                        playerController.Grow();
                    }
                    break;

                case Type.StarPower:
                    playerController.StarPower();
                    break;

                default:
                    Debug.LogWarning("[PowerUp] Unhandled power-up type.");
                    break;
            }

            // ✅ Destroy power-up object after collection
            Destroy(gameObject);
        }

        #endregion
    }
}
