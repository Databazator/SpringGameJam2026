using EasyButtons;
using UnityEngine;
using UnityEngine.Events;

public class TrebuchetController : MonoBehaviour
{
    private ShooterController shooterController;

    /// <summary>
    /// Active cat.
    /// </summary>
    [SerializeField]
    private GameObject cat;

    /// <summary>
    /// Occurs when cat collides with the environment.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<GameObject> OnCatCollidesEnvironment { get; private set; } = new();
    /// <summary>
    /// Occurs when cat lands in the box area trigger zone.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<GameObject> OnCatLandInBox { get; private set; } = new();
    /// <summary>
    /// Occurs when cat is launched from the trebuchet.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<GameObject> OnCatLaunched { get; private set; } = new();
    /// <summary>
    /// Occurs when cat is reloaded.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<GameObject> OnCatReloaded { get; private set; } = new();

    /// <summary>
    /// Active cat.
    /// </summary>
    public GameObject Cat
    {
        get => cat;
        set
        {
            if (cat != null)
            {
                var catLander = cat.GetComponent<CatLander>();
                catLander.OnCollidesEnvironment.RemoveListener(CatLander_OnEnvironemntCollision);
                catLander.OnLandInBox.RemoveListener(CatLander_OnCatBoxAreaCollision);
            }

            cat = value;

            if (cat != null)
            {
                var catLander = cat.GetComponent<CatLander>();
                catLander.OnCollidesEnvironment.AddListener(CatLander_OnEnvironemntCollision);
                catLander.OnLandInBox.AddListener(CatLander_OnCatBoxAreaCollision);
            }
        }
    }

    private void Awake()
    {
        Debug.Assert(Cat != null);

        var catLander = cat.GetComponent<CatLander>();
        catLander.OnCollidesEnvironment.AddListener(CatLander_OnEnvironemntCollision);
        catLander.OnLandInBox.AddListener(CatLander_OnCatBoxAreaCollision);

        shooterController = GetComponentInChildren<ShooterController>();
        shooterController.Projectile = cat.GetComponent<Rigidbody2D>();
        shooterController.OnProjectileLaunched.AddListener(Shooter_OnProjectileLaunched);
        shooterController.OnProjectileReset.AddListener(Shooter_OnProjectileReset);
    }

    /// <summary>
    /// Resets projectile to shooter's starting point and reset it's physical properties.
    /// </summary>
    [Button]
    public void Rearm()
    {
        shooterController.ResetProjectile();
    }

    private void CatLander_OnEnvironemntCollision(GameObject cat)
    {
        Debug.Assert(cat == Cat);
        OnCatCollidesEnvironment.Invoke(cat);
    }

    private void CatLander_OnCatBoxAreaCollision(GameObject cat)
    {
        Debug.Assert(cat == Cat);
        OnCatLandInBox.Invoke(cat);
    }

    private void Shooter_OnProjectileLaunched(GameObject cat)
    {
        Debug.Assert(cat == Cat);
        OnCatLaunched.Invoke(cat);
    }

    private void Shooter_OnProjectileReset(GameObject cat)
    {
        Debug.Assert(cat == Cat);
        OnCatReloaded.Invoke(cat);
    }
}
