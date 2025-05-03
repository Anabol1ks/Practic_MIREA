using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementPR11 : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;

    public AudioSource footstepSource;
    public AudioClip footstepClip;
    public AudioSource jumpSource;
    public AudioClip jumpClip;

    private CharacterController cc;
    private Vector3 velocity;
    private float gravity = -9.81f;
    private bool isGrounded;

    private bool wasMoving;

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = cc.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 move = transform.TransformDirection(new Vector3(h, 0, v));
        cc.Move(move * speed * Time.deltaTime);

        // Звук ходьбы
        bool isMoving = move.magnitude > 0.1f;
        if (isGrounded && isMoving && !footstepSource.isPlaying)
        {
            footstepSource.clip = footstepClip;
            footstepSource.Play();
        }
        else if (!isMoving || !isGrounded)
        {
            footstepSource.Stop();
        }

        // Прыжок
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            jumpSource.PlayOneShot(jumpClip);
        }

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}
