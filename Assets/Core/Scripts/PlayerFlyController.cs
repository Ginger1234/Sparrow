using UnityEngine;
using ui_bars;
using managers;
using interactables;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerTiredness))]
[RequireComponent(typeof(PlayerHunger))]
public class PlayerFlyController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float flightSpeed = 5f;
    [SerializeField] private float ascendSpeed = 3f;
    [SerializeField] private float descendSpeed = 3f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float rotationSpeed = 6f;

    [Header("Animation Parameters")]
    [SerializeField] private Animator animator;
    [SerializeField] private string flyParamName = "isFlying";
    [SerializeField] private string walkSpeedParamName = "walkSpeed";

    [SerializeField] private Transform holdingOffset;

    public PlayerTiredness playerTiredness { get; private set; }
    public PlayerHunger playerHunger { get; private set; }
    private CharacterController controller;

    private bool isGrounded;
    private bool isFlying;
    private float xRotation;
    private Vector3 velocity;
    private Vector3 verticalVelocity;
    private Transform groundChecker;
    private InteractableNew holding;
    private bool alive = true;
    private bool sleeping = false;

    private const float MaxVerticalRotation = 90f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerTiredness = GetComponent<PlayerTiredness>();
        playerHunger = GetComponent<PlayerHunger>();
        InitializeGroundChecker();
    }

    private void Update()
    {
        if (!alive || sleeping) return;

        CheckGroundStatus();
        HandleRotation();
        HandleMovement();
        HandleFlight();
        HandleInput();
        UpdateAnimations();
    }

    private void InitializeGroundChecker()
    {
        groundChecker = new GameObject("GroundChecker").transform;
        groundChecker.parent = transform;
        groundChecker.localPosition = Vector3.zero;
    }

    private void CheckGroundStatus()
    {
        isGrounded = Physics.CheckSphere(groundChecker.position, groundCheckDistance, LayerMask.GetMask("Ground"));
    }

    private void HandleInput()
    {
        if (!isFlying)
        {
            if (Input.GetKeyDown(KeyCode.E)) TriggerAnimation("attack");
            if (Input.GetKeyDown(KeyCode.R)) TriggerAnimation("spin");
        }
        if (Input.GetKeyDown(KeyCode.Q)) Eat();
        if (Input.GetKeyDown(KeyCode.Z)) Drop();
    }

    private void TriggerAnimation(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void Eat()
    {
        TriggerAnimation("eat");
    }

    public void Hold(InteractableNew _object)
    {
        if (holding) Drop();
        ConfigureHeldObject(_object);
        _object.transform.eulerAngles = Vector3.zero; 
        _object.transform.parent = holdingOffset;
        _object.transform.position = holdingOffset.position;
        holding = _object;
    }

    private void ConfigureHeldObject(InteractableNew _object)
    {
        Rigidbody rb = _object.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
    }

    public void Drop()
    {
        if (!holding) return;
        holding.Drop();
        holding = null;
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        transform.Rotate(Vector3.up * mouseX);
        xRotation = Mathf.Clamp(xRotation - mouseY, -MaxVerticalRotation, MaxVerticalRotation);
    }

    private void HandleMovement()
    {
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        Vector3 moveDirection = transform.TransformDirection(moveInput.normalized);
        velocity = isFlying ? moveDirection * flightSpeed : moveDirection * moveSpeed;

        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleFlight()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isFlying = true;
        }

        if (isFlying)
        {
            HandleVerticalMovement();
        }
        else
        {
            ApplyGravity();
        }

        if (isGrounded && isFlying)
        {
            isFlying = false;
        }

        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void HandleVerticalMovement()
    {
        float verticalInput = 0f;
        if (Input.GetKey(KeyCode.Space)) verticalInput += 1f;
        if (Input.GetKey(KeyCode.LeftShift)) verticalInput -= 1f;

        verticalVelocity.y = verticalInput > 0 ? verticalInput * ascendSpeed : verticalInput * descendSpeed;
    }

    private void ApplyGravity()
    {
        verticalVelocity.y += Physics.gravity.y * Time.deltaTime;
    }

    private void UpdateAnimations()
    {
        animator.SetBool(flyParamName, isFlying);

        if (!isFlying)
        {
            float speed = new Vector3(velocity.x, 0f, velocity.z).magnitude;
            animator.SetFloat(walkSpeedParamName, speed);
        }
    }

    public void Death()
    {
        animator.SetTrigger("death");
        alive = false;
    }

    public void Sleep(bool isSleeping)
    {
        animator.SetBool("sleeps", isSleeping);
        sleeping = isSleeping;
    }
}
