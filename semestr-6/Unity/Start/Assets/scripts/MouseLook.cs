using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform Controller;
    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // Скрываем курсор при старте
    }

    void Update()
    {
        // Переключение состояния курсора при нажатии G
        if (Input.GetKeyDown(KeyCode.G))
        {
            ToggleCursor();
        }

        // Обрабатываем движение камеры только если курсор заблокирован
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            Controller.Rotate(Vector3.up * mouseX);
        }
    }

    void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true; // Показываем курсор
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false; // Скрываем курсор
        }
    }
}