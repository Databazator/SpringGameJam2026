using MarkusSecundus.Utils.Datastructs;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

/// <summary>
/// Overrides linear velocity for all rigid bodies which enters air flow. Force vector at any point is
/// evaluated as spline tangent times strength.
/// </summary>
[RequireComponent(typeof(SplineContainer))]
public class AirFlowController : MonoBehaviour
{
    private SplineContainer splineContainer;
    private IDictionary<Rigidbody2D, List<Pusher>> affectedBodies
        = new Dictionary<Rigidbody2D, List<Pusher>>();

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

            var gameObject = new GameObject("Pusher");
            gameObject.transform.SetParent(transform);
            gameObject.transform.position = position.ToVector2();

            var collider = gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = Thickness / 2.0f;

            var pusher = gameObject.AddComponent<Pusher>();
            pusher.Direction = tangent.ToVector2();
            pusher.OnAreaEnter += Pusher_OnAreaEnter;
            pusher.OnAreaExit += Pusher_OnAreaExit;
        }
    }

    private void FixedUpdate()
    {
        foreach (var (rigidBody, pushers) in affectedBodies)
        {
            rigidBody.AddForce(pushers.Last().Direction * Strength);
        }
    }

    private void Pusher_OnAreaEnter(Pusher pusher, Rigidbody2D rigidBody)
    {
        if (!affectedBodies.TryAdd(rigidBody, new List<Pusher>() { pusher }))
            affectedBodies[rigidBody].Add(pusher);
    }

    private void Pusher_OnAreaExit(Pusher pusher, Rigidbody2D rigidBody)
    {
        // O(n) remove is not ideal, better solution would probably be to use `LinkedList`
        // but it should be ok in this case
        affectedBodies[rigidBody].Remove(pusher);

        if (affectedBodies[rigidBody].IsEmpty())
            affectedBodies.Remove(rigidBody);
    }
}
