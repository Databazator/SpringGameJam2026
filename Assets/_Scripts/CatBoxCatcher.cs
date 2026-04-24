using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CatBoxCatcher : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private InputAction moveAction;

    public float MaxSpeed;
    public float Acceleration;
    public float Deceleration;
    public float AccelerationDamping;

    public UnityEvent OnCatCaught;

    public bool HasControl = true;

    private Vector2 moveInput;
    private float currentHorVelocity = 0;
    [SerializeField] private Rigidbody characterController;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        moveAction = inputActions.Player.Move;
        if(!characterController) characterController = GetComponent<Rigidbody>();
    }
    void Start()
    {
        
    }

    public void CatCaught(GameObject cat)
    {
        Debug.Log($"Object caught in box: {cat.name}");
        OnCatCaught.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();       
    }

    void HandleMovement()
    {
        float horMove = moveInput.x;
        float moveSign = Mathf.Sign(horMove);
        bool stopping = Mathf.Approximately(horMove, 0f);

        if(!stopping)
        {
            currentHorVelocity += moveSign * Acceleration * Time.fixedDeltaTime;            
        }
        else
        {
            float deceleration = Mathf.Sign(currentHorVelocity) * Deceleration * Time.fixedDeltaTime;
            if(Mathf.Sign(deceleration) > 0)
            {
                currentHorVelocity -= Mathf.Min(currentHorVelocity, deceleration); 
            }
            else
            {
                currentHorVelocity -= Mathf.Max(currentHorVelocity, deceleration);
            }            
        }

        characterController.MovePosition(characterController.position + Vector3.right * currentHorVelocity * Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        if (HasControl)
        {
            HandleMovement();
        }
    }

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }
}
