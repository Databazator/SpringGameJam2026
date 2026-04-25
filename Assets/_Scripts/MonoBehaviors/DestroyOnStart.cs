using UnityEngine;

public class DestroyOnStart : MonoBehaviour
{
    public bool destroy = true;

    private void Start()
    {
        if(destroy)
            Destroy(this.gameObject);
    }
}
