using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // ------------- Настройки наземного движения -------------
    [Header("Наземное движение")]
    [SerializeField] private float walkSpeed = 5f;           // Скорость ходьбы
    [SerializeField] private float runSpeed = 10f;           // Скорость бега (при зажатом Shift)
    [SerializeField] private float jumpForce = 7f;           // Сила прыжка
    [SerializeField] private float airControl = 0.5f;        // Коэффициент контроля в воздухе

    // ------------- Настройки полёта -------------
    [Header("Полёт")]
    [SerializeField] private float flyHoverForce = 9.8f;      // Сила парения (если понадобится)
    [SerializeField] private float flyAcceleration = 15f;     // Ускорение для изменения скорости полёта
    [SerializeField] private float maxFlySpeed = 25f;         // Максимальная скорость полёта
    [SerializeField] private float flightDrag = 2f;           // Сопротивление воздуха в полёте
    [SerializeField] private float pitchSpeed = 100f;         // Скорость наклона (pitch)
    [SerializeField] private float yawSpeed = 100f;           // Скорость поворота по оси Y (yaw)
    [SerializeField] private float rollSpeed = 100f;          // Скорость крена (roll)
    [SerializeField] private float stability = 10f;           // Стабилизация (при отсутствии ввода)
    [SerializeField] private float flightBoostMultiplier = 1.5f; // Множитель ускорения при Shift
    [SerializeField] private KeyCode flightToggleKey = KeyCode.F; // Клавиша переключения режима полёта

    // ------------- Настройки анимации -------------
    [Header("Анимация")]
    [SerializeField] private Animator animator;             // Ссылка на Animator персонажа

    private Rigidbody rb;
    private bool isFlying = false;
    private bool isGrounded = false;
    private float originalDrag;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Сохраняем исходное значение сопротивления для наземного режима
        originalDrag = rb.linearDamping;
        // В режиме ходьбы вращение контролируем скриптом, физика его не должна менять
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleJump();
        HandleFlightToggle();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isFlying)
        {
            HandleFlight();
        }
        else
        {
            HandleGroundMovement();
        }
    }

    // ------------------- Наземное движение -------------------
    void HandleGroundMovement()
    {
        // Чтение ввода для перемещения
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveInput = transform.right * horizontal + transform.forward * vertical;

        // Выбор скорости (бег или ходьба)
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        Vector3 desiredVelocity = moveInput * speed;
        // Сохраняем вертикальную составляющую (например, от прыжка)
        desiredVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = desiredVelocity;

        // Если персонаж не на земле, добавляем дополнительное управление
        if (!isGrounded)
        {
            rb.AddForce(moveInput * speed * airControl, ForceMode.Acceleration);
        }
    }

    // ------------------- Полёт -------------------
    void HandleFlight()
    {
        // В полёте отключаем гравитацию и устанавливаем воздушное сопротивление
        rb.linearDamping = flightDrag;
        rb.angularDamping = flightDrag * 2f;

        // Чтение клавиатурного ввода для перемещения
        float horizontal = Input.GetAxis("Horizontal");  // перемещение вправо/влево
        float vertical = Input.GetAxis("Vertical");        // перемещение вперёд/назад
        // Вверх и вниз – по пробелу и Ctrl соответственно
        float ascend = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        float descend = Input.GetKey(KeyCode.LeftControl) ? -1f : 0f;
        float verticalInput = ascend + descend;

        // Чтение ввода мыши для поворота (наклон и рысканье)
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y"); // Инвертировано для естественного ощущения
        // Дополнительно: клавиши Q/E для крена
        float rollInput = 0f;
        if (Input.GetKey(KeyCode.Q))
            rollInput = rollSpeed * Time.fixedDeltaTime;
        else if (Input.GetKey(KeyCode.E))
            rollInput = -rollSpeed * Time.fixedDeltaTime;

        // Расчёт изменения вращения
        Quaternion deltaRotation = Quaternion.Euler(
            mouseY * pitchSpeed * Time.fixedDeltaTime,
            mouseX * yawSpeed * Time.fixedDeltaTime,
            rollInput
        );
        // Плавно обновляем вращение Rigidbody
        rb.MoveRotation(rb.rotation * deltaRotation);

        // Определяем вектор направления полёта на основе текущей ориентации
        Vector3 flightDirection = (transform.forward * vertical) +
                                  (transform.right * horizontal) +
                                  (transform.up * verticalInput);
        if (flightDirection.sqrMagnitude > 0.01f)
            flightDirection.Normalize();

        // Проверка буста (Shift)
        float boost = Input.GetKey(KeyCode.LeftShift) ? flightBoostMultiplier : 1f;
        Vector3 targetVelocity = flightDirection * maxFlySpeed * boost;

        // Расчёт ускорения для достижения целевой скорости
        Vector3 velocityDiff = targetVelocity - rb.linearVelocity;
        Vector3 acceleration = velocityDiff * flyAcceleration;
        rb.AddForce(acceleration, ForceMode.Acceleration);

        // Стабилизация: если ввода почти нет, снижаем угловую скорость и корректируем ориентацию
        if (flightDirection.sqrMagnitude < 0.1f)
        {
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, stability * Time.fixedDeltaTime);
            Vector3 currentUp = transform.up;
            Vector3 torque = Vector3.Cross(currentUp, Vector3.up) * stability;
            rb.AddTorque(torque, ForceMode.Acceleration);
        }
    }

    // ------------------- Прыжок -------------------
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
        if (Input.GetKeyDown(flightToggleKey))
        {
            isFlying = !isFlying;
            rb.useGravity = !isFlying;
            rb.linearDamping = isFlying ? flightDrag : originalDrag;

            if (isFlying)
            {
                // В полёте разрешаем свободное вращение
                rb.freezeRotation = false;
                rb.constraints = RigidbodyConstraints.None;
            }
            else
            {
                // При возвращении на землю блокируем вращение (контролируется скриптом)
                rb.freezeRotation = true;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.angularVelocity = Vector3.zero;
                // Сохраняем только горизонтальную составляющую скорости
                Vector3 vel = rb.linearVelocity;
                vel.y = 0;
                rb.linearVelocity = vel;
            }
        }
    }

    // ------------------- Обновление анимаций -------------------
    void UpdateAnimations()
    {
        if (animator)
        {
            animator.SetFloat("Speed", rb.linearVelocity.magnitude);
            animator.SetBool("isGrounded", isGrounded);
            animator.SetBool("isFlying", isFlying);
        }
    }

    // ------------------- Обработка столкновений -------------------
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
