using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerPR9 : MonoBehaviour
{
    [Header("Характеристики персонажа")]
    public int health = 100;
    public int strength = 20;
    public float speed = 5f;
    public float jumpForce = 5f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float gravity = -9.81f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Проверяем, на земле ли персонаж
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Движение
        float moveH = Input.GetAxis("Horizontal");
        float moveV = Input.GetAxis("Vertical");
        Vector3 move = transform.TransformDirection(new Vector3(moveH, 0, moveV));
        controller.Move(move * speed * Time.deltaTime);

        // Прыжок + потеря здоровья
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            health -= 10;
            Debug.Log($"Персонаж прыгнул! Текущее здоровье: {health}");
        }

        // Гравитация
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Изменение поведения при низком здоровье
        if (health <= 50 && speed > 2f)
        {
            speed = 2f;
            Debug.Log("Здоровье низкое — скорость снижена!");
        }
    }
}
