using DG.Tweening;
using System.Collections.Generic;
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
    private Tweener ChestSideTweener;
    public float ChangeHeadingDuration = 0.25f;
    public float ClipSpeedAtMaxSpeed;  

    [Header("Movement")]
    public float MaxSpeed;
    public float Acceleration;
    public float Deceleration;
    public float AccelerationDamping;

    public UnityEvent OnCatCaught;

    public bool HasControl = true;

    [Header("BoxHitbox")]
    public Transform ChestHitboxTransform;
    public Transform ChestHitboxPositionTrack;
    private float startChestHitboxRotation;
    public float AnchorOffset = 5.6f;

    private Vector2 moveInput;
    float lastInputSide;
    private float currentHorVelocity = 0;
    [SerializeField] private Rigidbody2D characterController;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        moveAction = inputActions.Player.Move;
        inputActions.Disable();

        if(!characterController) characterController = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
        startChestHitboxRotation = ChestHitboxTransform.localEulerAngles.z;
    }

    public void CatCaught(GameObject cat)
    {
        Debug.Log($"Object caught in box: {cat.name}");
        OnCatCaught.Invoke();


        //Add spring joint to cat so it stays in the box :3
        Rigidbody2D catRb = cat.GetComponent<Rigidbody2D>();
        int newLayer = LayerMask.NameToLayer("BoxedCat");
        var children = cat.GetComponentsInChildren<Transform>();
        foreach(var chld in children)
        {
            chld.gameObject.layer = newLayer;
        }

        //cat.transform.DOScale(cat.transform.localScale * 0.7f, 0.5f).SetEase(Ease.InOutQuad);
        if (!catRb)
        {
            Debug.Log($"Cat {cat.name} doesnt have a rigidbody2D");
            return;
        }
        SpringJoint2D spring = this.AddComponent<SpringJoint2D>();

        spring.anchor = new Vector2(0, AnchorOffset);
        spring.connectedBody = catRb;
        spring.autoConfigureDistance = false;
        spring.distance = Vector3.Distance(transform.position + Vector3.up * AnchorOffset, cat.transform.position);
        spring.enableCollision = true;
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
            SideTweener = CatcherVisuals.DOScaleX(inputSide, ChangeHeadingDuration).SetEase(Ease.InOutQuad);

            //if(ChestSideTweener != null && ChestSideTweener.IsActive())
            //{
            //    ChestSideTweener.Kill();
            //}
            //ChestSideTweener = ChestHitboxTransform.DOLocalRotate(new Vector3(0f, 0f, startChestHitboxRotation * inputSide), ChangeHeadingDuration, RotateMode.Fast);
            ChestHitboxTransform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, startChestHitboxRotation * inputSide));
            lastInputSide = inputSide;
        }

        float speedFactor = Mathf.Abs(currentHorVelocity) / MaxSpeed;

        if (Mathf.Abs(currentHorVelocity) > 0f)
        {
            Animator.SetBool("IsWalking", true);

            Animator.speed = Mathf.Lerp(1f, ClipSpeedAtMaxSpeed, speedFactor);
        }
        else
        {
            Animator.SetBool("IsWalking", false);

            Animator.speed = 1f;
        }

        //update chest hitbox
        ChestHitboxTransform.position = ChestHitboxPositionTrack.position;
    }

    /// <summary>
    /// Enable input control of the catcher.
    /// </summary>
    public void EnableInput()
    {
        inputActions.Enable();
    }

    /// <summary>
    /// Disable input control of the catcher.
    /// </summary>
    public void DisableInput()
    {
        inputActions.Disable();
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
