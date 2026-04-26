using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.0f;

    void Update()
    {
        transform.localPosition += Vector3.up * speed;
    }
}
