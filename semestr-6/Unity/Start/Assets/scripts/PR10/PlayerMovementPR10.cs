using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementPR10 : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;

    private CharacterController cc;
    private Vector3 velocity;
    private float gravity = -9.81f;
    private bool isGrounded;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // проверка касания земли
        isGrounded = cc.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // движение
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.TransformDirection(new Vector3(h, 0, v));
        cc.Move(move * speed * Time.deltaTime);

        // прыжок
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // гравитация
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}