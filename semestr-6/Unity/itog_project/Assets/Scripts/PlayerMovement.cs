using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour 
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpForce = 7f;
    public float airControl = 0.5f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrain = 20f;
    public float staminaRegen = 15f;
    public float staminaRegenDelay = 2f;

    private Rigidbody rb;
    private bool isGrounded;
    private float currentStamina;
    private float lastSprintTime;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentStamina = maxStamina;
        currentSpeed = moveSpeed;

        if (groundCheck == null)
        {
            groundCheck = new GameObject("GroundCheck").transform;
            groundCheck.parent = transform;
            groundCheck.localPosition = Vector3.down * 0.9f;
        }
    }

    void Update()
    {
        HandleJumpInput();
        HandleSprintInput();
        UpdateStamina();
    }

    void FixedUpdate()
    {
        CheckGrounded();
        HandleMovement();
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized;
        float controlMultiplier = isGrounded ? 1f : airControl;

        Vector3 targetVelocity = moveDirection * currentSpeed * controlMultiplier;
        rb.linearVelocity  = new Vector3(
            targetVelocity.x, 
            rb.linearVelocity .y, 
            targetVelocity.z
        );
    }

    void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void HandleSprintInput()
    {
        bool canSprint = isGrounded && currentStamina > 0 && Time.time - lastSprintTime > 0.5f;
        
        if (Input.GetKey(KeyCode.LeftShift) && canSprint)
        {
            currentSpeed = sprintSpeed;
            currentStamina -= staminaDrain * Time.deltaTime;
            lastSprintTime = Time.time;
        }
        else
        {
            currentSpeed = moveSpeed;
        }
    }

    void UpdateStamina()
    {
        if (Time.time - lastSprintTime > staminaRegenDelay)
        {
            currentStamina = Mathf.Min(currentStamina + staminaRegen * Time.deltaTime, maxStamina);
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public float GetStaminaNormalized() => currentStamina / maxStamina;
}