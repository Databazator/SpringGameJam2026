using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Triggers events when object enters/exits trigger area.
/// </summary>
public class EventTrigger : MonoBehaviour
{
    /// <summary>
    /// Occurs when some object enters the trigger area.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<GameObject> OnObjectEnter { get; private set; } = new();
    /// <summary>
    /// Occurs when some object leaves the trigger area.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<GameObject> OnObjectExit { get; private set; } = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.attachedRigidbody || collision.attachedRigidbody.bodyType == RigidbodyType2D.Static)
            return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Fish"))
            return;

        OnObjectEnter.Invoke(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.attachedRigidbody || collision.attachedRigidbody.bodyType == RigidbodyType2D.Static)
            return;
        if (collision.gameObject.layer == LayerMask.NameToLayer("Fish"))
            return;

        OnObjectExit.Invoke(collision.gameObject);
    }
}
