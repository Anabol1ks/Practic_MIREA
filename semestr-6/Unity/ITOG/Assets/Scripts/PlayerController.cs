using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerItog : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float airControl = 0.5f;

    [Header("Flight")]
    [SerializeField] private float flySpeed = 15f;
    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private KeyCode flyKey = KeyCode.F;

    private Rigidbody rb;
    private bool isFlying = false;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleJump();
        HandleFlightToggle();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        // Горизонтальное управление
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        if (isFlying)
        {
            // Отключаем гравитацию при полёте
            rb.useGravity = false;

            // Добавляем вертикальное управление:
            float verticalInput = 0f;
            if (Input.GetButton("Jump")) // подъем
                verticalInput = 1f;
            else if (Input.GetKey(KeyCode.LeftControl)) // спуск (можно изменить клавишу по желанию)
                verticalInput = -1f;

            // Формируем вектор движения с учётом вертикальной компоненты
            Vector3 flightMove = move + Vector3.up * verticalInput;
            // Нормализуем, чтобы избежать увеличения скорости при диагональном движении
            flightMove = flightMove.normalized;

            // Устанавливаем скорость
            rb.linearVelocity = flightMove * (Input.GetKey(KeyCode.LeftShift) ? dashSpeed : flySpeed);
        }
        else
        {
            rb.useGravity = true;
            if (isGrounded)
            {
                // Движение по земле – горизонтальная скорость сохраняется, вертикальная берется из текущей скорости
                rb.linearVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
            }
            else
            {
                // В воздухе добавляем силу для коррекции движения
                rb.AddForce(move * speed * airControl, ForceMode.Acceleration);
            }
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isFlying)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleFlightToggle()
    {
        if (Input.GetKeyDown(flyKey))
        {
            isFlying = !isFlying;
            rb.linearVelocity = Vector3.zero; // Сброс скорости при переключении
            Debug.Log("Режим полёта: " + isFlying);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("На земле");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("В воздухе");
        }
    }
}
