using UnityEngine;

public class MouseLook : MonoBehaviour
{

    public float mouseSensitivity = 100f;

    public Transform Controller;
    float xRotarion = 0f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotarion -= mouseY;
        xRotarion = Mathf.Clamp(xRotarion, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotarion, 0f, 0f);

        Controller.Rotate(Vector3.up * mouseX);

    }
}
