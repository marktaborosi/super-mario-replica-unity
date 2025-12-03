using UnityEngine;

/// <summary>
/// Handles basic 2D movement logic for autonomous entities such as enemies.
/// Entities move in a given direction with a defined speed and respond to collisions by reversing direction.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EntityMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Horizontal movement speed of the entity.")]
    public float speed = 1f;

    [Tooltip("Initial direction of movement. Typically Vector2.left or Vector2.right.")]
    public Vector2 direction = Vector2.left;

    // --- Private fields ---
    private Rigidbody2D _rigidbody;
    private Vector2 _velocity;

    #region Unity Lifecycle

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        enabled = false; // Entities are disabled by default until visible
    }

    /// <summary>
    /// Called when the object first enters the camera's view.
    /// Enables movement logic for performance optimization.
    /// </summary>
    private void OnBecameVisible()
    {
        enabled = true;
    }

    /// <summary>
    /// Called when the object exits the camera's view.
    /// Disables movement logic to save performance.
    /// </summary>
    private void OnBecameInvisible()
    {
        enabled = false;
    }

    /// <summary>
    /// Wakes up the Rigidbody when the movement component is enabled.
    /// </summary>
    private void OnEnable()
    {
        _rigidbody.WakeUp();
    }

    /// <summary>
    /// Stops movement and puts the Rigidbody to sleep when disabled.
    /// </summary>
    private void OnDisable()
    {
        _rigidbody.linearVelocity = Vector2.zero;
        _rigidbody.Sleep();
    }

    /// <summary>
    /// Handles entity physics updates.  
    /// Applies velocity, checks for collisions, reverses direction, and applies gravity.
    /// </summary>
    private void FixedUpdate()
    {
        ApplyHorizontalMovement();
        ApplyGravity();
        MoveEntity();
        HandleCollisionReversal();
        HandleGroundDetection();
    }

    #endregion

    #region Movement Logic

    /// <summary>
    /// Calculates horizontal velocity based on direction and speed.
    /// </summary>
    private void ApplyHorizontalMovement()
    {
        _velocity.x = direction.x * speed;
    }

    /// <summary>
    /// Simulates gravity for vertical motion.
    /// </summary>
    private void ApplyGravity()
    {
        _velocity.y += Physics2D.gravity.y * Time.fixedDeltaTime;
    }

    /// <summary>
    /// Moves the entity according to the calculated velocity.
    /// </summary>
    private void MoveEntity()
    {
        _rigidbody.MovePosition(_rigidbody.position + _velocity * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Detects forward collisions (walls, obstacles) and reverses movement direction.
    /// </summary>
    private void HandleCollisionReversal()
    {
        if (_rigidbody.RayCastCircle(direction))
        {
            direction = -direction;
        }
    }

    /// <summary>
    /// Detects ground below the entity and prevents it from sinking.
    /// </summary>
    private void HandleGroundDetection()
    {
        if (_rigidbody.RayCastCircle(Vector2.down))
        {
            _velocity.y = Mathf.Max(_velocity.y, 0f);
        }
    }

    #endregion
}
