using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CatBoxCatcher : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private InputAction moveAction;
    public Animator Animator;
    public Transform CatcherVisuals;
    private Tweener SideTweener;

    public float MaxSpeed;
    public float Acceleration;
    public float Deceleration;
    public float AccelerationDamping;

    public UnityEvent OnCatCaught;

    public bool HasControl = true;

    private Vector2 moveInput;
    float lastInputSide;
    private float currentHorVelocity = 0;
    [SerializeField] private Rigidbody2D characterController;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        moveAction = inputActions.Player.Move;
        if(!characterController) characterController = GetComponent<Rigidbody2D>();
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
        
        float inputSide = Mathf.Sign(moveInput.x);
        float currSide = lastInputSide;

        if(inputSide != currSide && Mathf.Abs(moveInput.x) > Mathf.Epsilon)
        {
            if(SideTweener != null && SideTweener.IsActive())
            {
                SideTweener.Kill();
            }
            SideTweener = CatcherVisuals.DOScaleX(inputSide, 0.25f).SetEase(Ease.InOutQuad);

            lastInputSide = inputSide;
        }

        if(moveInput.x != 0)
        {
            Animator.SetBool("IsWalking", true);
        }
        else
        {
            Animator.SetBool("IsWalking", false);
        }        
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

        currentHorVelocity = Mathf.Clamp(currentHorVelocity, -MaxSpeed, MaxSpeed);

        characterController.MovePosition(characterController.position + Vector2.right * currentHorVelocity * Time.fixedDeltaTime);
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
