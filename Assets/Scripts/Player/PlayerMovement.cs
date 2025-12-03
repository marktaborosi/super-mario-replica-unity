using System;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Controls all aspects of player movement, including:
    /// - Horizontal acceleration, deceleration, and facing direction
    /// - Jumping and fall gravity
    /// - Collision handling with enemies and world
    /// - Screen boundary clamping
    ///
    /// This script is designed to be physics-light and uses a custom velocity system
    /// instead of relying on Unity's built-in physics forces.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(PlayerController))]
    public class PlayerMovement : MonoBehaviour
    {
        #region Events

        public static event Action OnSmallJump;
        public static event Action OnBigJump;

        #endregion

        #region Inspector Fields

        [Header("Movement Settings")]
        [Tooltip("Default horizontal movement speed.")]
        public float moveSpeed = 7f;

        [Tooltip("Run speed when holding the sprint key (Left Shift).")]
        public float fastMoveSpeed = 10f;

        [Header("Jump Settings")]
        [Tooltip("Maximum jump height in Unity units.")]
        public float maxJumpHeight = 5f;

        [Tooltip("Time in seconds to reach the top of a jump.")]
        public float maxJumpTime = 1f;

        [Header("Acceleration Settings")]
        [Tooltip("Horizontal acceleration when moving.")]
        public float acceleration = 20f;

        [Tooltip("Deceleration when changing direction.")]
        public float deceleration = 13f;

        #endregion

        #region Calculated Properties

        /// <summary>Jump impulse force based on desired jump height and duration.</summary>
        private float JumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);

        /// <summary>Gravity acceleration value calculated from jump parameters.</summary>
        private float Gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2);

        #endregion

        #region State Properties

        public bool Grounded { get; private set; }
        public bool Jumping { get; private set; }

        /// <summary>True if the player is standing still.</summary>
        public bool Idle => Mathf.Abs(_velocity.x) == 0 && !flagPoleHold;

        /// <summary>True if the player is running horizontally.</summary>
        public bool Running =>
            (Mathf.Abs(_velocity.x) > 0.25f || Mathf.Abs(_inputAxis) > 0.25f) && !flagPoleHold && !cinematic;

        /// <summary>True if the player is sliding opposite their movement direction.</summary>
        public bool Sliding => (_inputAxis > 0f && _velocity.x < 0f) || (_inputAxis < 0f && _velocity.x > 0f);

        [Header("Special States")]
        [Tooltip("If true, disables movement during the flagpole sequence.")]
        public bool flagPoleHold = false;

        [Tooltip("If true, disables player control during cinematic sequences.")]
        public bool cinematic = false;

        #endregion

        #region Private Fields

        private Rigidbody2D _rigidBody;
        private Vector2 _velocity;
        private float _inputAxis;
        private Camera _camera;
        private Collider2D _collider;
        private PlayerController _playerController;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _playerController = GetComponent<PlayerController>();
            _rigidBody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            _rigidBody.bodyType = RigidbodyType2D.Dynamic;
            _collider.enabled = true;
            _velocity = Vector2.zero;
            Jumping = false;
        }

        private void OnDisable()
        {
            _rigidBody.bodyType = RigidbodyType2D.Kinematic;
            _collider.enabled = false;
            _velocity = Vector2.zero;
            Jumping = false;
        }

        private void Update()
        {
            if (flagPoleHold || cinematic) return;

            HandleHorizontalMovement();

            // Ground check (raycast method is assumed to be implemented as an extension)
            Grounded = _rigidBody.RayCastCircle(Vector2.down);

            if (Grounded)
            {
                HandleGroundedMovement();
            }

            ApplyGravity();
        }

        private void FixedUpdate()
        {
            ApplyVelocity();
        }

        #endregion

        #region Movement Logic

        /// <summary>
        /// Handles horizontal input, acceleration, deceleration, and facing direction.
        /// </summary>
        private void HandleHorizontalMovement()
        {
            float targetMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed;

            _inputAxis = Input.GetAxis("Horizontal");
            float targetSpeed = _inputAxis * targetMoveSpeed;

            // Determine acceleration or deceleration based on input direction
            float inputSign = Math.Sign(targetSpeed);
            float velocitySign = Math.Sign(_velocity.x);
            float rate = (inputSign != 0 && inputSign != velocitySign) ? deceleration : acceleration;

            _velocity.x = Mathf.MoveTowards(_velocity.x, targetSpeed, rate * Time.deltaTime);

            // Stop if hitting a wall
            if (_rigidBody.RayCastCircle(Vector2.right * _velocity.x))
            {
                _velocity.x = 0f;
            }

            // Flip character based on movement direction
            if (_velocity.x > 0f)
                transform.eulerAngles = Vector3.zero;
            else if (_velocity.x < 0f)
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }

        /// <summary>
        /// Handles grounded movement, including jump input.
        /// </summary>
        private void HandleGroundedMovement()
        {
            _velocity.y = Mathf.Max(_velocity.y, 0f);
            Jumping = _velocity.y > 0f;

            if (Input.GetButtonDown("Jump"))
            {
                _velocity.y = JumpForce;
                Jumping = true;

                if (_playerController.Big)
                    OnBigJump?.Invoke();
                else
                    OnSmallJump?.Invoke();
            }
        }

        /// <summary>
        /// Applies gravity to the player's vertical velocity.
        /// </summary>
        private void ApplyGravity()
        {
            bool falling = _velocity.y < 0f || !Input.GetButton("Jump");
            float multiplier = falling ? 2f : 1f;

            _velocity.y += Gravity * multiplier * Time.deltaTime;
            _velocity.y = Mathf.Max(_velocity.y, Gravity / 2f);
        }

        /// <summary>
        /// Moves the player based on current velocity and clamps their position inside the camera bounds.
        /// </summary>
        private void ApplyVelocity()
        {
            Vector2 position = _rigidBody.position;
            position += _velocity * Time.fixedDeltaTime;

            // Clamp horizontal movement to screen bounds
            Vector2 leftEdge = _camera.ScreenToWorldPoint(Vector2.zero);
            Vector2 rightEdge = _camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

            _rigidBody.MovePosition(position);
        }

        #endregion

        #region Collision Handling

        private void OnCollisionEnter2D(Collision2D collision)
        {
            int layer = collision.gameObject.layer;

            if (layer == LayerMask.NameToLayer("Enemy"))
            {
                // Bounce slightly if stomping on an enemy
                if (transform.IsInDirectionDotTest(collision.transform, Vector2.down))
                {
                    _velocity.y = JumpForce / 2f;
                    Jumping = true;
                }
            }
            else if (layer != LayerMask.NameToLayer("PowerUp"))
            {
                // Stop upward movement when hitting a ceiling
                if (transform.IsInDirectionDotTest(collision.transform, Vector2.up))
                {
                    _velocity.y = 0f;
                }
            }
        }

        #endregion
    }
}
