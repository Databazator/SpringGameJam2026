using UnityEngine;
using UnityEngine.InputSystem;

public class CatBoxCatcher : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private InputAction moveAction;

    public float MaxSpeed;
    public float Acceleration;
    public float Deceleration;
    public float AccelerationDamping;

    public bool HasControl = true;

    private Vector2 moveInput;
    private float currentHorVelocity = 0;
    [SerializeField] private CharacterController characterController;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        moveAction = inputActions.Player.Move;
        if(!characterController) characterController = GetComponent<CharacterController>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

        if(HasControl)
        {
            HandleMovement();
        }
    }

    void HandleMovement()
    {
        float horMove = moveInput.x;
        float moveSign = Mathf.Sign(horMove);
        bool stopping = Mathf.Approximately(horMove, 0f);

        if(!stopping)
        {
            currentHorVelocity += moveSign * Acceleration * Time.deltaTime;            
        }
        else
        {
            currentHorVelocity -= Mathf.Min(currentHorVelocity, Mathf.Sign(currentHorVelocity) * Deceleration * Time.deltaTime);
        }

        characterController.Move(new Vector2(currentHorVelocity * Time.deltaTime, 0f));
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
