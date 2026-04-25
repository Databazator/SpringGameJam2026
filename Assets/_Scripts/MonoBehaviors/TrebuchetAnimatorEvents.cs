using System;
using UnityEngine;

public class TrebuchetAnimatorEvents : MonoBehaviour
{
    public event EventHandler OnProjectileLaunch;

    [field: SerializeField]
    public Transform ProjectileTransform { get; set; }

    public void LaunchProjectile()
    {
        ProjectileTransform.SetParent(null, true);
        OnProjectileLaunch?.Invoke(this, new EventArgs());
    }
}
