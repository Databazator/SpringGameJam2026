using EasyButtons;
using UnityEngine;

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
        Input = new InputSystem_Actions();
        Input.Enable();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position.WithZ(lineZ));

        ResetProjectile();
    }

    private void Update()
    {
        HandleAim();
        HandleShooting();
    }

    /// <summary>
    /// Resets projectile to shooter's starting point and reset it's physical properties.
    /// </summary>
    [Button]
    public void ResetProjectile()
    {
        projectile.bodyType = RigidbodyType2D.Kinematic;
        projectile.linearVelocity = Vector2.zero;
        projectile.angularVelocity = 0.0f;
        projectile.transform.position = transform.position.WithZ(Projectile.transform.position.z);
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
            return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.Catapult.Aim.ReadValue<Vector2>());
        aimingArmVector = (mousePosition.Truncate() - transform.position.Truncate())
            .ClampMagnitude(MaxAimingArmLength);
            //.ClampConeX(AimingArmRotationRangeDegrees);

        //Vector2 endPointPosition = transform.position.Truncate() + aimingArmVector;
        //lineRenderer.SetPosition(1, endPointPosition.Extend(lineZ));
        lineRenderer.positionCount = _ballisticCurve.TimeStepCount;
		float angle = Mathf.Atan2(aimingArmVector.y, aimingArmVector.x) + Mathf.PI;
        Debug.Log($"Aiming arm: {aimingArmVector}", this);
		for (int t=1; t < _ballisticCurve.TimeStepCount; ++t)
        {
            float time = t * _ballisticCurve.TimeStep;
            Vector2 pos = GetBallisticCurvePoint(transform.position.Truncate(), aimingArmVector.magnitude * StrengthMultiplier, angle, Physics2D.gravity.y * projectile.gravityScale, time);
            lineRenderer.SetPosition(t, pos);
        }
    }

    private void HandleShooting()
    {
        if (launched)
            return;
        if (Projectile == null)
            return;
        if (!Input.Catapult.Toggle.WasReleasedThisFrame())
            return;

		Debug.Log($"SHOOT - Aiming arm: {aimingArmVector}", this);
		Projectile.bodyType = RigidbodyType2D.Dynamic;
        Projectile.AddForce(-aimingArmVector * StrengthMultiplier, ForceMode2D.Impulse);

        aimingArmVector = Vector2.zero;
        lineRenderer.SetPosition(1, transform.position.Truncate());
        launched = true;
    }
}
