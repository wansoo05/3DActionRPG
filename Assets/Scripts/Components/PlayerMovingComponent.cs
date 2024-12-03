using System;
using UnityEngine;
using UnityEngine.InputSystem;
using StateType = StateComponent.StateType;

public enum EvadeDirection
{
    Forward = 0, Backward, Left, Right,
}

public class PlayerMovingComponent : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 2.0f;

    [SerializeField]
    private float runSpeed = 4.0f;

    [SerializeField]
    private string followTargetName = "FollowTarget";

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private bool isGrounded = true;
    public bool IsGrounded { get => isGrounded; }

    [SerializeField]
    private float groundRadius = 0.28f;

    [SerializeField]
    private float groundOffset = 0.15f;

    [SerializeField]
    [Range(0.0f, 0.3f)]
    private float rotationSmoothTime = 0.12f;

    /// <summary>
    /// 점프 후 바로 Fall상태가 되지 않게 하기 위한 쿨타임
    /// </summary>
    [SerializeField]
    private float fallTimeOut = 0.15f;

    [SerializeField]
    private float jumpHeight = 1.2f;

    [SerializeField]
    public float gravity = -15.0f;

    /// <summary>
    /// 공기 저항 변수
    /// </summary>
    [SerializeField]
    public float drag = 0.0f;

    /// <summary>
    /// 점프가 끝난 후 바로 점프가 불가능하도록 하기 위한 쿨타임
    /// </summary>
    [SerializeField]
    private float jumpTimeout = 0.5f;

    private float targetRotation;
    private Vector3 targetDirection;
    private float rotationVelocity;
    private float verticalVelocity;
    public float VerticalVelocity { get => verticalVelocity; set => verticalVelocity = value; }
    private float animationBlend;
    private float fallTimeDelta;
    private float animationBlendX;
    private float animationBlendZ;
    private float jumpTimeoutDelta;
    private Vector3 externalForce = Vector3.zero;
    public Vector3 ExternalForce { set => externalForce = value; }

    private bool isJumping = false;

    private bool bCanMove = true;

    private Animator animator;
    private WeaponComponent weapon;
    private StateComponent state;
    private GhostEffectComponent ghost;
    private CharacterController controller;
    private ParkourComponent parkour;

    private Vector2 inputMove;
    private Vector2 inputLook;
    private bool bRun;
    private float speed;
    private Transform followTargetTransform;
    private InputActions input;

    public void Move()
    {
        bCanMove = true;
    }

    public void Stop()
    {
        bCanMove = false;
    }

    private void Awake()
    {
        //필요한 Component받고 관련된 이벤트 연결시키기
        animator = GetComponent<Animator>();
        weapon = GetComponent<WeaponComponent>();
        ghost = GetComponent<GhostEffectComponent>();
        state = GetComponent<StateComponent>();
        state.OnStateTypeChanged += OnStateTypeChanged;
        controller = GetComponent<CharacterController>();
        parkour = GetComponent<ParkourComponent>();
    }
    private void Start()
    {
        //커서 안보이게 하고 중앙위치에서 잠그기
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        followTargetTransform = transform.FindChildByName(followTargetName);
        speed = walkSpeed;

        input = GameManager.Instance.input;

        input.Player.Enable();

        input.Player.Move.performed += Input_Move_Performed;
        input.Player.Move.canceled += Input_Move_Cancled;

        input.Player.Look.performed += Input_Look_Performed;
        input.Player.Look.canceled += Input_Look_Cancled;

        input.Player.Run.started += Input_Run_Started;
        input.Player.Run.canceled += Input_Run_Cancled;

        input.Player.Jump.started += Input_Jump_Started;

        input.Player.Evade.started += Input_Evade_Started;
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        input.Player.Evade.started -= Input_Evade_Started;

        input.Player.Jump.started -= Input_Jump_Started;

        input.Player.Run.canceled -= Input_Run_Cancled;
        input.Player.Run.started -= Input_Run_Started;

        input.Player.Look.canceled -= Input_Look_Cancled;
        input.Player.Look.performed -= Input_Look_Performed;

        input.Player.Move.canceled -= Input_Move_Cancled;
        input.Player.Move.performed -= Input_Move_Performed;
        
        input.Player.Disable();
    }

    private void Input_Move_Performed(InputAction.CallbackContext context) => inputMove = context.ReadValue<Vector2>();

    private void Input_Move_Cancled(InputAction.CallbackContext context) => inputMove = Vector2.zero;

    private void Input_Look_Performed(InputAction.CallbackContext context) => inputLook = context.ReadValue<Vector2>();

    private void Input_Look_Cancled(InputAction.CallbackContext context) => inputLook = Vector2.zero;

    private void Input_Run_Started(InputAction.CallbackContext context) => bRun = true;

    private Vector2 velocity;

    private void Input_Run_Cancled(InputAction.CallbackContext context) => bRun = false;
    

    private void Input_Jump_Started(InputAction.CallbackContext context)
    {
        if (isGrounded == true && state.IdleMode)
        {
            isJumping = true;
        }

        if (isGrounded == false)
        {
            parkour.TryClimb();
        }
    }

    private void Input_Evade_Started(InputAction.CallbackContext context)
    {
        if (state.IdleMode == false)
            return;

        weapon.DoEvade();
    }


    private void Update()
    {
        ControllerMove();
        JumpAndGravity();
    }
    private void FixedUpdate()
    {

        Update_GroundCheck();
    }

    private void LateUpdate()
    {

    }

    /// <summary>
    /// 플레이어의 이동을 담당해주는 함수
    /// </summary>
    private void ControllerMove()
    {
        if (controller.enabled == false)
            return;

        speed = 0.0f;
        if (bCanMove)
        {
            float targetSpeed = bRun && inputMove.y > 0.0f ? runSpeed : walkSpeed;

            if (inputMove == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.fixedDeltaTime * 10.0f);

                speed = Mathf.Round(speed * 1000.0f) / 1000f;
            }
            else
            {
                speed = targetSpeed;
            }

            float blendOffset = 0.01f;
            animationBlend = Mathf.Lerp(animationBlend, targetSpeed, Time.fixedDeltaTime * 10.0f);
            if (Mathf.Abs(animationBlend - targetSpeed) < blendOffset) animationBlend = targetSpeed;

            Vector3 inputDirection = new Vector3(inputMove.x, 0.0f, inputMove.y).normalized;
            if (weapon.UnarmedMode)
            {
                if (inputMove != Vector2.zero)
                {
                    targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);

                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                    targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;
                }
                animator.SetFloat("SpeedY", animationBlend);
            }
            else
            {
                Vector3 animationTargetDirection = inputDirection * targetSpeed;
                animationBlendX = Mathf.Lerp(animationBlendX, animationTargetDirection.x, Time.deltaTime * 10.0f);
                if (Mathf.Abs(animationBlendX - animationTargetDirection.x) < blendOffset) animationBlendX = animationTargetDirection.x;
                animationBlendZ = Mathf.Lerp(animationBlendZ, animationTargetDirection.z, Time.deltaTime * 10.0f);
                if (Mathf.Abs(animationBlendZ - animationTargetDirection.z) < blendOffset) animationBlendZ = animationTargetDirection.z;

                if (inputMove != Vector2.zero)
                {
                    targetRotation = Camera.main.transform.eulerAngles.y;

                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);

                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                    targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * inputDirection;
                }
                animator.SetFloat("SpeedY", animationBlendZ);
                animator.SetFloat("SpeedX", animationBlendX);
            }
        }
        else
        {
            animator.SetFloat("SpeedX", 0.0f);
            animator.SetFloat("SpeedY", 0.0f);
        }

        if (externalForce.magnitude > 0.1f)
        {
            externalForce -= externalForce * 25.0f * Time.deltaTime;
        }
        else
        {
            externalForce = Vector3.zero;
        }

        Vector3 movement = targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime;
        //외부 충격의 방향과 움직임의 방향이 같으면 외부 충격 힘 40퍼센트로 감소
        if (Vector3.Dot(externalForce.normalized, movement.normalized) > 0 )
        {
            externalForce *= 0.3f;
        }
        movement += externalForce * Time.deltaTime;
        if (state.ClimbMode == false)
            controller.Move(movement);
    }


    private void Update_GroundCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y + groundOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePosition, groundRadius, groundLayer, QueryTriggerInteraction.Ignore);

        animator.SetBool("IsGrounded", isGrounded);
    }

    private void JumpAndGravity()
    {
        if (isGrounded)
        {
            fallTimeDelta = fallTimeOut;

            animator.SetBool("IsJump", false);
            animator.SetBool("IsFall", false);

            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2.0f;
            }

            if (isJumping && jumpTimeoutDelta <= 0.0f)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2.0f * gravity);

                animator.SetBool("IsJump", true);
                state.SetJumpMode();
            }

            if (jumpTimeoutDelta > 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
            if (state.FallingMode)
                state.SetIdleMode();
        }
        else
        {
            jumpTimeoutDelta = jumpTimeout;

            if (fallTimeDelta >= 0.0f)
            {
                fallTimeDelta -= Time.deltaTime;
            }
            else
            {
                if (state.IdleMode || state.JumpMode)
                {
                    state.SetFallingMode();
                    animator.SetBool("IsFall", true);
                }
            }

            isJumping = false;
        }
        verticalVelocity += gravity * Time.deltaTime;
        verticalVelocity -= verticalVelocity * drag * Time.deltaTime;
    }

    private void OnStateTypeChanged(StateType prevType, StateType newType)
    {
        switch (newType)
        {
            case StateType.Evade:
            {
                ghost.Play = true;
                ghost.StartCoroutine(ghost.SpawnGhosts());

                Vector2 value = inputMove;

                EvadeDirection direction = EvadeDirection.Forward;
                if (value.y == 0.0f)
                {
                    direction = EvadeDirection.Forward;

                    if (value.x < 0.0f)
                        direction = EvadeDirection.Left;
                    else if (value.x > 0.0f)
                        direction = EvadeDirection.Right;
                }
                else if (value.y >= 0.0f)
                {
                    direction = EvadeDirection.Forward;
                }
                else
                {
                    direction = EvadeDirection.Backward;
                }

                animator.SetInteger("Direction", (int)direction);
                animator.SetTrigger("Evade");
            }
            return;  
        }
    }

    private void End_Evade()
    {
        state.SetIdleMode();
        ghost.Play = false;
    }


    private void Begin_Land(AnimationEvent animationEvent)
    {
        state.SetLandMode();
        Stop();

        string name = "Footsteps_";
        string groundName = GroundHelpers.GetGroundName(transform);
        name += groundName;
        name += "_Land_";
        name += SoundManager.Instance.SoundRandomRange(1, 3);

        if (groundName == null)
            return;

        SoundManager.Instance.PlaySound(name, SoundType.Effect, transform);

    }

    private void End_Land()
    {
        state.SetIdleMode();
        Move();
    }
}