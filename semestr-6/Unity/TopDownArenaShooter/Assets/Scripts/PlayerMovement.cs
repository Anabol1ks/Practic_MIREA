using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float mouseSensitivity = 2f; // Чувствительность мыши
    public Rigidbody rb;
    public Camera mainCamera;
    public LayerMask groundMask;

    private Vector2 _moveInput;
    private float _rotationX = 0f; // Текущий поворот по горизонтали
    private Vector3 _moveDirection;
    private bool _isMoving;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        // Настраиваем Rigidbody для более стабильного движения
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
        Cursor.lockState = CursorLockMode.Locked; // Блокируем курсор в центре экрана
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovementInput();
        HandleCameraRotation();
    }

    void HandleMovementInput()
    {
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");
        _isMoving = _moveInput.magnitude > 0.1f;
    }

    void HandleCameraRotation()
    {
        // Получаем ввод мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        
        // Поворачиваем персонажа по горизонтали
        _rotationX += mouseX;
        transform.rotation = Quaternion.Euler(0, _rotationX, 0);
    }

    void FixedUpdate()
    {
        if (!_isMoving) return;

        // Получаем направления камеры
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        _moveDirection = (camRight * _moveInput.x + camForward * _moveInput.y).normalized;
        
        // Используем MovePosition для более плавного движения
        Vector3 newPosition = rb.position + _moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}