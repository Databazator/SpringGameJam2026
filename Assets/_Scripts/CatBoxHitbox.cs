using UnityEngine;

public class CatBoxHitbox : MonoBehaviour
{
    public CatBoxCatcher Catcher;

    private void Awake()
    {
        if(!Catcher) Catcher = GetComponentInParent<CatBoxCatcher>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Catcher.CatCaught(other.gameObject);
    }
}
