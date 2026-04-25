using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles logic related to cat's landing.
/// </summary>
public class CatLander : MonoBehaviour
{
    /// <summary>
    /// Occurs when cat collides with the environment.
    /// </summary>
    [SerializeField]
    public UnityEvent<GameObject> OnCollidesEnvironment = new();

    /// <summary>
    /// Occurs when cat lands in the box area trigger zone.
    /// </summary>
    [SerializeField]
    public UnityEvent<GameObject> OnLandInBox = new();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
            OnCollidesEnvironment.Invoke(gameObject);

        if (collision.gameObject.layer == LayerMask.NameToLayer("CatBox"))
            OnLandInBox.Invoke(gameObject);
    }
}
