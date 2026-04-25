using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Pushes rigid bodies in certain direction by certain strength. Applies to rigid bodies whihc enters it's
/// trigger area.
/// </summary>
public class Pusher : MonoBehaviour
{
    /// <summary>
    /// Counts number of active pushers for each flow.
    /// </summary>
    private static readonly IDictionary<(AirFlowController, Rigidbody2D), int> counter 
        = new Dictionary<(AirFlowController, Rigidbody2D), int>();

    public static Pusher Create(GameObject gameObject, AirFlowController airFlow)
    {
        var pusher = gameObject.AddComponent<Pusher>();
        pusher.AirFlow = airFlow;

        return pusher;
    }

    /// <summary>
    /// Push direction.
    /// </summary>
    [field: SerializeField]
    public Vector2 Direction { get; set; } = Vector2.right;

    /// <summary>
    /// Push strength (force vector magnitude).
    /// </summary>
    public float Strength => AirFlow.Strength;

    /// <summary>
    /// Associated air flow.
    /// </summary>
    [field: SerializeField]
    public AirFlowController AirFlow { get; set; } = null;

    private void Start()
    {
        Debug.Assert(AirFlow);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rigidbody = other.attachedRigidbody;
        if (!rigidbody || rigidbody.bodyType != RigidbodyType2D.Dynamic)
            return;

        var constantForce = rigidbody.GetComponent<ConstantForce2D>();
        if (!constantForce)
            constantForce = rigidbody.AddComponent<ConstantForce2D>();

        constantForce.force = Direction.normalized * Strength;

        // cannot use `GetValueRefOrAddDefault` because that needs .NET 6
        if (!counter.ContainsKey((AirFlow, rigidbody)))
            counter.Add((AirFlow, rigidbody), 0);
        counter[(AirFlow, rigidbody)]++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rigidbody = other.attachedRigidbody;
        if (!rigidbody || rigidbody.bodyType != RigidbodyType2D.Dynamic)
            return;

        Debug.Assert(counter.ContainsKey((AirFlow, rigidbody)));
        counter[(AirFlow, rigidbody)]--;
        
        Debug.Assert(counter[(AirFlow, rigidbody)] >= 0);

        if (counter[(AirFlow, rigidbody)] == 0)
        {
            var constantForce = rigidbody.GetComponent<ConstantForce2D>();
            Debug.Assert(constantForce);

            constantForce.force = Vector2.zero;
        }
    }
}
