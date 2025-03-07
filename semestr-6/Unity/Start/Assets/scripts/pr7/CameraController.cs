using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Настройки камеры")]
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public bool isThirdPerson = false;
    public KeyCode toggleKey = KeyCode.V;

    [Header("Параметры третьего лица")]
    public Vector3 thirdPersonOffset = new Vector3(0, 2, -5);
    public float cameraCollisionRadius = 0.3f;
    public float minDistance = 1f;
    public LayerMask collisionMask;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (playerBody == null) Debug.LogError("Назначьте playerBody в инспекторе!");
    }

    void Update()
    {
        // Переключение режима
        if (Input.GetKeyDown(toggleKey))
        {
            isThirdPerson = !isThirdPerson;
        }

        // Обработка камеры
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
        // Вращение от первого лица
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        // Позиция камеры на уровне головы
        transform.position = playerBody.position + Vector3.up * 1.7f;
    }

    void ThirdPersonLogic()
    {
        // Позиция камеры с коллизиями
        Vector3 desiredPosition = playerBody.position + 
            playerBody.forward * thirdPersonOffset.z + 
            playerBody.up * thirdPersonOffset.y + 
            playerBody.right * thirdPersonOffset.x;

        RaycastHit hit;
        Vector3 direction = (desiredPosition - playerBody.position).normalized;
        float distance = Vector3.Distance(playerBody.position, desiredPosition);

        if (Physics.SphereCast(
            playerBody.position, 
            cameraCollisionRadius, 
            direction, 
            out hit, 
            distance, 
            collisionMask))
        {
            transform.position = hit.point - direction * 0.2f;
        }
        else
        {
            transform.position = desiredPosition;
        }

        // Вращение камеры
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerBody.Rotate(Vector3.up * mouseX);
        transform.rotation = Quaternion.Euler(xRotation, playerBody.eulerAngles.y, 0f);
    }
}