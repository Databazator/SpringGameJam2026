using Unity.Mathematics;
using Unity.VisualScripting;
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

    /// <summary>
    /// Return copy of itself with X component set to a specified value.
    /// </summary>
    public static Vector3 WithX(this Vector3 vector, float x) => new(x, vector.y, vector.z);

    /// <summary>
    /// Return copy of itself with Y component set to a specified value.
    /// </summary>
    public static Vector3 WithY(this Vector3 vector, float y) => new(vector.x, y, vector.z);

    /// <summary>
    /// Return copy of itself with Z component set to a specified value.
    /// </summary>
    public static Vector3 WithZ(this Vector3 vector, float z) => new(vector.x, vector.y, z);
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
    /// Return copy of itself with X component set to a specified value.
    /// </summary>
    public static Vector2 WithX(this Vector2 vector, float x) => new(x, vector.y);

    /// <summary>
    /// Return copy of itself with Y component set to a specified value.
    /// </summary>
    public static Vector2 WithY(this Vector2 vector, float y) => new(vector.x, y);

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

/// <summary>
/// Contains extension methods for <see cref="float3"/> class.
/// </summary>
public static class Float3Extensions
{
    /// <summary>
    /// Converts <see cref="float3"/> to <see cref="Vector3"/>.
    /// </summary>
    public static Vector3 ToVector3(this float3 vector) => (Vector3)vector;

    /// <summary>
    /// Converts <see cref="float3"/> to <see cref="Vector2"/> truncating it in the process.
    /// </summary>
    public static Vector2 ToVector2(this float3 vector) => vector.ToVector3().Truncate();
}
