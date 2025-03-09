using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Настройки камеры")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public bool isThirdPerson = false;
    public KeyCode toggleKey = KeyCode.V;

    [Header("Параметры первого лица")]
    public Transform headTransform;

    [Header("Параметры третьего лица")]
    public Vector3 thirdPersonOffset = new Vector3(0, 1.5f, -5f);
    public float cameraCollisionRadius = 0.3f;
    public float minDistance = 1f;
    public LayerMask collisionMask;
    public float cameraSmoothness = 5f;
    public Vector2 verticalClamp = new Vector2(-40, 80);

    private float xRotation = 0f;
    private float yRotation = 0f;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (playerBody == null) Debug.LogError("Назначьте playerBody в инспекторе!");
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isThirdPerson = !isThirdPerson;
        }

        if (!isThirdPerson)
        {
            FirstPersonLogic();
        }
        else
        {
            ThirdPersonLogic();
        }
    }

    void FirstPersonLogic()
    {
        if (headTransform == null) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
        transform.position = headTransform.position;
    }

    void ThirdPersonLogic()
    {
        // Обработка ввода
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Вращение игрока по горизонтали
        playerBody.Rotate(Vector3.up * mouseX);
        
        // Вращение камеры по вертикали
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, verticalClamp.x, verticalClamp.y);

        // Расчет позиции камеры
        Quaternion rotation = Quaternion.Euler(xRotation, playerBody.eulerAngles.y, 0);
        Vector3 offset = rotation * thirdPersonOffset;
        targetPosition = playerBody.position + offset;

        // Обработка коллизий
        HandleCameraCollision(ref targetPosition);

        // Плавное перемещение камеры
        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSmoothness * Time.deltaTime);
        
        // Направление взгляда камеры
        Vector3 lookDirection = playerBody.position - transform.position;
        targetRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraSmoothness * Time.deltaTime);
    }

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
            targetPos = hit.point - direction.normalized * 0.2f;
            targetPos = Vector3.Lerp(playerBody.position, targetPos, minDistance / distance);
        }
    }
}