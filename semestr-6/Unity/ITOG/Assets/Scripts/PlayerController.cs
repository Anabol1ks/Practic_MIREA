using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // ------------- Настройки наземного движения -------------
    [Header("Наземное движение")]
    [SerializeField] private float walkSpeed = 5f;           
    [SerializeField] private float runSpeed = 10f;           
    [SerializeField] private float jumpForce = 7f;           
    [SerializeField] private float airControl = 0.5f;        

    // ------------- Настройки полёта -------------
    [Header("Полёт")]
    [SerializeField] private float pitchSpeed = 100f;         // Скорость наклона (pitch) по мыши
    [SerializeField] private float yawSpeed = 100f;           // Скорость поворота (yaw) по мыши
    [SerializeField] private float rollSpeed = 100f;          // Скорость крена (roll) по Q/E
    [SerializeField] private float flyAcceleration = 15f;     
    [SerializeField] private float maxFlySpeed = 25f;         
    [SerializeField] private float flightDrag = 2f;           
    [SerializeField] private float stability = 10f;           
    [SerializeField] private float flightBoostMultiplier = 1.5f; 
    [SerializeField] private KeyCode flightToggleKey = KeyCode.F; 

    // Новые параметры для автоматического выравнивания
    [Header("Автовыравнивание (полёт)")]
    [SerializeField] private float levelingSpeed = 2f;       // Сила выравнивания (крутящий момент)
    [SerializeField] private float levelingDamping = 1f;     // Коэффициент затухания угловой скорости

    // ------------- Настройки анимации -------------
    [Header("Анимация")]
    [SerializeField] private Animator animator;             

    private Rigidbody rb;
    private bool isFlying = false;
    private bool isGrounded = false;
    private float originalDrag;

    // Публичное свойство для проверки режима полёта (может использоваться в CameraController)
    public bool IsFlying { get { return isFlying; } }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalDrag = rb.linearDamping;
        // На земле вращение контролируем сами, поэтому замораживаем поворот физикой
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleJump();
        HandleFlightToggle();

        // === ВАЖНО для полёта: вращение от мыши и Q/E ===
        if (isFlying)
        {
            HandleMouseFlightRotation();
        }

        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isFlying)
            HandleFlightMovement();
        else
            HandleGroundMovement();
    }

    // ------------------- Наземное движение -------------------
    void HandleGroundMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveInput = transform.right * horizontal + transform.forward * vertical;

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        Vector3 desiredVelocity = moveInput * speed;
        desiredVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = desiredVelocity;

        if (!isGrounded)
            rb.AddForce(moveInput * speed * airControl, ForceMode.Acceleration);
    }

    // ------------------- Полёт: вращение от мыши (pitch, yaw) + Q/E (roll) -------------------
    void HandleMouseFlightRotation()
    {
        // Считываем мышь
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y"); // Инвертируем Y для «самолётного» ощущения

        // Считываем крен по Q/E
        float rollInput = 0f;
        if (Input.GetKey(KeyCode.Q))
            rollInput = rollSpeed * Time.deltaTime;
        else if (Input.GetKey(KeyCode.E))
            rollInput = -rollSpeed * Time.deltaTime;

        // Формируем приращение поворота
        Quaternion deltaRotation = Quaternion.Euler(
            mouseY * pitchSpeed * Time.deltaTime,   // pitch
            mouseX * yawSpeed * Time.deltaTime,     // yaw
            rollInput                               // roll
        );

        // Применяем к Rigidbody
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    // ------------------- Полёт: перемещение и стабилизация -------------------
    void HandleFlightMovement()
    {
        // Настраиваем сопротивление в полёте
        rb.linearDamping = flightDrag;
        rb.angularDamping = flightDrag * 2f;

        // Чтение ввода для перемещения вперёд/назад, влево/вправо, вверх/вниз
        float horizontal = Input.GetAxis("Horizontal");  
        float vertical = Input.GetAxis("Vertical");        
        float ascend = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        float descend = Input.GetKey(KeyCode.LeftControl) ? -1f : 0f;
        float verticalInput = ascend + descend;

        // Вектор направления полёта (на основе текущего поворота персонажа)
        Vector3 flightDirection = (transform.forward * vertical) +
                                  (transform.right   * horizontal) +
                                  (transform.up      * verticalInput);
        if (flightDirection.sqrMagnitude > 0.01f)
            flightDirection.Normalize();

        // Буст (Shift)
        float boost = Input.GetKey(KeyCode.LeftShift) ? flightBoostMultiplier : 1f;
        Vector3 targetVelocity = flightDirection * maxFlySpeed * boost;
        Vector3 velocityDiff = targetVelocity - rb.linearVelocity;
        Vector3 acceleration = velocityDiff * flyAcceleration;
        rb.AddForce(acceleration, ForceMode.Acceleration);

        // Применяем мягкое автовыравнивание
        ApplyAutoLeveling();
    }

    // ------------------- Метод для автоматического выравнивания -------------------
    void ApplyAutoLeveling()
    {
        // Если игрок активно управляет (движение мышью), снижаем влияние стабилизации.
        // Берём максимальное значение по осям ввода мыши.
        float inputIntensity = Mathf.Max(Mathf.Abs(Input.GetAxis("Mouse X")), Mathf.Abs(Input.GetAxis("Mouse Y")));
        // Чем меньше ввода, тем больше влияние стабилизации (предел 1)
        float levelingFactor = Mathf.Clamp01(1f - inputIntensity);

        // Плавное затухание угловой скорости
        rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, levelingDamping * levelingFactor * Time.fixedDeltaTime);

        // Вычисляем крутящий момент для выравнивания: стремимся, чтобы transform.up совпадал с Vector3.up
        Vector3 torque = Vector3.Cross(transform.up, Vector3.up) * levelingSpeed * levelingFactor;
        rb.AddTorque(torque, ForceMode.Acceleration);
    }

    // ------------------- Прыжок -------------------
    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isFlying)
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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
                // В полёте разрешаем вращение
                rb.freezeRotation = false;
                rb.constraints = RigidbodyConstraints.None;
            }
            else
            {
                // Возвращаемся на землю
                rb.freezeRotation = true;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                rb.angularVelocity = Vector3.zero;
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
            if (Input.GetButtonDown("Jump") && isGrounded && !isFlying)
                animator.SetTrigger("JumpTrigger");
        }
    }

    // ------------------- Обработка столкновений -------------------
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    
}
