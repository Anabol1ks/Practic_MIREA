using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 15f; // Скорость поворота (можно регулировать)
    public Rigidbody rb;
    public Camera mainCamera;
    public LayerMask groundMask;

    
    private Vector2 _moveInput; // Ввод движения (WASD / левый стик)
    private Vector2 _aimInput;  // Ввод прицеливания (мышь / правый стик)

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        
        // Заблокировать курсор в центре экрана (опционально)
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Получаем ввод движения (клавиши WASD)
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");
        
        // 2. Получаем направление прицеливания (мышь)
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = mainCamera.transform.position.y; // Дистанция от камеры до игрока (для Raycast)
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundMask))
        {
            Vector3 lookDir = hit.point - transform.position;
            lookDir.y = 0;

            if (lookDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }

    void FixedUpdate()
    {
        // Получаем направления камеры
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;

        // Убираем наклон по Y
        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        // Направление движения: относительно камеры
        Vector3 moveDirection = (camRight * _moveInput.x + camForward * _moveInput.y).normalized;

        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

}