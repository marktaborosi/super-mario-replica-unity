using UnityEngine;

/// <summary>
/// Contains extension methods for physics and spatial checks commonly used in gameplay.
/// These helpers are designed to make collision detection, environment checks, and direction tests simpler and more readable.
/// </summary>
public static class Extensions
{
    // ✅ Cached layer mask for default collisions (can be customized as needed)
    private static readonly LayerMask DefaultLayerMask = LayerMask.GetMask("Default");

    #region Physics Cast Extensions

    /// <summary>
    /// Performs a small <see cref="Physics2D.CircleCast"/> in a given direction from a <see cref="Rigidbody2D"/> position  
    /// to check if there is an obstacle nearby.
    /// </summary>
    /// <param name="rigidbody">The Rigidbody2D to cast from.</param>
    /// <param name="direction">The direction to cast the circle in.</param>
    /// <returns><c>true</c> if a nearby obstacle is detected (excluding itself); otherwise <c>false</c>.</returns>
    public static bool RayCastCircle(this Rigidbody2D rigidbody, Vector2 direction)
    {
        if (rigidbody == null || rigidbody.bodyType == RigidbodyType2D.Kinematic)
            return false;

        const float radius = 0.25f;
        const float distance = 0.375f;

        var hit = Physics2D.CircleCast(
            rigidbody.position,
            radius,
            direction.normalized,
            distance,
            DefaultLayerMask
        );

        return hit.collider != null && hit.rigidbody != rigidbody;
    }

    /// <summary>
    /// Performs a small <see cref="Physics2D.BoxCast"/> in a given direction from a <see cref="Rigidbody2D"/> position  
    /// to check if there is an obstacle nearby.
    /// </summary>
    /// <param name="rigidbody">The Rigidbody2D to cast from.</param>
    /// <param name="direction">The direction to cast the box in.</param>
    /// <returns><c>true</c> if a nearby obstacle is detected (excluding itself); otherwise <c>false</c>.</returns>
    public static bool RayCastBox(this Rigidbody2D rigidbody, Vector2 direction)
    {
        if (rigidbody == null || rigidbody.bodyType == RigidbodyType2D.Kinematic)
            return false;

        // Define the size of the box cast (width, height)
        var boxSize = new Vector2(0.5f, 0.5f);
        const float distance = 0.375f;

        var hit = Physics2D.BoxCast(
            rigidbody.position,
            boxSize,
            0f,                    // Angle: 0 = axis-aligned
            direction.normalized,
            distance,
            DefaultLayerMask
        );

        return hit.collider != null && hit.rigidbody != rigidbody;
    }

    #endregion

    #region Transform Extensions

    /// <summary>
    /// Determines whether another <see cref="Transform"/> is roughly in a given direction relative to this transform.  
    /// This uses a dot product check for efficient directional testing.
    /// </summary>
    /// <param name="transform">The reference transform.</param>
    /// <param name="other">The target transform to test against.</param>
    /// <param name="testDirection">The direction to test (e.g., <see cref="Vector2.up"/>, <see cref="Vector2.down"/>).</param>
    /// <returns><c>true</c> if the other transform is in the specified direction (within ~60° cone); otherwise <c>false</c>.</returns>
    public static bool IsInDirectionDotTest(this Transform transform, Transform other, Vector2 testDirection)
    {
        if (transform == null || other == null)
            return false;

        var directionToOther = (other.position - transform.position).normalized;
        return Vector2.Dot(directionToOther, testDirection.normalized) > 0.5f;
    }

    #endregion
}
