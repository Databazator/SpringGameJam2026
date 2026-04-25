using TreeEditor;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Overrides linear velocity for all rigid bodies which enters air flow. Force vector at any point is
/// evaluated as spoline tangent time strength.
/// </summary>
[RequireComponent(typeof(SplineContainer))]
public class AirFlowController : MonoBehaviour
{
    private static int flowCounter = 0;

    private SplineContainer splineContainer;

    public int ID { get; } = flowCounter + 1;

    /// <summary>
    /// Thickness of the flow along the spline.
    /// </summary>
    [field: SerializeField]
    public float Thickness { get; private set; } = 1.0f;

    /// <summary>
    /// Force vector magnitude.
    /// </summary>
    [field: SerializeField]
    public float Strength { get; set; } = 1.0f;

    /// <summary>
    /// Trigger spawned at each step along the spline.
    /// </summary>
    [field: SerializeField]
    public float StepSize { get; private set; } = 0.1f;

    private void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();

        float length = splineContainer.CalculateLength();
        for (float t = 0; t < length; t += StepSize)
        {
            splineContainer.Evaluate(t / length, out float3 position, out float3 tangent, out _);

            var gameObject = new GameObject();
            gameObject.transform.SetParent(transform);
            gameObject.transform.position = position.ToVector2();

            var collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = Thickness / 2.0f;

            var pusher = Pusher.Create(gameObject, this);
            pusher.Direction = tangent.ToVector2();
        }
    }
}
