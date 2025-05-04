using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform thirdPersonCamPivot;
    public Camera mainCamera;

    [Header("Settings")]
    public float mouseSensitivity = 100f;
    public float cameraDistance = 5f;
    public Vector2 verticalClamp = new Vector2(-30f, 60f);
    public LayerMask cameraCollisionMask;

    [Header("First Person")]
    public Vector3 firstPersonOffset = new Vector3(0, 0.5f, 0.1f);
    public bool isFirstPerson = false;

    private float xRotation = 0f;
    private float currentDistance;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentDistance = cameraDistance;
    }

    void Update()
    {
        HandleInput();
        RotateCamera();
        UpdateCameraPosition();
    }

    void HandleInput()
    {
        // Переключение режима камеры по V
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;
        }

        // Переключение курсора по G
        if (Input.GetKeyDown(KeyCode.G))
        {
            ToggleCursor();
        }
    }

    void RotateCamera()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, verticalClamp.x, verticalClamp.y);

        // Поворот игрока по горизонтали
        player.Rotate(Vector3.up * mouseX);
        
        // Поворот камеры по вертикали
        thirdPersonCamPivot.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    void UpdateCameraPosition()
    {
        if (isFirstPerson)
        {
            // Первый режим
            mainCamera.transform.position = player.position + 
                player.TransformDirection(firstPersonOffset);
            mainCamera.transform.rotation = player.rotation * 
                Quaternion.Euler(xRotation, 0, 0);
        }
        else
        {
            // Третий режим с проверкой коллизий
            Vector3 desiredPos = thirdPersonCamPivot.position - 
                thirdPersonCamPivot.forward * cameraDistance;

            RaycastHit hit;
            if (Physics.Linecast(thirdPersonCamPivot.position, desiredPos, out hit, cameraCollisionMask))
            {
                currentDistance = Mathf.Clamp(hit.distance * 0.9f, 0.5f, cameraDistance);
            }
            else
            {
                currentDistance = cameraDistance;
            }

            mainCamera.transform.position = thirdPersonCamPivot.position - 
                thirdPersonCamPivot.forward * currentDistance;
            mainCamera.transform.LookAt(thirdPersonCamPivot);
        }
    }

    void ToggleCursor()
    {
        Cursor.lockState = (Cursor.lockState == CursorLockMode.Locked) ? 
            CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = !Cursor.visible;
    }
}