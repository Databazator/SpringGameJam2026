using UnityEngine;

/// <summary>
/// Controller of shooter game object. Shooter would probbaly be a catapult or a trebuchet.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class ShooterController : MonoBehaviour
{
    private InputSystem_Actions Input;
    private LineRenderer lineRenderer;
    /// <summary>
    /// Projectile which could be shot. Projectile does not get automatically reloaded.
    /// </summary>
    [field: SerializeField]
    private Rigidbody2D projectile;

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
            projectile.bodyType = RigidbodyType2D.Kinematic;
            projectile.linearVelocity = Vector2.zero;
            projectile.angularVelocity = 0.0f;
        }
    }

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

    private void Awake()
    {
        Input = new InputSystem_Actions();
        Input.Enable();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);

        // setter enforces desired physical properties
        Projectile = projectile;
    }

    private void Update()
    {
        HandleAim();
        HandleShooting();
    }

    private void HandleAim()
    {
        if (!Input.Catapult.Toggle.IsPressed())
            return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.Catapult.Aim.ReadValue<Vector2>());
        aimingArmVector = mousePosition.Truncate() - transform.position.Truncate();
        aimingArmVector = aimingArmVector.ClampMagnitude(MaxAimingArmLength);
        aimingArmVector = aimingArmVector.ClampConeX(AimingArmRotationRangeDegrees);

        lineRenderer.SetPosition(1, transform.position.Truncate() + aimingArmVector);
    }

    private void HandleShooting()
    {
        if (Projectile == null)
            return;
        if (!Input.Catapult.Toggle.WasReleasedThisFrame())
            return;

        Projectile.bodyType = RigidbodyType2D.Dynamic;
        Projectile.AddForce(-aimingArmVector * StrengthMultiplier, ForceMode2D.Impulse);

        aimingArmVector = Vector2.zero;
        lineRenderer.SetPosition(1, transform.position.Truncate());
    }
}
