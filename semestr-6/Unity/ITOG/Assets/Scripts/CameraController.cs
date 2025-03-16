using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Настройки камеры")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    
    [Header("Параметры третьего лица")]
    public Vector3 thirdPersonOffset = new Vector3(0, 1.5f, -5f);
    public float cameraCollisionRadius = 0.3f;
    public float minDistance = 1f;
    public LayerMask collisionMask;
    public float cameraSmoothness = 5f;
    public Vector2 verticalClamp = new Vector2(-40, 80);

    private float xRotation = 0f;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private PlayerController playerController;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (playerBody == null)
            Debug.LogError("Назначьте playerBody в инспекторе!");

        playerController = playerBody.GetComponent<PlayerController>();
    }

    void Update()
    {
        if (playerController != null && playerController.IsFlying)
        {
            // === В полёте ===
            HandleFlightCamera();
        }
        else
        {
            // === На земле (или не летаем) ===
            HandleCameraMovement();
        }
    }

    // ------------------- Обычный режим камеры (крутится от мыши) -------------------
    void HandleCameraMovement()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Вращаем игрока по горизонтали (если нужно)
        playerBody.Rotate(Vector3.up * mouseX);

        // Управляем наклоном камеры по вертикали
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, verticalClamp.x, verticalClamp.y);

        // Вычисляем итоговый поворот
        Quaternion rotation = Quaternion.Euler(xRotation, playerBody.eulerAngles.y, 0);

        // Позиция с учётом offset
        Vector3 offset = rotation * thirdPersonOffset;
        targetPosition = playerBody.position + offset;

        // Проверка коллизий (чтобы камера не заходила в стены)
        HandleCameraCollision(ref targetPosition);

        // Плавно перемещаем камеру
        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSmoothness * Time.deltaTime);

        // Смотрим на игрока
        Vector3 lookDirection = playerBody.position - transform.position;
        targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraSmoothness * Time.deltaTime);
    }

    // ------------------- Камера в полёте: копирует поворот игрока и просто отстаёт -------------------
    void HandleFlightCamera()
    {
        // Берём тот же поворот, что у игрока
        Quaternion newCamRot = playerBody.rotation;

        // Смещение в локальных координатах игрока
        Vector3 offsetPos = newCamRot * thirdPersonOffset;
        targetPosition = playerBody.position + offsetPos;
        targetRotation = newCamRot;

        // Проверка коллизий
        HandleCameraCollision(ref targetPosition);

        // Плавное движение
        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSmoothness * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraSmoothness * Time.deltaTime);
    }

    // ------------------- Проверка коллизий камеры -------------------
    void HandleCameraCollision(ref Vector3 targetPos)
    {
        RaycastHit hit;
        Vector3 direction = targetPos - playerBody.position;
        float distance = direction.magnitude;

        if (Physics.SphereCast(
            playerBody.position,
            cameraCollisionRadius,
            direction.normalized,
            out hit,
            distance,
            collisionMask))
        {
            // Сдвигаем позицию камеры чуть ближе, чтобы не проходить сквозь стены
            targetPos = hit.point - direction.normalized * 0.2f;
            targetPos = Vector3.Lerp(playerBody.position, targetPos, minDistance / distance);
        }
    }
}
