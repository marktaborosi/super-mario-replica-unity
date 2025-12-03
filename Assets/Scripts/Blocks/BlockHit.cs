using System;
using System.Collections;
using Player;
using Enemies.Base;
using UnityEngine;

namespace Blocks
{
    /// <summary>
    /// Handles interactions with a block when hit from below by the player.
    /// Supports breakable blocks, item blocks, bump animations, and enemy collisions.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class BlockHit : MonoBehaviour
    {
        #region Inspector Fields

        [Header("Block Settings")]
        [Tooltip("Optional item prefab to spawn when the block is hit.")]
        public GameObject item;

        [Tooltip("Prefab to spawn if this block is breakable.")]
        public GameObject breakPrefab;

        [Tooltip("Sprite to use when the block has been emptied.")]
        public Sprite emptyBlock;

        [Tooltip("How many times this block can be hit. -1 = infinite.")]
        public int maxHits = -1;

        #endregion

        #region Events

        public static event Action OnBlockBreak;
        public static event Action OnBump;

        #endregion

        #region Private Fields

        private BlockBreakEffect _blockBreakEffect;
        private bool _animating;
        private PlayerController _player;
        private bool _playerHitBlockFromDown;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _player = playerObject.GetComponent<PlayerController>();
            }
            else
            {
                Debug.LogWarning($"[{nameof(BlockHit)}] Player not found. Block will not respond correctly.");
            }

            if (breakPrefab != null)
            {
                _blockBreakEffect = breakPrefab.GetComponent<BlockBreakEffect>();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Trigger a bump event if the block is empty and still hit
            if (maxHits == 0 && collision.gameObject.CompareTag("Player") &&
                collision.transform.IsInDirectionDotTest(transform, Vector2.up))
            {
                OnBump?.Invoke();
            }

            // Ignore conditions where the block should not react
            if (_animating || maxHits == 0 || 
                !collision.gameObject.CompareTag("Player") ||
                !collision.transform.IsInDirectionDotTest(transform, Vector2.up))
                return;

            _playerHitBlockFromDown = true;
            Hit();
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            // If the player is not involved, and we previously detected a hit from below, damage enemies
            if (!collision.gameObject.CompareTag("Player") && _playerHitBlockFromDown)
            {
                if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
                {
                    enemy.Hit();
                }
                _playerHitBlockFromDown = false;
            }
        }

        #endregion

        #region Block Logic

        /// <summary>
        /// Called when the player hits the block from below.
        /// </summary>
        private void Hit()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = true;

            if (maxHits == -1 && breakPrefab != null)
            {
                HandleBreakableBlock();
            }
            else
            {
                HandleNormalBlock(spriteRenderer);
            }
        }

        /// <summary>
        /// Handles behavior for breakable blocks (e.g., Big Mario breaks it).
        /// </summary>
        private void HandleBreakableBlock()
        {
            if (_player != null && _player.Big)
            {
                // Destroy block and spawn break effect
                Destroy(gameObject);
                Instantiate(breakPrefab, transform.position, Quaternion.identity);

                if (_blockBreakEffect != null)
                    _blockBreakEffect.enabled = true;

                OnBlockBreak?.Invoke();
            }
            else if (_player != null && _player.Small)
            {
                // Small Mario only bumps the block
                OnBump?.Invoke();
                StartCoroutine(Animate());
            }
        }

        /// <summary>
        /// Handles behavior for regular blocks (finite hits or containing items).
        /// </summary>
        private void HandleNormalBlock(SpriteRenderer spriteRenderer)
        {
            // Decrease the remaining hit count if applicable
            if (maxHits > 0)
            {
                maxHits--;
            }

            // If the block is now empty, update sprite and collider
            if (maxHits == 0)
            {
                OnBump?.Invoke();
                spriteRenderer.sprite = emptyBlock;

                var boxCollider = GetComponent<BoxCollider2D>();
                var size = boxCollider.size;
                size.x = 0.9f;
                boxCollider.size = size;
            }

            // Spawn item if defined, otherwise just bump
            if (item != null)
            {
                OnBump?.Invoke();
                Instantiate(item, transform.position, Quaternion.identity);
            }
            else
            {
                OnBump?.Invoke();
            }

            StartCoroutine(Animate());
        }

        #endregion

        #region Animation

        /// <summary>
        /// Plays the block bump animation.
        /// </summary>
        private IEnumerator Animate()
        {
            _animating = true;

            var restingPosition = transform.localPosition;
            var animatedPosition = restingPosition + Vector3.up * 0.5f;

            yield return Move(restingPosition, animatedPosition);
            yield return Move(animatedPosition, restingPosition);

            _animating = false;
        }

        /// <summary>
        /// Moves the block smoothly from one position to another over time.
        /// </summary>
        private IEnumerator Move(Vector3 from, Vector3 to)
        {
            const float duration = 0.125f;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                var t = elapsed / duration;
                transform.localPosition = Vector3.Lerp(from, to, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = to;
        }

        #endregion
    }
}
