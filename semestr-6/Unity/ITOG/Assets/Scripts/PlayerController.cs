using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerItog : MonoBehaviour
{
    // ------------------- Параметры движения -------------------
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f; // Скорость ходьбы персонажа (единиц в секунду)
    [SerializeField] private float runSpeed = 10f; // Скорость бега (при зажатом Shift)
    [SerializeField] private float jumpForce = 7f; // Сила прыжка (импульс вверх)
    [SerializeField] private float airControl = 0.5f; // Коэффициент контроля в воздухе (чем выше – тем более отзывчиво)

    // ------------------- Параметры полёта -------------------
    [Header("Flight Settings")]
    [SerializeField] private float flyHoverForce = 9.8f; // Сила парения, компенсирующая гравитацию
    [SerializeField] private float flyAcceleration = 15f; // Ускорение при изменении направления полёта
    [SerializeField] private float maxFlySpeed = 25f; // Максимальная скорость полёта
    [SerializeField] private float flightDrag = 2f; // Сопротивление воздуха при полёте (замедление)
    [SerializeField] private float pitchSpeed = 100f; // Скорость наклона (pitch) при поворотах
    [SerializeField] private float rollSpeed = 100f; // Скорость крена (roll) при поворотах
    [SerializeField] private float yawSpeed = 100f; // Скорость рыскания (yaw) при поворотах
    [SerializeField] private KeyCode flyKey = KeyCode.F; // Клавиша для переключения режима полёта
    [SerializeField] private float stability = 10f; // Параметр стабилизации: скорость возвращения к исходной ориентации
    [SerializeField] private float flightBoostMultiplier = 1.5f; // Множитель ускорения в полёте при зажатом Shift

    private Rigidbody rb;
    private bool isFlying = false;
    private bool isGrounded;
    private float originalDrag;
    private Vector3 flightInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // При обычном движении блокируем вращение физикой
        rb.freezeRotation = true;
        originalDrag = rb.linearDamping;
    }

    void Update()
    {
        HandleJump();
        HandleFlightToggle();
    }

    void FixedUpdate()
    {
        if (isFlying)
            HandleFlight();
        else
            HandleGroundMovement();
    }

    // ------------------- Управление на земле -------------------
    void HandleGroundMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        // Если зажат Shift – бежим, иначе ходим
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        if (isGrounded)
        {
            // Сохраняем вертикальную скорость (например, для прыжков)
            Vector3 targetVelocity = move * speed;
            targetVelocity.y = rb.linearVelocity.y;
            rb.linearVelocity = targetVelocity;
        }
        else
        {
            // Воздушное управление – плавное изменение скорости
            rb.AddForce(move * speed * airControl, ForceMode.Acceleration);
        }
    }

    // ------------------- Управление полётом -------------------
    void HandleFlight()
    {
        // Считываем ввод для движения по трём осям
        flightInput = new Vector3(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"),
            Input.GetKey(KeyCode.Space) ? 1f : (Input.GetKey(KeyCode.LeftControl) ? -1f : 0f)
        );

        // Применяем сопротивление для плавности
        rb.linearDamping = flightDrag;
        rb.angularDamping = flightDrag * 2;

        // Поддержание высоты (парение)
        rb.AddForce(Vector3.up * flyHoverForce, ForceMode.Acceleration);

        // Если зажат Shift, увеличиваем скорость полёта
        float boost = Input.GetKey(KeyCode.LeftShift) ? flightBoostMultiplier : 1f;
        // Расчет целевой скорости на основе направления и максимальной скорости полёта с учетом ускорения
        Vector3 targetVelocity = (transform.forward * flightInput.y +
                                  transform.right * flightInput.x +
                                  transform.up * flightInput.z) * maxFlySpeed * boost;

        // Вычисляем ускорение для достижения целевой скорости
        Vector3 acceleration = (targetVelocity - rb.linearVelocity) * flyAcceleration;
        rb.AddForce(acceleration, ForceMode.Acceleration);

        // Обработка поворотов с использованием мыши и клавиш Q/E для крена
        float pitch = Input.GetAxis("Mouse Y") * pitchSpeed * Time.fixedDeltaTime;
        float yaw = Input.GetAxis("Mouse X") * yawSpeed * Time.fixedDeltaTime;
        float roll = Input.GetKey(KeyCode.Q) ? rollSpeed * Time.fixedDeltaTime :
                     Input.GetKey(KeyCode.E) ? -rollSpeed * Time.fixedDeltaTime : 0f;
        Quaternion rotation = Quaternion.Euler(pitch, yaw, roll);
        rb.MoveRotation(rb.rotation * rotation);

        // Стабилизация: если нет ввода – корректируем вращение
        if (flightInput.magnitude < 0.1f)
        {
            // Плавно снижаем угловую скорость, чтобы прекратить кручение
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, Time.fixedDeltaTime * stability);

            // Вычисляем вектор для выравнивания "верха" с мировым вверх
            Vector3 currentUp = transform.up;
            Vector3 correctionTorque = Vector3.Cross(currentUp, Vector3.up) * stability;
            rb.AddTorque(correctionTorque, ForceMode.Acceleration);
        }
    }

    // ------------------- Обработка прыжка -------------------
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isFlying)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // ------------------- Переключение режима полёта -------------------
    void HandleFlightToggle()
    {
        if (Input.GetKeyDown(flyKey))
        {
            isFlying = !isFlying;
            rb.useGravity = !isFlying;
            rb.linearDamping = isFlying ? flightDrag : originalDrag;
            // В режиме полёта убираем ограничения вращения
            rb.constraints = isFlying ? RigidbodyConstraints.None : RigidbodyConstraints.FreezeRotation;

            if (!isFlying)
            {
                // При отключении полёта сбрасываем угловую скорость и сохраняем горизонтальную скорость
                rb.angularVelocity = Vector3.zero;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            }
        }
    }

    // ------------------- Обработка столкновений для определения соприкосновения с землей -------------------
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
