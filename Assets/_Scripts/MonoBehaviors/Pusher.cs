using UnityEngine;

/// <summary>
/// Event handler for pusher area events.
/// </summary>
/// <param name="target">Target which enters/leave pusher area.</param>
/// <param name="direction">Pusher direction.</param>
public delegate void PusherAreaEventHandler(Pusher sender, Rigidbody2D target);

/// <summary>
/// Pushes rigid bodies in certain direction by certain strength. Applies to rigid bodies which enters it's
/// trigger area.
/// </summary>
public class Pusher : MonoBehaviour
{
    /// <summary>
    /// Occurs when dynamic rigid body enters pusher area.
    /// </summary>
    public event PusherAreaEventHandler OnAreaEnter;
    /// <summary>
    /// Occurs when dynamic rigid body leaves pusher area.
    /// </summary>
    public event PusherAreaEventHandler OnAreaExit;

    /// <summary>
    /// Push direction.
    /// </summary>
    [field: SerializeField]
    public Vector2 Direction { get; set; } = Vector2.right;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rigidbody = other.attachedRigidbody;
        if (!rigidbody || rigidbody.bodyType != RigidbodyType2D.Dynamic)
            return;

        OnAreaEnter?.Invoke(this, rigidbody);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rigidbody = other.attachedRigidbody;
        if (!rigidbody || rigidbody.bodyType != RigidbodyType2D.Dynamic)
            return;

        OnAreaExit?.Invoke(this, rigidbody);
    }
}
