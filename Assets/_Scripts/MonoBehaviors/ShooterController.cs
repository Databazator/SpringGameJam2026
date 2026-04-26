using EasyButtons;
using MarkusSecundus.Utils.Primitives;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder;

/// <summary>
/// Controller of shooter game object. Shooter would probbaly be a catapult or a trebuchet.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class ShooterController : MonoBehaviour
{
    /// <summary>
    /// Z component of the rendered line.
    /// </summary>
    private const float lineZ = -1.0f;

    /// <summary>
    /// Determine if projectile was launched.
    /// </summary>
    private bool launched = false;

    private InputSystem_Actions Input;
    private LineRenderer lineRenderer;
    private TrebuchetAnimationController animationController;
    private TrebuchetAnimatorEvents animationEvents;

    [SerializeField]
    private Transform visualPivot;

    /// <summary>
    /// Flag for setting if trebuchet can be aimed and fired
    /// </summary>
    public bool IsControlActive = true;

    /// <summary>
    /// Projectile which could be shot. Projectile does not get automatically reloaded.
    /// </summary>
    [field: SerializeField]
    private Rigidbody2D projectile;

    /// <summary>
    /// Vector from shooter's origin to the aiming arm's end point.
    /// </summary>
    private Vector2 aimingArmVector;

    /// <summary>
    /// Occurs when projectile is launched.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<GameObject> OnProjectileLaunched { get; private set; } = new();

    /// <summary>
    /// Occurs when <see cref="ResetProjectile"/> is called.
    /// </summary>
    [field: SerializeField]
    public UnityEvent<GameObject> OnProjectileReset { get; private set; } = new();

    /// <summary>
    /// Projectile which could be shot. Projectile does not get automatically reloaded. Shooter takes
    /// authority over physical properties upon assignment.
    /// </summary>
    public Rigidbody2D Projectile
    {
        get => projectile;
        set
        {
            projectile = value;
            ResetProjectile();
        }
    }

    [field: SerializeField]
    public Transform ProjectileStartPosition { get; private set; }

    /// <summary>
    /// Maximum magnitude of the aiming arm vector.
    /// </summary>
    [field: SerializeField]
    [Range(0.0f, 10.0f)]
    public float MaxAimingArmLength { get; private set; } = 3.0f;

    /// <summary>
    /// Half-angle of the allowed aiming cone.
    /// </summary>
    [field: SerializeField]
    [Range(0.0f, 360.0f)]
    public float AimingArmRotationRangeDegrees { get; private set; } = 60.0f;

    [field: SerializeField]
    [Range(0.0f, 10.0f)]
    public float StrengthMultiplier { get; private set; } = 1.0f;

    private Vector2 GetBallisticCurvePoint(Vector2 origin, float velocity, float angle_rad, float g, float time)
        => new Vector2(origin.x + velocity * time * Mathf.Cos(angle_rad), origin.y + velocity * time * Mathf.Sin(angle_rad) + (0.5f * g * (time * time)));

    private void Awake()
    {
        Debug.Assert(ProjectileStartPosition != null);
        Debug.Assert(visualPivot != null);

        animationController = GetComponentInParent<TrebuchetAnimationController>();

        animationEvents = transform.parent.GetComponentInChildren<TrebuchetAnimatorEvents>();
        animationEvents.OnProjectileLaunch += AnimationEvents_OnProjectileLaunch;

        Input = new InputSystem_Actions();
        Input.Enable();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount += 1;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position.WithZ(0.0f));
        lineRenderer.positionCount += _ballisticCurve.TimeStepCount - 2;

        ResetProjectile();
    }

    private void Update()
    {
        if (!IsControlActive) return;

        HandleAim();
        HandleShooting();
    }

    public void SetCatAsProjectile(CatController cat)
    {
        projectile = cat.GetComponent<Rigidbody2D>();
        //ResetProjectile();
    }

    /// <summary>
    /// Resets projectile to shooter's starting point and reset it's physical properties.
    /// </summary>
    [Button]
    public void ResetProjectile()
    {
        if (projectile == null)
            return;

        projectile.bodyType = RigidbodyType2D.Kinematic;
        projectile.linearVelocity = Vector2.zero;
        projectile.angularVelocity = 0.0f;
        projectile.transform.localPosition = Vector2.zero.Extend(projectile.transform.localPosition.z);
        projectile.transform.localRotation = Quaternion.Euler(Vector3.zero);
        projectile.transform.SetParent(visualPivot, false);

        var spinner = projectile.GetComponentInChildren<CatSpinner>();
        spinner.SetIdle();
        spinner.ResetTransform();

        projectile.GetComponentInChildren<CatController>().SetAirNudgeControl(true);


        animationEvents.ProjectileTransform = projectile.transform;
        if (launched)
            OnProjectileReset.Invoke(projectile.gameObject);
        launched = false;
    }

    [System.Serializable]
    class BallisticCurveConfig
	{
		public float TimeStep = 0.5f;
		public int TimeStepCount = 10;
	}

    [SerializeField] BallisticCurveConfig _ballisticCurve;
    private void HandleAim()
    {
        if (!Input.Catapult.Toggle.IsPressed())
        {
            lineRenderer.enabled = false;
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.Catapult.Aim.ReadValue<Vector2>());
        aimingArmVector = (mousePosition.Truncate() - transform.position.Truncate())
            .ClampMagnitude(MaxAimingArmLength)
            .ClampConeX(AimingArmRotationRangeDegrees);

        lineRenderer.enabled = true;
		float angle = Mathf.Atan2(aimingArmVector.y, aimingArmVector.x) + Mathf.PI;
		for (int t = 1; t < _ballisticCurve.TimeStepCount - 1; ++t)
        {
            float time = t * _ballisticCurve.TimeStep;
            Vector2 pos = GetBallisticCurvePoint(transform.position.xy(), aimingArmVector.magnitude * StrengthMultiplier, angle, Physics2D.gravity.y * projectile.gravityScale, time);
            lineRenderer.SetPosition(lineRenderer.positionCount - _ballisticCurve.TimeStepCount + t + 1, pos);
        }
        Projectile.GetComponent<CatController>().PrepareShooting(aimingArmVector.magnitude / MaxAimingArmLength);
    }

    private void HandleShooting()
    {
        if (launched)
            return;
        if (Projectile == null)
            return;
        if (!Input.Catapult.Toggle.WasReleasedThisFrame())
            return;

        animationController.PlayTrebuchetFireAnim();
    }

    private void AnimationEvents_OnProjectileLaunch(object sender, System.EventArgs e)
    {
        Projectile.bodyType = RigidbodyType2D.Dynamic;
        Projectile.GetComponent<CatController>().DoShoot();
        Projectile.AddForce(-aimingArmVector * StrengthMultiplier, ForceMode2D.Impulse);

        aimingArmVector = Vector2.zero;
        lineRenderer.enabled = false;
        launched = true;

        OnProjectileLaunched.Invoke(Projectile.gameObject);
    }
}
