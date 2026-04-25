using UnityEngine;

/// <summary>
/// Contains extension methods for <see cref="Vector3"/> class.
/// </summary>
public static class Vector3Extensions
{
    /// <summary>
    /// Truncates <see cref="Vector3"/> into <see cref="Vector2"/> by dropping the Z component.
    /// </summary>
    public static Vector2 Truncate(this Vector3 vector) => (Vector2)vector;

    /// <summary>
    /// Returns a copy of itself with its magnitude clamped to <paramref name="maxMagnitude"/>.
    /// </summary>
    public static Vector3 ClampMagnitude(this Vector3 vector, float maxMagnitude)
        => Vector3.ClampMagnitude(vector, maxMagnitude);
}

/// <summary>
/// Contains extension methods for <see cref="Vector2"/> class.
/// </summary>
public static class Vector2Extensions
{
    /// <summary>
    /// Extend <see cref="Vector2"/> into <see cref="Vector3"/>.
    /// </summary>
    public static Vector3 Extend(this Vector2 vector, float z) => new Vector3(vector.x, vector.y, z);

    /// <summary>
    /// Returns a copy of itself with its magnitude clamped to <paramref name="maxMagnitude"/>.
    /// </summary>
    public static Vector2 ClampMagnitude(this Vector2 vector, float maxMagnitude)
        => Vector2.ClampMagnitude(vector, maxMagnitude);

    /// <summary>
    /// Converts direction vector with arbitrary magnitude to angle in degrees.
    /// </summary>
    public static float ToAngleDegrees(this Vector2 vector)
    {
        // Vector2.Angle is inefficient because it internally normalizes
        return Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
    }

    /// <summary>
    /// Clamps direction vector within a cone centered on the +X axis while preserving the vector's
    /// magnitude.
    /// </summary>
    public static Vector2 ClampConeX(this Vector2 vector, float maxAngleDegrees)
    {
        float cosAngle = Mathf.Cos(maxAngleDegrees * Mathf.Deg2Rad);

        if (vector.x < 0.0f && vector.x * vector.x >= cosAngle * cosAngle * vector.sqrMagnitude)
            return vector;

        float sinAngle = Mathf.Sin(maxAngleDegrees * Mathf.Deg2Rad);
        float sign = vector.y >= 0.0f ? 1.0f : -1.0f;

        return new Vector2(-cosAngle, sign * sinAngle) * vector.magnitude;
    }
}
